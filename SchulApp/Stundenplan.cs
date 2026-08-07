using Microsoft.Data.SqlClient;
using System.Data;

namespace SchulApp
{
    public partial class Stundenplan : Form
    {
        private int? ausgewaehlteStundenplanId;

        public Stundenplan()
        {
            InitializeComponent();
            WochentageEinrichten();
            KurseLaden();
            StundenplanLaden();
        }

        private void WochentageEinrichten()
        {
            DataTable tage = new DataTable();
            tage.Columns.Add("Wert", typeof(int));
            tage.Columns.Add("Text", typeof(string));
            tage.Rows.Add(1, "Montag");
            tage.Rows.Add(2, "Dienstag");
            tage.Rows.Add(3, "Mittwoch");
            tage.Rows.Add(4, "Donnerstag");
            tage.Rows.Add(5, "Freitag");

            newWochentag.DisplayMember = "Text";
            newWochentag.ValueMember = "Wert";
            newWochentag.DataSource = tage.Copy();

            editWochentag.DisplayMember = "Text";
            editWochentag.ValueMember = "Wert";
            editWochentag.DataSource = tage.Copy();
        }

        private void KurseLaden()
        {
            try
            {
                const string sql = @"
                    SELECT
                        ku.KursId,
                        ku.Name + ' | ' + kl.Bezeichnung + ' | ' + l.Name AS Anzeige
                    FROM dbo.Kurse AS ku
                    INNER JOIN dbo.Klassen AS kl ON ku.KlasseId = kl.KlassenId
                    INNER JOIN dbo.Lehrer AS l ON ku.LehrerId = l.LehrerId
                    ORDER BY ku.Name, kl.Bezeichnung;";

                using SqlConnection connection = Database.GetConnection();
                using SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);

                DataTable kurse = new DataTable();
                adapter.Fill(kurse);

                newKurs.DisplayMember = "Anzeige";
                newKurs.ValueMember = "KursId";
                newKurs.DataSource = kurse.Copy();

                editKurs.DisplayMember = "Anzeige";
                editKurs.ValueMember = "KursId";
                editKurs.DataSource = kurse.Copy();

                bool vorhanden = kurse.Rows.Count > 0;
                createEintragBtn.Enabled = vorhanden;
                updateEintragBtn.Enabled = vorhanden;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Die Kurse konnten nicht geladen werden.\n\n" + ex.Message);
            }
        }

