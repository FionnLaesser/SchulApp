using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace SchulApp
{
    public partial class Lehrer : Form
    {
        private int? ausgewaehlteLehrerId = null;

        public Lehrer()
        {
            InitializeComponent();
            LehrerListeAktualisieren();
        }


        // =========================================================
        // LEHRER AUS SQL LADEN UND ANZEIGEN
        // =========================================================

        private void LehrerListeAktualisieren()
        {
            try
            {
                const string sql = @"
                    SELECT
                        LehrerId,
                        Name,
                        Email,
                        Telefon
                    FROM dbo.Lehrer
                    ORDER BY Name;";

                using SqlConnection connection = Database.GetConnection();
                using SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);

                DataTable lehrer = new DataTable();
                adapter.Fill(lehrer);

                lehrerGrid.DataSource = lehrer;

                if (lehrerGrid.Columns.Contains("LehrerId"))
                {
                    lehrerGrid.Columns["LehrerId"].HeaderText = "ID";
                    lehrerGrid.Columns["LehrerId"].Width = 55;
                }

                if (lehrerGrid.Columns.Contains("Name"))
                {
                    lehrerGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                if (lehrerGrid.Columns.Contains("Email"))
                {
                    lehrerGrid.Columns["Email"].Width = 190;
                }

                if (lehrerGrid.Columns.Contains("Telefon"))
                {
                    lehrerGrid.Columns["Telefon"].Width = 115;
                }

                lehrerGrid.ClearSelection();
                AuswahlZuruecksetzen();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Die Lehrer konnten nicht aus der Datenbank geladen werden.\n\n" +
                    ex.Message
                );
            }
        }


        // =========================================================
        // NEUEN LEHRER ERSTELLEN
        // =========================================================

        private void OKnewTeacherBtn_Click(object sender, EventArgs e)
        {
            string name = newTeacherName.Text.Trim();
            string email = newTeacherEmail.Text.Trim();
            string telefon = newTeacherTelefon.Text.Trim();

            if (name == "")
            {
                MessageBox.Show("Bitte einen Namen eingeben.");
                return;
            }

            try
            {
                const string sql = @"
                    INSERT INTO dbo.Lehrer (Name, Email, Telefon)
                    VALUES (@Name, @Email, @Telefon);";

                using SqlConnection connection = Database.GetConnection();
                using SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name;
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value =
                    email == "" ? DBNull.Value : email;
                command.Parameters.Add("@Telefon", SqlDbType.NVarChar, 50).Value =
                    telefon == "" ? DBNull.Value : telefon;

                connection.Open();
                command.ExecuteNonQuery();

                newTeacherName.Text = "";
                newTeacherEmail.Text = "";
                newTeacherTelefon.Text = "";

                LehrerListeAktualisieren();

                MessageBox.Show("Lehrer wurde gespeichert.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Der Lehrer konnte nicht gespeichert werden.\n\n" +
                    ex.Message
                );
            }
        }


        // =========================================================
        // AUSGEWAEHLTEN LEHRER LADEN
        // =========================================================

        private void lehrerGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (lehrerGrid.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = lehrerGrid.SelectedRows[0];

            if (row.Cells["LehrerId"].Value == null)
            {
                return;
            }

            ausgewaehlteLehrerId = Convert.ToInt32(
                row.Cells["LehrerId"].Value
            );

            string name = Convert.ToString(row.Cells["Name"].Value) ?? "";
            string email = Convert.ToString(row.Cells["Email"].Value) ?? "";
            string telefon = Convert.ToString(row.Cells["Telefon"].Value) ?? "";

            currentTeacherName.Text = name;
            editTeacherName.Text = name;
            editTeacherEmail.Text = email;
            editTeacherTelefon.Text = telefon;
            deleteTeacherName.Text = name;

            LehrerInformationenLaden();
        }


        // =========================================================
        // LEHRER BEARBEITEN
        // Die LehrerId bestimmt eindeutig, welcher Datensatz geändert wird.
        // =========================================================

        private void OKchangeTeacherBtn_Click(object sender, EventArgs e)
        {
            if (ausgewaehlteLehrerId == null)
            {
                MessageBox.Show("Bitte zuerst einen Lehrer in der Tabelle auswählen.");
                return;
            }

            string name = editTeacherName.Text.Trim();
            string email = editTeacherEmail.Text.Trim();
            string telefon = editTeacherTelefon.Text.Trim();

            if (name == "")
            {
                MessageBox.Show("Bitte einen Namen eingeben.");
                return;
            }

            try
            {
                const string sql = @"
                    UPDATE dbo.Lehrer
                    SET
                        Name = @Name,
                        Email = @Email,
                        Telefon = @Telefon
                    WHERE LehrerId = @LehrerId;";

                using SqlConnection connection = Database.GetConnection();
                using SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = name;
                command.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value =
                    email == "" ? DBNull.Value : email;
                command.Parameters.Add("@Telefon", SqlDbType.NVarChar, 50).Value =
                    telefon == "" ? DBNull.Value : telefon;
                command.Parameters.Add("@LehrerId", SqlDbType.Int).Value =
                    ausgewaehlteLehrerId.Value;

                connection.Open();
                int anzahl = command.ExecuteNonQuery();

                if (anzahl == 0)
                {
                    MessageBox.Show("Der Lehrer wurde nicht gefunden.");
                    LehrerListeAktualisieren();
                    return;
                }

                LehrerListeAktualisieren();

                MessageBox.Show("Lehrer wurde bearbeitet.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Der Lehrer konnte nicht bearbeitet werden.\n\n" +
                    ex.Message
                );
            }
        }


        // =========================================================
        // LEHRER LOESCHEN
        // =========================================================

        private void OKdeleteTeacherBtn_Click(object sender, EventArgs e)
        {
            if (ausgewaehlteLehrerId == null)
            {
                MessageBox.Show("Bitte zuerst einen Lehrer in der Tabelle auswählen.");
                return;
            }

            string name = deleteTeacherName.Text;

            DialogResult antwort = MessageBox.Show(
                $"Soll {name} wirklich gelöscht werden?",
                "Lehrer löschen",
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
                    DELETE FROM dbo.Lehrer
                    WHERE LehrerId = @LehrerId;";

                using SqlConnection connection = Database.GetConnection();
                using SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@LehrerId", SqlDbType.Int).Value =
                    ausgewaehlteLehrerId.Value;

                connection.Open();
                int anzahl = command.ExecuteNonQuery();

                if (anzahl == 0)
                {
                    MessageBox.Show("Der Lehrer wurde nicht gefunden.");
                    LehrerListeAktualisieren();
                    return;
                }

                LehrerListeAktualisieren();

                MessageBox.Show("Lehrer wurde gelöscht.");
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                MessageBox.Show(
                    "Dieser Lehrer kann noch nicht gelöscht werden, weil er einer Klasse oder einem Kurs zugeordnet ist.\n\n" +
                    "Entferne zuerst diese Zuordnung."
                );
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Der Lehrer konnte nicht gelöscht werden.\n\n" +
                    ex.Message
                );
            }
        }


        // =========================================================
        // FREIE INFORMATIONEN DES AUSGEWAEHLTEN LEHRERS LADEN
        // =========================================================

        private void LehrerInformationenLaden()
        {
            if (ausgewaehlteLehrerId == null)
            {
                lehrerInfoGrid.DataSource = null;
                return;
            }

            try
            {
                const string sql = @"
                    SELECT
                        LehrerInformationId,
                        Titel,
                        Information
                    FROM dbo.LehrerInformationen
                    WHERE LehrerId = @LehrerId
                    ORDER BY Titel;";

                using SqlConnection connection = Database.GetConnection();
                using SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@LehrerId", SqlDbType.Int).Value =
                    ausgewaehlteLehrerId.Value;

                using SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable informationen = new DataTable();
                adapter.Fill(informationen);

                lehrerInfoGrid.DataSource = informationen;

                if (lehrerInfoGrid.Columns.Contains("LehrerInformationId"))
                {
                    lehrerInfoGrid.Columns["LehrerInformationId"].Visible = false;
                }

                if (lehrerInfoGrid.Columns.Contains("Titel"))
                {
                    lehrerInfoGrid.Columns["Titel"].Width = 150;
                }

                if (lehrerInfoGrid.Columns.Contains("Information"))
                {
                    lehrerInfoGrid.Columns["Information"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;
                }

                lehrerInfoGrid.ClearSelection();
                infoTitel.Text = "";
                info.Text = "";
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Die Lehrerinformationen konnten nicht geladen werden.\n\n" +
                    ex.Message
                );
            }
        }


        // =========================================================
        // INFORMATION SPEICHERN ODER AKTUALISIEREN
        // Gleicher Titel beim gleichen Lehrer wird aktualisiert.
        // =========================================================

        private void OKchangingInfo_Click(object sender, EventArgs e)
        {
            if (ausgewaehlteLehrerId == null)
            {
                MessageBox.Show("Bitte zuerst einen Lehrer in der Tabelle auswählen.");
                return;
            }

            string titel = infoTitel.Text.Trim();
            string information = info.Text.Trim();

            if (titel == "" || information == "")
            {
                MessageBox.Show("Bitte Titel und Information ausfüllen.");
                return;
            }

            try
            {
                const string sql = @"
                    UPDATE dbo.LehrerInformationen
                    SET Information = @Information
                    WHERE LehrerId = @LehrerId
                      AND Titel = @Titel;

                    IF @@ROWCOUNT = 0
                    BEGIN
                        INSERT INTO dbo.LehrerInformationen
                            (LehrerId, Titel, Information)
                        VALUES
                            (@LehrerId, @Titel, @Information);
                    END;";

                using SqlConnection connection = Database.GetConnection();
                using SqlCommand command = new SqlCommand(sql, connection);

                command.Parameters.Add("@LehrerId", SqlDbType.Int).Value =
                    ausgewaehlteLehrerId.Value;
                command.Parameters.Add("@Titel", SqlDbType.NVarChar, 100).Value = titel;
                command.Parameters.Add("@Information", SqlDbType.NVarChar, 500).Value = information;

                connection.Open();
                command.ExecuteNonQuery();

                LehrerInformationenLaden();

                MessageBox.Show("Information wurde gespeichert.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show(
                    "Die Information konnte nicht gespeichert werden.\n\n" +
                    ex.Message
                );
            }
        }


        // =========================================================
        // INFO AUS DER TABELLE ZUM BEARBEITEN UEBERNEHMEN
        // =========================================================

        private void lehrerInfoGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (lehrerInfoGrid.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = lehrerInfoGrid.SelectedRows[0];

            infoTitel.Text = Convert.ToString(row.Cells["Titel"].Value) ?? "";
            info.Text = Convert.ToString(row.Cells["Information"].Value) ?? "";
        }


        // =========================================================
        // LEHRER NEU LADEN
        // =========================================================

        private void reloadTeacherBtn_Click(object sender, EventArgs e)
        {
            LehrerListeAktualisieren();
        }


        // =========================================================
        // AUSWAHL ZURUECKSETZEN
        // =========================================================

        private void AuswahlZuruecksetzen()
        {
            ausgewaehlteLehrerId = null;

            currentTeacherName.Text = "";
            editTeacherName.Text = "";
            editTeacherEmail.Text = "";
            editTeacherTelefon.Text = "";
            deleteTeacherName.Text = "";
            infoTitel.Text = "";
            info.Text = "";

            lehrerInfoGrid.DataSource = null;
        }


        // =========================================================
        // ZURUECK ZUM HAUPTMENUE
        // =========================================================

        private void back_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
