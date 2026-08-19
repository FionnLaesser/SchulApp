using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;

namespace SchulApp
{
    public partial class Kurse : CustomForm
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
                // Erstellt den Entity-Framework-Datenbankkontext
                using SchulAppContext context = new SchulAppContext();

                // Lädt alle Klassen ohne selbst geschriebenes SELECT
                var klassen = context.Klassen
                    .AsNoTracking()
                    .OrderBy(k => k.KlassenId)
                    .Select(k => new
                    {
                        k.KlassenId,
                        k.Bezeichnung
                    })
                    .ToList();

                // Lädt alle Lehrer ohne selbst geschriebenes SELECT
                var lehrer = context.Lehrer
                    .AsNoTracking()
                    .OrderBy(l => l.LehrerId)
                    .Select(l => new
                    {
                        l.LehrerId,
                        l.Name
                    })
                    .ToList();

                newKursKlasse.DisplayMember = "Bezeichnung";
                newKursKlasse.ValueMember = "KlassenId";
                newKursKlasse.DataSource = klassen;

                editKursKlasse.DisplayMember = "Bezeichnung";
                editKursKlasse.ValueMember = "KlassenId";

                // Eigene Liste für die zweite ComboBox
                editKursKlasse.DataSource = context.Klassen
                    .AsNoTracking()
                    .OrderBy(k => k.KlassenId)
                    .Select(k => new
                    {
                        k.KlassenId,
                        k.Bezeichnung
                    })
                    .ToList();

                newKursLehrer.DisplayMember = "Name";
                newKursLehrer.ValueMember = "LehrerId";
                newKursLehrer.DataSource = lehrer;

                editKursLehrer.DisplayMember = "Name";
                editKursLehrer.ValueMember = "LehrerId";

                // Eigene Liste für die zweite ComboBox
                editKursLehrer.DataSource = context.Lehrer
                    .AsNoTracking()
                    .OrderBy(l => l.LehrerId)
                    .Select(l => new
                    {
                        l.LehrerId,
                        l.Name
                    })
                    .ToList();

                bool kannSpeichern =
                    klassen.Count > 0 &&
                    lehrer.Count > 0;

