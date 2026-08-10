using Microsoft.Data.SqlClient;
using System.Data;

namespace SchulApp
{
    public partial class Klassen : Form
    {
        private int? ausgewaehlteKlassenId;

        public Klassen()
        {
            InitializeComponent();
            LehrerLaden();
            KlassenListeAktualisieren();
        }

        private void LehrerLaden()
        {
            try
            {
                const string sql = @"
                    SELECT
                        CAST(NULL AS INT) AS LehrerId,
                        CAST('(Kein Klassenlehrer)' AS NVARCHAR(100)) AS Name,
                        0 AS Sortierung
                    UNION ALL
                    SELECT
                        LehrerId,
                        Name,
                        1 AS Sortierung
                    FROM dbo.Lehrer
                    ORDER BY Sortierung, Name;";

                using SqlConnection connection = Database.GetConnection();
                using SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);

                DataTable lehrer = new DataTable();
                adapter.Fill(lehrer);

                newKlassenlehrer.DisplayMember = "Name";
                newKlassenlehrer.ValueMember = "LehrerId";
                newKlassenlehrer.DataSource = lehrer.Copy();

                editKlassenlehrer.DisplayMember = "Name";
                editKlassenlehrer.ValueMember = "LehrerId";
                editKlassenlehrer.DataSource = lehrer.Copy();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Die Lehrer konnten nicht geladen werden.\n\n" + ex.Message);
            }
        }