        private void StundenplanLaden()
        {
            try
            {
                const string sql = @"
                    SELECT
                        sp.StundenplanId,
                        sp.KursId,
                        sp.Wochentag AS WochentagNr,
                        CASE sp.Wochentag
                            WHEN 1 THEN 'Montag'
                            WHEN 2 THEN 'Dienstag'
                            WHEN 3 THEN 'Mittwoch'
                            WHEN 4 THEN 'Donnerstag'
                            WHEN 5 THEN 'Freitag'
                        END AS Wochentag,
                        sp.Startzeit,
                        sp.Endzeit,
                        ku.Name AS Kurs,
                        kl.Bezeichnung AS Klasse,
                        l.Name AS Lehrer,
                        sp.Raum
                    FROM dbo.Stundenplan AS sp
                    INNER JOIN dbo.Kurse AS ku ON sp.KursId = ku.KursId
                    INNER JOIN dbo.Klassen AS kl ON ku.KlasseId = kl.KlassenId
                    INNER JOIN dbo.Lehrer AS l ON ku.LehrerId = l.LehrerId
                    ORDER BY sp.Wochentag, sp.Startzeit;";

                using SqlConnection connection = Database.GetConnection();
                using SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);

                DataTable plan = new DataTable();
                adapter.Fill(plan);
                stundenplanGrid.DataSource = plan;

                if (stundenplanGrid.Columns.Contains("StundenplanId"))
                {
                    stundenplanGrid.Columns["StundenplanId"].HeaderText = "ID";
                    stundenplanGrid.Columns["StundenplanId"].Width = 45;
                }

                if (stundenplanGrid.Columns.Contains("KursId"))
                {
                    stundenplanGrid.Columns["KursId"].Visible = false;
                }

                if (stundenplanGrid.Columns.Contains("WochentagNr"))
                {
                    stundenplanGrid.Columns["WochentagNr"].Visible = false;
                }

                if (stundenplanGrid.Columns.Contains("Wochentag"))
                {
                    stundenplanGrid.Columns["Wochentag"].Width = 90;
                }

                if (stundenplanGrid.Columns.Contains("Startzeit"))
                {
                    stundenplanGrid.Columns["Startzeit"].HeaderText = "Von";
                    stundenplanGrid.Columns["Startzeit"].DefaultCellStyle.Format = @"hh\:mm";
                    stundenplanGrid.Columns["Startzeit"].Width = 60;
                }

                if (stundenplanGrid.Columns.Contains("Endzeit"))
                {
                    stundenplanGrid.Columns["Endzeit"].HeaderText = "Bis";
                    stundenplanGrid.Columns["Endzeit"].DefaultCellStyle.Format = @"hh\:mm";
                    stundenplanGrid.Columns["Endzeit"].Width = 60;
                }

                if (stundenplanGrid.Columns.Contains("Kurs"))
                {
                    stundenplanGrid.Columns["Kurs"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                if (stundenplanGrid.Columns.Contains("Klasse"))
                {
                    stundenplanGrid.Columns["Klasse"].Width = 75;
                }

                if (stundenplanGrid.Columns.Contains("Lehrer"))
                {
                    stundenplanGrid.Columns["Lehrer"].Width = 140;
                }

                if (stundenplanGrid.Columns.Contains("Raum"))
                {
                    stundenplanGrid.Columns["Raum"].Width = 60;
                }

                stundenplanGrid.ClearSelection();
                AuswahlZuruecksetzen();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Der Stundenplan konnte nicht geladen werden.\n\n" + ex.Message);
            }
        }

        private static TimeSpan ZeitAusPicker(DateTimePicker picker)
        {
            return new TimeSpan(picker.Value.Hour, picker.Value.Minute, 0);
        }

        private bool EingabePruefen(ComboBox kursBox, ComboBox tagBox, DateTimePicker startPicker, DateTimePicker endePicker, out int kursId, out int wochentag, out TimeSpan start, out TimeSpan ende)
        {
            kursId = 0;
            wochentag = 0;
            start = ZeitAusPicker(startPicker);
            ende = ZeitAusPicker(endePicker);

            if (kursBox.SelectedValue == null)
            {
                MessageBox.Show("Bitte einen Kurs auswählen.");
                return false;
            }

            if (tagBox.SelectedValue == null)
            {
                MessageBox.Show("Bitte einen Wochentag auswählen.");
                return false;
            }

            if (ende <= start)
            {
                MessageBox.Show("Die Endzeit muss nach der Startzeit liegen.");
                return false;
            }

            kursId = Convert.ToInt32(kursBox.SelectedValue);
            wochentag = Convert.ToInt32(tagBox.SelectedValue);
            return true;
        }

        private void createEintragBtn_Click(object sender, EventArgs e)
        {
            if (!EingabePruefen(newKurs, newWochentag, newStartzeit, newEndzeit, out int kursId, out int tag, out TimeSpan start, out TimeSpan ende))
            {
                return;
            }

            try
            {
                const string sql = @"
                    INSERT INTO dbo.Stundenplan
                        (KursId, Wochentag, Startzeit, Endzeit, Raum)
                    VALUES
                        (@KursId, @Wochentag, @Startzeit, @Endzeit, @Raum);";

                using SqlConnection connection = Database.GetConnection();
                using SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@KursId", SqlDbType.Int).Value = kursId;
                command.Parameters.Add("@Wochentag", SqlDbType.TinyInt).Value = tag;
                command.Parameters.Add("@Startzeit", SqlDbType.Time).Value = start;
                command.Parameters.Add("@Endzeit", SqlDbType.Time).Value = ende;
                command.Parameters.Add("@Raum", SqlDbType.NVarChar, 50).Value =
                    string.IsNullOrWhiteSpace(newRaum.Text) ? DBNull.Value : newRaum.Text.Trim();

                connection.Open();
                command.ExecuteNonQuery();

                newRaum.Text = "";
                StundenplanLaden();
                MessageBox.Show("Stundenplaneintrag wurde gespeichert.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Der Stundenplaneintrag konnte nicht gespeichert werden.\n\n" + ex.Message);
            }
        }

        private void stundenplanGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (stundenplanGrid.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = stundenplanGrid.SelectedRows[0];
            if (row.Cells["StundenplanId"].Value == null)
            {
                return;
            }

            ausgewaehlteStundenplanId = Convert.ToInt32(row.Cells["StundenplanId"].Value);
            editKurs.SelectedValue = Convert.ToInt32(row.Cells["KursId"].Value);
            editWochentag.SelectedValue = Convert.ToInt32(row.Cells["WochentagNr"].Value);

            TimeSpan start = (TimeSpan)row.Cells["Startzeit"].Value;
            TimeSpan ende = (TimeSpan)row.Cells["Endzeit"].Value;
            editStartzeit.Value = DateTime.Today.Add(start);
            editEndzeit.Value = DateTime.Today.Add(ende);
            editRaum.Text = row.Cells["Raum"].Value == DBNull.Value ? "" : Convert.ToString(row.Cells["Raum"].Value) ?? "";
            deleteInfo.Text = $"{row.Cells["Wochentag"].Value}, {start:hh\\:mm} - {row.Cells["Kurs"].Value}";
        }

        private void updateEintragBtn_Click(object sender, EventArgs e)
        {
            if (ausgewaehlteStundenplanId == null)
            {
                MessageBox.Show("Bitte zuerst einen Stundenplaneintrag auswählen.");
                return;
            }

            if (!EingabePruefen(editKurs, editWochentag, editStartzeit, editEndzeit, out int kursId, out int tag, out TimeSpan start, out TimeSpan ende))
            {
                return;
            }

            try
            {
                const string sql = @"
                    UPDATE dbo.Stundenplan
                    SET KursId = @KursId,
                        Wochentag = @Wochentag,
                        Startzeit = @Startzeit,
                        Endzeit = @Endzeit,
                        Raum = @Raum
                    WHERE StundenplanId = @StundenplanId;";

                using SqlConnection connection = Database.GetConnection();
                using SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@KursId", SqlDbType.Int).Value = kursId;
                command.Parameters.Add("@Wochentag", SqlDbType.TinyInt).Value = tag;
                command.Parameters.Add("@Startzeit", SqlDbType.Time).Value = start;
                command.Parameters.Add("@Endzeit", SqlDbType.Time).Value = ende;
                command.Parameters.Add("@Raum", SqlDbType.NVarChar, 50).Value =
                    string.IsNullOrWhiteSpace(editRaum.Text) ? DBNull.Value : editRaum.Text.Trim();
                command.Parameters.Add("@StundenplanId", SqlDbType.Int).Value = ausgewaehlteStundenplanId.Value;

                connection.Open();
                int anzahl = command.ExecuteNonQuery();

                MessageBox.Show(anzahl == 1 ? "Stundenplaneintrag wurde bearbeitet." : "Der Eintrag wurde nicht gefunden.");
                StundenplanLaden();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Der Stundenplaneintrag konnte nicht bearbeitet werden.\n\n" + ex.Message);
            }
        }

        private void deleteEintragBtn_Click(object sender, EventArgs e)
        {
            if (ausgewaehlteStundenplanId == null)
            {
                MessageBox.Show("Bitte zuerst einen Stundenplaneintrag auswählen.");
                return;
            }

            DialogResult antwort = MessageBox.Show(
                $"Soll dieser Eintrag wirklich gelöscht werden?\n\n{deleteInfo.Text}",
                "Stundenplaneintrag löschen",
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
                    DELETE FROM dbo.Stundenplan
                    WHERE StundenplanId = @StundenplanId;";

                using SqlConnection connection = Database.GetConnection();
                using SqlCommand command = new SqlCommand(sql, connection);
                command.Parameters.Add("@StundenplanId", SqlDbType.Int).Value = ausgewaehlteStundenplanId.Value;

                connection.Open();
                command.ExecuteNonQuery();

                StundenplanLaden();
                MessageBox.Show("Stundenplaneintrag wurde gelöscht.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Der Stundenplaneintrag konnte nicht gelöscht werden.\n\n" + ex.Message);
            }
        }

        private void reloadBtn_Click(object sender, EventArgs e)
        {
            KurseLaden();
            StundenplanLaden();
        }

        private void AuswahlZuruecksetzen()
        {
            ausgewaehlteStundenplanId = null;
            editRaum.Text = "";
            deleteInfo.Text = "";
        }

        private void back_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
