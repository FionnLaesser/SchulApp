using Microsoft.Data.SqlClient;
using System.Data;

namespace SchulApp
{
    public partial class Kurse : Form
    {
        private int? ausgewaehlteKursId;

        public Kurse()
        {
            InitializeComponent();
            AuswahlDatenLaden();
            ThemeManager.Anwenden(this);
            KurseListeAktualisieren();
        }

        private void AuswahlDatenLaden()
        {
            try
            {
                const string klassenSql = @"
                    SELECT KlassenId, Bezeichnung
                    FROM dbo.Klassen
                    ORDER BY KlassenId;";

                const string lehrerSql = @"
                    SELECT LehrerId, Name
                    FROM dbo.Lehrer
                    ORDER BY LehrerId;";

                using SqlConnection connection = Database.GetConnection();
                connection.Open();

                using SqlDataAdapter klassenAdapter = new SqlDataAdapter(klassenSql, connection);
                DataTable klassen = new DataTable();
                klassenAdapter.Fill(klassen);

                using SqlDataAdapter lehrerAdapter = new SqlDataAdapter(lehrerSql, connection);
                DataTable lehrer = new DataTable();
                lehrerAdapter.Fill(lehrer);

                newKursKlasse.DisplayMember = "Bezeichnung";
                newKursKlasse.ValueMember = "KlassenId";
                newKursKlasse.DataSource = klassen.Copy();

                editKursKlasse.DisplayMember = "Bezeichnung";
                editKursKlasse.ValueMember = "KlassenId";
                editKursKlasse.DataSource = klassen.Copy();

                newKursLehrer.DisplayMember = "Name";
                newKursLehrer.ValueMember = "LehrerId";
                newKursLehrer.DataSource = lehrer.Copy();

                editKursLehrer.DisplayMember = "Name";
                editKursLehrer.ValueMember = "LehrerId";
                editKursLehrer.DataSource = lehrer.Copy();

                bool kannSpeichern = klassen.Rows.Count > 0 && lehrer.Rows.Count > 0;
                createKursBtn.Enabled = kannSpeichern;
                updateKursBtn.Enabled = kannSpeichern;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Klassen und Lehrer konnten nicht geladen werden.\n\n" + ex.Message);
            }
        }

        private void KurseListeAktualisieren()
        {
            try
            {
                const string sql = @"
                    SELECT
                        ku.KursId,
                        ku.Name,
                        ku.KlasseId,
                        kl.Bezeichnung AS Klasse,
                        ku.LehrerId,
                        l.Name AS Lehrer
                    FROM dbo.Kurse AS ku
                    INNER JOIN dbo.Klassen AS kl
                        ON ku.KlasseId = kl.KlassenId
                    INNER JOIN dbo.Lehrer AS l
                        ON ku.LehrerId = l.LehrerId
                    ORDER BY ku.Name, kl.KlassenId;";

                using SqlConnection connection = Database.GetConnection();
                using SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);

                DataTable kurse = new DataTable();
                adapter.Fill(kurse);
                kurseGrid.DataSource = kurse;

                if (kurseGrid.Columns.Contains("KursId"))
                {
                    kurseGrid.Columns["KursId"].HeaderText = "ID";
                    kurseGrid.Columns["KursId"].Width = 50;
                }

                if (kurseGrid.Columns.Contains("KlasseId"))
                {
                    kurseGrid.Columns["KlasseId"].Visible = false;
                }

                if (kurseGrid.Columns.Contains("LehrerId"))
                {
                    kurseGrid.Columns["LehrerId"].Visible = false;
                }

                if (kurseGrid.Columns.Contains("Name"))
                {
                    kurseGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                if (kurseGrid.Columns.Contains("Klasse"))
                {
                    kurseGrid.Columns["Klasse"].Width = 100;
                }

                if (kurseGrid.Columns.Contains("Lehrer"))
                {
                    kurseGrid.Columns["Lehrer"].Width = 180;
                }

                kurseGrid.ClearSelection();
                AuswahlZuruecksetzen();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Die Kurse konnten nicht geladen werden.\n\n" + ex.Message);
            }
        }

        private bool NeueDatenPruefen(TextBox nameBox, ComboBox klasseBox, ComboBox lehrerBox, out string name, out int klasseId, out int lehrerId)
        {
            name = nameBox.Text.Trim();
            klasseId = 0;
            lehrerId = 0;

            if (name == "")
            {
                MessageBox.Show("Bitte einen Kursnamen eingeben.");
                return false;
            }

            if (klasseBox.SelectedValue == null)
            {
                MessageBox.Show("Bitte eine Klasse auswählen.");
                return false;
            }

            if (lehrerBox.SelectedValue == null)
            {
                MessageBox.Show("Bitte einen Lehrer auswählen.");
                return false;
            }

            klasseId = Convert.ToInt32(klasseBox.SelectedValue);
            lehrerId = Convert.ToInt32(lehrerBox.SelectedValue);
            return true;
        }