        private void KlassenListeAktualisieren()
        {
            try
            {
                const string sql = @"
                    SELECT
                        k.KlassenId,
                        k.Bezeichnung,
                        k.KlassenlehrerId,
                        COALESCE(l.Name, '(Kein Klassenlehrer)') AS Klassenlehrer,
                        COUNT(DISTINCT s.SchuelerId) AS AnzahlSchueler
                    FROM dbo.Klassen AS k
                    LEFT JOIN dbo.Lehrer AS l
                        ON k.KlassenlehrerId = l.LehrerId
                    LEFT JOIN dbo.Schueler AS s
                        ON k.KlassenId = s.KlasseId
                    GROUP BY
                        k.KlassenId,
                        k.Bezeichnung,
                        k.KlassenlehrerId,
                        l.Name
                    ORDER BY k.KlassenId;";

                using SqlConnection connection = Database.GetConnection();
                using SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);

                DataTable klassen = new DataTable();
                adapter.Fill(klassen);
                klassenGrid.DataSource = klassen;

                if (klassenGrid.Columns.Contains("KlassenId"))
                {
                    klassenGrid.Columns["KlassenId"].HeaderText = "ID";
                    klassenGrid.Columns["KlassenId"].Width = 55;
                }

                if (klassenGrid.Columns.Contains("KlassenlehrerId"))
                {
                    klassenGrid.Columns["KlassenlehrerId"].Visible = false;
                }

                if (klassenGrid.Columns.Contains("Bezeichnung"))
                {
                    klassenGrid.Columns["Bezeichnung"].HeaderText = "Klasse";
                    klassenGrid.Columns["Bezeichnung"].Width = 130;
                }

                if (klassenGrid.Columns.Contains("Klassenlehrer"))
                {
                    klassenGrid.Columns["Klassenlehrer"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                if (klassenGrid.Columns.Contains("AnzahlSchueler"))
                {
                    klassenGrid.Columns["AnzahlSchueler"].HeaderText = "Schüler";
                    klassenGrid.Columns["AnzahlSchueler"].Width = 70;
                }

                klassenGrid.ClearSelection();
                AuswahlZuruecksetzen();
                SchuelerDerKlasseLaden();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Die Klassen konnten nicht geladen werden.\n\n" + ex.Message);
            }
        }

        private static object LehrerParameter(ComboBox comboBox)
        {
            if (comboBox.SelectedValue == null || comboBox.SelectedValue == DBNull.Value)
            {
                return DBNull.Value;
            }

            return Convert.ToInt32(comboBox.SelectedValue);
        }

        private void createKlasseBtn_Click(object sender, EventArgs e)
        {
            string bezeichnung = newKlasseName.Text.Trim();

            if (bezeichnung == "")
            {
                MessageBox.Show("Bitte eine Klassenbezeichnung eingeben.");
                return;
            }

            try
            {
                const string sql = @"
                    INSERT INTO dbo.Klassen (Bezeichnung, KlassenlehrerId)
                    VALUES (@Bezeichnung, @KlassenlehrerId);";

                using SqlConnection connection = Database.GetConnection();
                using SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Bezeichnung", SqlDbType.NVarChar, 50).Value = bezeichnung;
                command.Parameters.Add("@KlassenlehrerId", SqlDbType.Int).Value = LehrerParameter(newKlassenlehrer);

                connection.Open();
                command.ExecuteNonQuery();

                newKlasseName.Text = "";
                newKlassenlehrer.SelectedIndex = 0;
                KlassenListeAktualisieren();
                MessageBox.Show("Klasse wurde gespeichert.");
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                MessageBox.Show("Diese Klassenbezeichnung existiert bereits.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Die Klasse konnte nicht gespeichert werden.\n\n" + ex.Message);
            }
        }

        private void klassenGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (klassenGrid.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = klassenGrid.SelectedRows[0];
            if (row.Cells["KlassenId"].Value == null)
            {
                return;
            }

            ausgewaehlteKlassenId = Convert.ToInt32(row.Cells["KlassenId"].Value);
            editKlasseName.Text = Convert.ToString(row.Cells["Bezeichnung"].Value) ?? "";
            deleteKlasseName.Text = editKlasseName.Text;

            object lehrerId = row.Cells["KlassenlehrerId"].Value;
            if (lehrerId == null || lehrerId == DBNull.Value)
            {
                editKlassenlehrer.SelectedIndex = 0;
            }
            else
            {
                editKlassenlehrer.SelectedValue = Convert.ToInt32(lehrerId);
            }

            SchuelerDerKlasseLaden();
        }

        private void updateKlasseBtn_Click(object sender, EventArgs e)
        {
            if (ausgewaehlteKlassenId == null)
            {
                MessageBox.Show("Bitte zuerst eine Klasse auswählen.");
                return;
            }

            string bezeichnung = editKlasseName.Text.Trim();
            if (bezeichnung == "")
            {
                MessageBox.Show("Bitte eine Klassenbezeichnung eingeben.");
                return;
            }

            try
            {
                const string sql = @"
                    UPDATE dbo.Klassen
                    SET Bezeichnung = @Bezeichnung,
                        KlassenlehrerId = @KlassenlehrerId
                    WHERE KlassenId = @KlassenId;";

                using SqlConnection connection = Database.GetConnection();
                using SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Bezeichnung", SqlDbType.NVarChar, 50).Value = bezeichnung;
                command.Parameters.Add("@KlassenlehrerId", SqlDbType.Int).Value = LehrerParameter(editKlassenlehrer);
                command.Parameters.Add("@KlassenId", SqlDbType.Int).Value = ausgewaehlteKlassenId.Value;

                connection.Open();
                int anzahl = command.ExecuteNonQuery();

                if (anzahl == 0)
                {
                    MessageBox.Show("Die Klasse wurde nicht gefunden.");
                }
                else
                {
                    MessageBox.Show("Klasse wurde bearbeitet.");
                }

                KlassenListeAktualisieren();
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                MessageBox.Show("Diese Klassenbezeichnung existiert bereits.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Die Klasse konnte nicht bearbeitet werden.\n\n" + ex.Message);
            }
        }

        private void deleteKlasseBtn_Click(object sender, EventArgs e)
        {
            if (ausgewaehlteKlassenId == null)
            {
                MessageBox.Show("Bitte zuerst eine Klasse auswählen.");
                return;
            }

            DialogResult antwort = MessageBox.Show(
                $"Soll die Klasse {deleteKlasseName.Text} wirklich gelöscht werden?",
                "Klasse löschen",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (antwort != DialogResult.Yes)
            {
                return;
            }

            try
            {
                const string sql = @"
                    DELETE FROM dbo.Klassen
                    WHERE KlassenId = @KlassenId;";

                using SqlConnection connection = Database.GetConnection();
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@KlassenId", SqlDbType.Int).Value = ausgewaehlteKlassenId.Value;

                connection.Open();
                command.ExecuteNonQuery();

                KlassenListeAktualisieren();
                MessageBox.Show("Klasse wurde gelöscht.");
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show(
                    "Diese Klasse kann noch nicht gelöscht werden, weil ihr Schüler oder Kurse zugeordnet sind.\n\n" +
                    "Ordne die Schüler zuerst einer anderen Klasse zu und entferne oder ändere die Kurse."
                );
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Die Klasse konnte nicht gelöscht werden.\n\n" + ex.Message);
            }
        }

        private void SchuelerDerKlasseLaden()
        {
            if (ausgewaehlteKlassenId == null)
            {
                klassenSchuelerGrid.DataSource = null;
                return;
            }

            try
            {
                const string sql = @"
                    SELECT SchuelerId, Name
                    FROM dbo.Schueler
                    WHERE KlasseId = @KlassenId
                    ORDER BY SchuelerId;";

                using SqlConnection connection = Database.GetConnection();
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@KlassenId", SqlDbType.Int).Value = ausgewaehlteKlassenId.Value;
                using SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable schueler = new DataTable();
                adapter.Fill(schueler);
                klassenSchuelerGrid.DataSource = schueler;

                if (klassenSchuelerGrid.Columns.Contains("SchuelerId"))
                {
                    klassenSchuelerGrid.Columns["SchuelerId"].HeaderText = "ID";
                    klassenSchuelerGrid.Columns["SchuelerId"].Width = 55;
                }

                if (klassenSchuelerGrid.Columns.Contains("Name"))
                {
                    klassenSchuelerGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Die Schüler der Klasse konnten nicht geladen werden.\n\n" + ex.Message);
            }
        }

        private void reloadBtn_Click(object sender, EventArgs e)
        {
            LehrerLaden();
            KlassenListeAktualisieren();
        }

        private void AuswahlZuruecksetzen()
        {
            ausgewaehlteKlassenId = null;
            editKlasseName.Text = "";
            deleteKlasseName.Text = "";
            if (editKlassenlehrer.Items.Count > 0)
            {
                editKlassenlehrer.SelectedIndex = 0;
            }
        }

        private void back_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
