using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Services;

namespace SchulApp
{
    public partial class Schueler : Form
    {
        private readonly SchuelerService schuelerService;
        private readonly bool nurHinzufuegen;
        private int? ausgewaehlteSchuelerId;

        public Schueler(bool nurHinzufuegen = false)
        {
            this.nurHinzufuegen = nurHinzufuegen;

            InitializeComponent();

            // Das Repository verwendet jetzt intern Entity Framework
            schuelerService = new SchuelerService(
                new SqlSchuelerRepository()
            );

            ThemeManager.Anwenden(this);
            RechteAnwenden();

            KlassenLaden();
            SchuelerListeAktualisieren();
        }

        private void KlassenLaden()
        {
            try
            {
                // Erstellt den Entity-Framework-Datenbankkontext
                using SchulAppContext context = new SchulAppContext();

                // Lädt alle Klassen aus der Datenbank.
                // Entity Framework erstellt SELECT und ORDER BY automatisch.
                var klassen = context.Klassen
                    .AsNoTracking()
                    .OrderBy(k => k.KlassenId)
                    .Select(k => new
                    {
                        k.KlassenId,
                        k.Bezeichnung
                    })
                    .ToList();

                newStudentKlasse.DisplayMember = "Bezeichnung";
                newStudentKlasse.ValueMember = "KlassenId";

                // Eigene Liste für die ComboBox
                newStudentKlasse.DataSource = klassen.ToList();

                editKlasse.DisplayMember = "Bezeichnung";
                editKlasse.ValueMember = "KlassenId";

                // Eigene Liste für die zweite ComboBox
                editKlasse.DataSource = klassen.ToList();

                // Schüler können nur erstellt oder bearbeitet werden,
                // wenn mindestens eine Klasse vorhanden ist
                bool vorhanden = klassen.Count > 0;

                OKnewStudent.Enabled = vorhanden;
                OKchangeName.Enabled = vorhanden && !nurHinzufuegen;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Die Klassen konnten nicht geladen werden.\n\n" +
                    ex.Message
                );
            }
        }