        private void createKursBtn_Click(object sender, EventArgs e)
        {
            if (!NeueDatenPruefen(newKursName, newKursKlasse, newKursLehrer, out string name, out int klasseId, out int lehrerId))
            {
                return;
            }

            try
            {
                const string sql = @"
                    INSERT INTO dbo.Kurse (Name, KlasseId, LehrerId)
                    VALUES (@Name, @KlasseId, @LehrerId);";

                using SqlConnection connection = Database.GetConnection();
                using SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name;
                command.Parameters.Add("@KlasseId", SqlDbType.Int).Value = klasseId;
                command.Parameters.Add("@LehrerId", SqlDbType.Int).Value = lehrerId;

                connection.Open();
                command.ExecuteNonQuery();

                newKursName.Text = "";
                KurseListeAktualisieren();
                MessageBox.Show("Kurs wurde gespeichert.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Der Kurs konnte nicht gespeichert werden.\n\n" + ex.Message);
            }
        }

        private void kurseGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (kurseGrid.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = kurseGrid.SelectedRows[0];
            if (row.Cells["KursId"].Value == null)
            {
                return;
            }

            ausgewaehlteKursId = Convert.ToInt32(row.Cells["KursId"].Value);
            string name = Convert.ToString(row.Cells["Name"].Value) ?? "";

            editKursName.Text = name;
            deleteKursName.Text = name;
            editKursKlasse.SelectedValue = Convert.ToInt32(row.Cells["KlasseId"].Value);
            editKursLehrer.SelectedValue = Convert.ToInt32(row.Cells["LehrerId"].Value);
        }

        private void updateKursBtn_Click(object sender, EventArgs e)
        {
            if (ausgewaehlteKursId == null)
            {
                MessageBox.Show("Bitte zuerst einen Kurs auswählen.");
                return;
            }

            if (!NeueDatenPruefen(editKursName, editKursKlasse, editKursLehrer, out string name, out int klasseId, out int lehrerId))
            {
                return;
            }

            try
            {
                const string sql = @"
                    UPDATE dbo.Kurse
                    SET Name = @Name,
                        KlasseId = @KlasseId,
                        LehrerId = @LehrerId
                    WHERE KursId = @KursId;";

                using SqlConnection connection = Database.GetConnection();
                using SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name;
                command.Parameters.Add("@KlasseId", SqlDbType.Int).Value = klasseId;
                command.Parameters.Add("@LehrerId", SqlDbType.Int).Value = lehrerId;
                command.Parameters.Add("@KursId", SqlDbType.Int).Value = ausgewaehlteKursId.Value;

                connection.Open();
                int anzahl = command.ExecuteNonQuery();

                MessageBox.Show(anzahl == 1 ? "Kurs wurde bearbeitet." : "Der Kurs wurde nicht gefunden.");
                KurseListeAktualisieren();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Der Kurs konnte nicht bearbeitet werden.\n\n" + ex.Message);
            }
        }

        private void deleteKursBtn_Click(object sender, EventArgs e)
        {
            if (ausgewaehlteKursId == null)
            {
                MessageBox.Show("Bitte zuerst einen Kurs auswählen.");
                return;
            }

            DialogResult antwort = MessageBox.Show(
                $"Soll der Kurs {deleteKursName.Text} wirklich gelöscht werden?",
                "Kurs löschen",
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
                    DELETE FROM dbo.Kurse
                    WHERE KursId = @KursId;";

                using SqlConnection connection = Database.GetConnection();
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@KursId", SqlDbType.Int).Value = ausgewaehlteKursId.Value;

                connection.Open();
                command.ExecuteNonQuery();

                KurseListeAktualisieren();
                MessageBox.Show("Kurs wurde gelöscht.");
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show(
                    "Dieser Kurs kann noch nicht gelöscht werden, weil er im Stundenplan verwendet wird.\n\n" +
                    "Lösche oder ändere zuerst die entsprechenden Stundenplaneinträge."
                );
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Der Kurs konnte nicht gelöscht werden.\n\n" + ex.Message);
            }
        }

        private void reloadBtn_Click(object sender, EventArgs e)
        {
            AuswahlDatenLaden();
            KurseListeAktualisieren();
        }

        private void AuswahlZuruecksetzen()
        {
            ausgewaehlteKursId = null;
            editKursName.Text = "";
            deleteKursName.Text = "";
        }

        private void back_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
