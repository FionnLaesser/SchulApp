using Microsoft.Data.SqlClient;
using SchulApp.Data;
using SchulApp.Services;
using System.Data;

namespace SchulApp
{
    public partial class Schueler : Form
    {
        private readonly SchuelerService schuelerService;
        private int? ausgewaehlteSchuelerId;

        public Schueler()
        {
            InitializeComponent();
            schuelerService = new SchuelerService(new SqlSchuelerRepository());

            KlassenLaden();
            SchuelerListeAktualisieren();
        }

        private void KlassenLaden()
        {
            try
            {
                const string sql = @"
                    SELECT KlassenId, Bezeichnung
                    FROM dbo.Klassen
                    ORDER BY KlassenId;";

                using SqlConnection connection = Database.GetConnection();
                using SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);

                DataTable klassen = new DataTable();
                adapter.Fill(klassen);

                newStudentKlasse.DisplayMember = "Bezeichnung";
                newStudentKlasse.ValueMember = "KlassenId";
                newStudentKlasse.DataSource = klassen.Copy();

                editKlasse.DisplayMember = "Bezeichnung";
                editKlasse.ValueMember = "KlassenId";
                editKlasse.DataSource = klassen.Copy();

                bool vorhanden = klassen.Rows.Count > 0;
                OKnewStudent.Enabled = vorhanden;
                OKchangeName.Enabled = vorhanden;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Die Klassen konnten nicht geladen werden.\n\n" + ex.Message);
            }
        }

        private void SchuelerListeAktualisieren()
        {
            try
            {
                studentGrid.DataSource = schuelerService.AlleLaden()
    .OrderBy(s => s.SchuelerId)
    .ToList();

                if (studentGrid.Columns.Contains("KlasseId"))
                {
                    studentGrid.Columns["KlasseId"].Visible = false;
                }

                if (studentGrid.Columns.Contains("SchuelerId"))
                {
                    studentGrid.Columns["SchuelerId"].HeaderText = "ID";
                    studentGrid.Columns["SchuelerId"].Width = 60;
                }

                if (studentGrid.Columns.Contains("Name"))
                {
                    studentGrid.Columns["Name"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }

                if (studentGrid.Columns.Contains("Klasse"))
                {
                    studentGrid.Columns["Klasse"].Width = 130;
                }

                studentGrid.ClearSelection();
                AuswahlZuruecksetzen();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Die Schüler konnten nicht geladen werden.\n\n" + ex.Message);
            }
        }

        private void OKnewStudent_Click(object sender, EventArgs e)
        {
            if (newStudentKlasse.SelectedValue == null)
            {
                MessageBox.Show("Bitte eine Klasse auswählen.");
                return;
            }

            try
            {
                schuelerService.Erstellen(
                    newStudentName.Text,
                    Convert.ToInt32(newStudentKlasse.SelectedValue)
                );

                newStudentName.Text = "";
                SchuelerListeAktualisieren();
                MessageBox.Show("Schüler wurde gespeichert.");
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Der Schüler konnte nicht gespeichert werden.\n\n" + ex.Message);
            }
        }

        private void showStudents_Click(object sender, EventArgs e)
        {
            KlassenLaden();
            SchuelerListeAktualisieren();
        }

        private void studentGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (studentGrid.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = studentGrid.SelectedRows[0];
            if (row.Cells["SchuelerId"].Value == null)
            {
                return;
            }

            ausgewaehlteSchuelerId = Convert.ToInt32(row.Cells["SchuelerId"].Value);
            string name = Convert.ToString(row.Cells["Name"].Value) ?? "";
            int klasseId = Convert.ToInt32(row.Cells["KlasseId"].Value);

            oldName.Text = name;
            newName.Text = name;
            deleteNameBox.Text = name;
            editKlasse.SelectedValue = klasseId;
        }

        private void OKchangeName_Click(object sender, EventArgs e)
        {
            if (ausgewaehlteSchuelerId == null)
            {
                MessageBox.Show("Bitte zuerst einen Schüler in der Tabelle auswählen.");
                return;
            }

            if (editKlasse.SelectedValue == null)
            {
                MessageBox.Show("Bitte eine Klasse auswählen.");
                return;
            }

            try
            {
                bool bearbeitet = schuelerService.Bearbeiten(
                    ausgewaehlteSchuelerId.Value,
                    newName.Text,
                    Convert.ToInt32(editKlasse.SelectedValue)
                );

                if (!bearbeitet)
                {
                    MessageBox.Show("Der Schüler wurde nicht gefunden.");
                    SchuelerListeAktualisieren();
                    return;
                }

                SchuelerListeAktualisieren();
                MessageBox.Show("Schüler wurde bearbeitet.");
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Der Schüler konnte nicht bearbeitet werden.\n\n" + ex.Message);
            }
        }

        private void OKdeleteBtn_Click(object sender, EventArgs e)
        {
            if (ausgewaehlteSchuelerId == null)
            {
                MessageBox.Show("Bitte zuerst einen Schüler in der Tabelle auswählen.");
                return;
            }

            DialogResult antwort = MessageBox.Show(
                $"Soll {deleteNameBox.Text} wirklich gelöscht werden?",
                "Schüler löschen",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (antwort != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool geloescht = schuelerService.Loeschen(ausgewaehlteSchuelerId.Value);

                if (!geloescht)
                {
                    MessageBox.Show("Der Schüler wurde nicht gefunden.");
                }
                else
                {
                    MessageBox.Show("Schüler wurde gelöscht.");
                }

                SchuelerListeAktualisieren();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Der Schüler konnte nicht gelöscht werden.\n\n" + ex.Message);
            }
        }

        private void AuswahlZuruecksetzen()
        {
            ausgewaehlteSchuelerId = null;
            oldName.Text = "";
            newName.Text = "";
            deleteNameBox.Text = "";
        }

        private void newStudentName_TextChanged(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void back_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