        private void SchuelerListeAktualisieren()
        {
            try
            {
                // Der Service lädt die Schüler über das EF-Repository.
                // Die Klasse wird hier in einen String umgewandelt,
                // damit im DataGridView die Klassenbezeichnung angezeigt wird.
                var schueler = schuelerService
                    .AlleLaden()
                    .OrderBy(s => s.SchuelerId)
                    .Select(s => new
                    {
                        s.SchuelerId,
                        s.Name,
                        s.KlasseId,

                        Klasse = s.Klasse?.Bezeichnung ?? ""
                    })
                    .ToList();

                studentGrid.DataSource = schueler;

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
                    studentGrid.Columns["Name"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;
                }

                if (studentGrid.Columns.Contains("Klasse"))
                {
                    studentGrid.Columns["Klasse"].Width = 130;
                }

                studentGrid.ClearSelection();
                AuswahlZuruecksetzen();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Die Schüler konnten nicht geladen werden.\n\n" +
                    ex.Message
                );
            }
        }

        private void OKnewStudent_Click(
            object sender,
            EventArgs e)
        {
            if (newStudentKlasse.SelectedValue == null)
            {
                MessageBox.Show(
                    "Bitte eine Klasse auswählen."
                );

                return;
            }

            try
            {
                // Der Service erstellt den Schüler.
                // Das Repository verwendet dafür Entity Framework.
                schuelerService.Erstellen(
                    newStudentName.Text,
                    Convert.ToInt32(
                        newStudentKlasse.SelectedValue
                    )
                );

                newStudentName.Text = "";

                SchuelerListeAktualisieren();

                MessageBox.Show(
                    "Schüler wurde gespeichert."
                );
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                // Fehler beim Speichern mit Entity Framework
                MessageBox.Show(
                    "Der Schüler konnte nicht gespeichert werden.\n\n" +
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Der Schüler konnte nicht gespeichert werden.\n\n" +
                    ex.Message
                );
            }
        }

        private void showStudents_Click(
            object sender,
            EventArgs e)
        {
            KlassenLaden();
            SchuelerListeAktualisieren();
        }

        private void studentGrid_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (studentGrid.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row =
                studentGrid.SelectedRows[0];

            if (row.Cells["SchuelerId"].Value == null)
            {
                return;
            }

            ausgewaehlteSchuelerId =
                Convert.ToInt32(
                    row.Cells["SchuelerId"].Value
                );

            string name =
                Convert.ToString(
                    row.Cells["Name"].Value
                ) ?? "";

            int klasseId =
                Convert.ToInt32(
                    row.Cells["KlasseId"].Value
                );

            oldName.Text = name;
            newName.Text = name;
            deleteNameBox.Text = name;

            editKlasse.SelectedValue = klasseId;
        }

        private void OKchangeName_Click(
            object sender,
            EventArgs e)
        {
            if (nurHinzufuegen)
            {
                return;
            }

            if (ausgewaehlteSchuelerId == null)
            {
                MessageBox.Show(
                    "Bitte zuerst einen Schüler in der Tabelle auswählen."
                );

                return;
            }

            if (editKlasse.SelectedValue == null)
            {
                MessageBox.Show(
                    "Bitte eine Klasse auswählen."
                );

                return;
            }

            try
            {
                // Der Service bearbeitet den Schüler.
                // Das Repository verwendet Find() und SaveChanges().
                bool bearbeitet =
                    schuelerService.Bearbeiten(
                        ausgewaehlteSchuelerId.Value,
                        newName.Text,
                        Convert.ToInt32(
                            editKlasse.SelectedValue
                        )
                    );

                if (!bearbeitet)
                {
                    MessageBox.Show(
                        "Der Schüler wurde nicht gefunden."
                    );

                    SchuelerListeAktualisieren();

                    return;
                }

                SchuelerListeAktualisieren();

                MessageBox.Show(
                    "Schüler wurde bearbeitet."
                );
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                // Fehler beim UPDATE über Entity Framework
                MessageBox.Show(
                    "Der Schüler konnte nicht bearbeitet werden.\n\n" +
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Der Schüler konnte nicht bearbeitet werden.\n\n" +
                    ex.Message
                );
            }
        }

        private void OKdeleteBtn_Click(
            object sender,
            EventArgs e)
        {
            if (nurHinzufuegen)
            {
                return;
            }

            if (ausgewaehlteSchuelerId == null)
            {
                MessageBox.Show(
                    "Bitte zuerst einen Schüler in der Tabelle auswählen."
                );

                return;
            }

            DialogResult antwort =
                MessageBox.Show(
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
                // Der Service löscht den Schüler.
                // Das Repository verwendet Remove() und SaveChanges().
                bool geloescht =
                    schuelerService.Loeschen(
                        ausgewaehlteSchuelerId.Value
                    );

                if (!geloescht)
                {
                    MessageBox.Show(
                        "Der Schüler wurde nicht gefunden."
                    );
                }
                else
                {
                    MessageBox.Show(
                        "Schüler wurde gelöscht."
                    );
                }

                SchuelerListeAktualisieren();
            }
            catch (DbUpdateException ex)
            {
                // Fehler beim DELETE über Entity Framework
                MessageBox.Show(
                    "Der Schüler konnte nicht gelöscht werden.\n\n" +
                    ex.Message
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Der Schüler konnte nicht gelöscht werden.\n\n" +
                    ex.Message
                );
            }
        }


        private void RechteAnwenden()
        {
            if (!nurHinzufuegen)
            {
                return;
            }

            label4.Visible = false;
            label2.Visible = false;
            oldName.Visible = false;
            label3.Visible = false;
            newName.Visible = false;
            labelKlasseBearbeiten.Visible = false;
            editKlasse.Visible = false;
            OKchangeName.Visible = false;

            label6.Visible = false;
            label7.Visible = false;
            deleteNameBox.Visible = false;
            OKdeleteBtn.Visible = false;
        }

        private void AuswahlZuruecksetzen()
        {
            ausgewaehlteSchuelerId = null;

            oldName.Text = "";
            newName.Text = "";
            deleteNameBox.Text = "";
        }

        private void newStudentName_TextChanged(
            object sender,
            EventArgs e)
        {
        }

        private void label1_Click(
            object sender,
            EventArgs e)
        {
        }

        private void back_Click(
            object sender,
            EventArgs e)
        {
            Close();
        }
    }
}