                createKursBtn.Enabled = kannSpeichern;
                updateKursBtn.Enabled = kannSpeichern;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Klassen und Lehrer konnten nicht geladen werden.\n\n" +
                    ex.Message
                );
            }
        }

        private void KurseListeAktualisieren()
        {
            try
            {
                using SchulAppContext context = new SchulAppContext();

                // Lädt die Kurse inklusive Klassen- und Lehrerinformationen.
                // Die benötigten JOINs erstellt Entity Framework automatisch.
                var kurse = context.Kurse
                    .AsNoTracking()
                    .OrderBy(k => k.Name)
                    .ThenBy(k => k.KlasseId)
                    .Select(k => new
                    {
                        k.KursId,
                        k.Name,
                        k.KlasseId,

                        Klasse = k.Klasse.Bezeichnung,

                        k.LehrerId,

                        Lehrer = k.Lehrer.Name
                    })
                    .ToList();

                kurseGrid.DataSource = kurse;

                if (kurseGrid.Columns["KursId"] is DataGridViewColumn kursId)
                {
                    kursId.HeaderText = "ID";
                    kursId.Width = 50;
                }

                if (kurseGrid.Columns["KlasseId"] is DataGridViewColumn klasseId)
                {
                    klasseId.Visible = false;
                }

                if (kurseGrid.Columns["LehrerId"] is DataGridViewColumn lehrerId)
                {
                    lehrerId.Visible = false;
                }

                if (kurseGrid.Columns["Name"] is DataGridViewColumn name)
                {
                    name.AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;
                }

                if (kurseGrid.Columns["Klasse"] is DataGridViewColumn klasse)
                {
                    klasse.Width = 100;
                }

                if (kurseGrid.Columns["Lehrer"] is DataGridViewColumn lehrer)
                {
                    lehrer.Width = 180;
                }

                kurseGrid.ClearSelection();
                AuswahlZuruecksetzen();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Die Kurse konnten nicht geladen werden.\n\n" +
                    ex.Message
                );
            }
        }

        private bool NeueDatenPruefen(
            TextBox nameBox,
            ComboBox klasseBox,
            ComboBox lehrerBox,
            out string name,
            out int klasseId,
            out int lehrerId)
        {
            name = nameBox.Text.Trim();
            klasseId = 0;
            lehrerId = 0;

            if (name == "")
            {
                MessageBox.Show(
                    "Bitte einen Kursnamen eingeben."
                );

                return false;
            }

            if (klasseBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Bitte eine Klasse auswählen."
                );

                return false;
            }

            if (lehrerBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Bitte einen Lehrer auswählen."
                );

                return false;
            }

            klasseId =
                Convert.ToInt32(klasseBox.SelectedValue);

            lehrerId =
                Convert.ToInt32(lehrerBox.SelectedValue);

            return true;
        }

        private void createKursBtn_Click(
            object sender,
            EventArgs e)
        {
            if (!NeueDatenPruefen(
                newKursName,
                newKursKlasse,
                newKursLehrer,
                out string name,
                out int klasseId,
                out int lehrerId))
            {
                return;
            }

            try
            {
                using SchulAppContext context =
                    new SchulAppContext();

                // Erstellt einen neuen Kurs als C#-Objekt
                KursModel neuerKurs = new KursModel
                {
                    Name = name,
                    KlasseId = klasseId,
                    LehrerId = lehrerId
                };

                // Markiert den Kurs zum Einfügen
                context.Kurse.Add(neuerKurs);

                // Entity Framework erstellt das INSERT automatisch
                context.SaveChanges();

                newKursName.Text = "";

                KurseListeAktualisieren();

                MessageBox.Show(
                    "Kurs wurde gespeichert."
                );
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(
                    "Der Kurs konnte nicht gespeichert werden.\n\n" +
                    ex.Message
                );
            }
        }

        private void kurseGrid_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (kurseGrid.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row =
                kurseGrid.SelectedRows[0];

            if (row.Cells["KursId"].Value == null)
            {
                return;
            }

            ausgewaehlteKursId =
                Convert.ToInt32(
                    row.Cells["KursId"].Value
                );

            string name =
                Convert.ToString(
                    row.Cells["Name"].Value
                ) ?? "";

            editKursName.Text = name;
            deleteKursName.Text = name;

            editKursKlasse.SelectedValue =
                Convert.ToInt32(
                    row.Cells["KlasseId"].Value
                );

            editKursLehrer.SelectedValue =
                Convert.ToInt32(
                    row.Cells["LehrerId"].Value
                );
        }

        private void updateKursBtn_Click(
            object sender,
            EventArgs e)
        {
            if (ausgewaehlteKursId == null)
            {
                MessageBox.Show(
                    "Bitte zuerst einen Kurs auswählen."
                );

                return;
            }

            if (!NeueDatenPruefen(
                editKursName,
                editKursKlasse,
                editKursLehrer,
                out string name,
                out int klasseId,
                out int lehrerId))
            {
                return;
            }

            try
            {
                using SchulAppContext context =
                    new SchulAppContext();

                // Sucht den Kurs anhand seiner ID
                KursModel? kurs =
                    context.Kurse.Find(
                        ausgewaehlteKursId.Value
                    );

                if (kurs == null)
                {
                    MessageBox.Show(
                        "Der Kurs wurde nicht gefunden."
                    );

                    return;
                }

                // Ändert die Eigenschaften am geladenen Objekt
                kurs.Name = name;
                kurs.KlasseId = klasseId;
                kurs.LehrerId = lehrerId;

                // Entity Framework erkennt die Änderungen
                // und erstellt das UPDATE automatisch
                context.SaveChanges();

                MessageBox.Show(
                    "Kurs wurde bearbeitet."
                );

                KurseListeAktualisieren();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(
                    "Der Kurs konnte nicht bearbeitet werden.\n\n" +
                    ex.Message
                );
            }
        }

        private void deleteKursBtn_Click(
            object sender,
            EventArgs e)
        {
            if (ausgewaehlteKursId == null)
            {
                MessageBox.Show(
                    "Bitte zuerst einen Kurs auswählen."
                );

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
                using SchulAppContext context =
                    new SchulAppContext();

                // Sucht den Kurs in der Datenbank
                KursModel? kurs =
                    context.Kurse.Find(
                        ausgewaehlteKursId.Value
                    );

                if (kurs == null)
                {
                    MessageBox.Show(
                        "Der Kurs wurde nicht gefunden."
                    );

                    return;
                }

                // Markiert den Kurs zum Löschen
                context.Kurse.Remove(kurs);

                // Entity Framework erstellt das DELETE automatisch
                context.SaveChanges();

                KurseListeAktualisieren();

                MessageBox.Show(
                    "Kurs wurde gelöscht."
                );
            }
            catch (DbUpdateException)
            {
                // Kann auftreten, wenn der Kurs noch
                // von einem Stundenplaneintrag verwendet wird
                MessageBox.Show(
                    "Dieser Kurs kann noch nicht gelöscht werden, weil er im Stundenplan verwendet wird.\n\n" +
                    "Lösche oder ändere zuerst die entsprechenden Stundenplaneinträge."
                );
            }
        }

        private void reloadBtn_Click(
            object sender,
            EventArgs e)
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

        private void back_Click(
            object sender,
            EventArgs e)
        {
            Close();
        }
    }
}