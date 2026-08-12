using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;

namespace SchulApp
{
    public partial class Klassen : CustomForm
    {
        private int? ausgewaehlteKlassenId;

        // Hilfsklasse für die Klassenlehrer-ComboBox.
        // LehrerId darf null sein für "(Kein Klassenlehrer)".
        private class LehrerAuswahl
        {
            public int? LehrerId { get; set; }

            public string Name { get; set; } = "";
        }

        public Klassen()
        {
            InitializeComponent();
            LehrerLaden();
            ThemeManager.Anwenden(this);
            KlassenListeAktualisieren();
        }

        private void LehrerLaden()
        {
            try
            {
                // Erstellt den Entity-Framework-Datenbankkontext.
                using SchulAppContext context = new SchulAppContext();

                // Lädt alle Lehrer aus der Datenbank.
                // Entity Framework erstellt die SELECT-Abfrage automatisch.
                List<LehrerAuswahl> lehrer = context.Lehrer
                    .AsNoTracking()
                    .OrderBy(l => l.Name)
                    .Select(l => new LehrerAuswahl
                    {
                        LehrerId = l.LehrerId,
                        Name = l.Name
                    })
                    .ToList();

                // Fügt die Auswahl für eine Klasse ohne Klassenlehrer hinzu.
                lehrer.Insert(0, new LehrerAuswahl
                {
                    LehrerId = null,
                    Name = "(Kein Klassenlehrer)"
                });

                newKlassenlehrer.DisplayMember = "Name";
                newKlassenlehrer.ValueMember = "LehrerId";

                // Eigene Liste, damit die beiden ComboBoxen unabhängig sind.
                newKlassenlehrer.DataSource =
                    new List<LehrerAuswahl>(lehrer);

                editKlassenlehrer.DisplayMember = "Name";
                editKlassenlehrer.ValueMember = "LehrerId";

                editKlassenlehrer.DataSource =
                    new List<LehrerAuswahl>(lehrer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Die Lehrer konnten nicht geladen werden.\n\n" +
                    ex.Message
                );
            }
        }

        private void KlassenListeAktualisieren()
        {
            try
            {
                using SchulAppContext context = new SchulAppContext();

                // Lädt Klassen, Klassenlehrer und Anzahl Schüler.
                // JOIN, COUNT und SELECT werden von Entity Framework erzeugt.
                var klassen = context.Klassen
                    .AsNoTracking() //Nur lesen, speichersparsam und schneller.
                    .OrderBy(k => k.KlassenId)
                    .Select(k => new
                    {
                        k.KlassenId,
                        k.Bezeichnung,
                        k.KlassenlehrerId,

                        Klassenlehrer =
                            k.Klassenlehrer != null
                                ? k.Klassenlehrer.Name
                                : "(Kein Klassenlehrer)",

                        AnzahlSchueler = k.Schueler.Count
                    })
                    .ToList();

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
                    klassenGrid.Columns["Klassenlehrer"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;
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
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Die Klassen konnten nicht geladen werden.\n\n" +
                    ex.Message
                );
            }
        }

        // Holt die LehrerId aus der ComboBox.
        // Gibt null zurück, wenn "(Kein Klassenlehrer)" ausgewählt ist.
        private static int? LehrerIdAusComboBox(ComboBox comboBox)
        {
            if (comboBox.SelectedValue == null)
            {
                return null;
            }

            return Convert.ToInt32(comboBox.SelectedValue);
        }

        private void createKlasseBtn_Click(object sender, EventArgs e)
        {
            string bezeichnung = newKlasseName.Text.Trim();

            if (bezeichnung == "")
            {
                MessageBox.Show(
                    "Bitte eine Klassenbezeichnung eingeben."
                );

                return;
            }

            try
            {
                using SchulAppContext context = new SchulAppContext();

                // Prüft ohne selbst geschriebenes SQL,
                // ob die Klassenbezeichnung bereits existiert.
                bool existiertBereits = context.Klassen
                    .Any(k => k.Bezeichnung == bezeichnung);

                if (existiertBereits)
                {
                    MessageBox.Show(
                        "Diese Klassenbezeichnung existiert bereits."
                    );

                    return;
                }

                // Erstellt ein neues Klassenobjekt.
                KlasseModel neueKlasse = new KlasseModel
                {
                    Bezeichnung = bezeichnung,

                    KlassenlehrerId =
                        LehrerIdAusComboBox(newKlassenlehrer)
                };

                // Markiert die neue Klasse zum Einfügen.
                context.Klassen.Add(neueKlasse);

                // Entity Framework erstellt das INSERT automatisch.
                context.SaveChanges();

                newKlasseName.Text = "";
                newKlassenlehrer.SelectedIndex = 0;

                KlassenListeAktualisieren();

                MessageBox.Show(
                    "Klasse wurde gespeichert."
                );
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(
                    "Die Klasse konnte nicht gespeichert werden.\n\n" +
                    ex.Message
                );
            }
        }

        private void klassenGrid_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (klassenGrid.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row =
                klassenGrid.SelectedRows[0];

            if (row.Cells["KlassenId"].Value == null)
            {
                return;
            }

            ausgewaehlteKlassenId =
                Convert.ToInt32(
                    row.Cells["KlassenId"].Value
                );

            editKlasseName.Text =
                Convert.ToString(
                    row.Cells["Bezeichnung"].Value
                ) ?? "";

            deleteKlasseName.Text =
                editKlasseName.Text;

            object? lehrerId =
                row.Cells["KlassenlehrerId"].Value;

            if (lehrerId == null ||
                lehrerId == DBNull.Value)
            {
                editKlassenlehrer.SelectedIndex = 0;
            }
            else
            {
                editKlassenlehrer.SelectedValue =
                    Convert.ToInt32(lehrerId);
            }

            SchuelerDerKlasseLaden();
        }

        private void updateKlasseBtn_Click(
            object sender,
            EventArgs e)
        {
            if (ausgewaehlteKlassenId == null)
            {
                MessageBox.Show(
                    "Bitte zuerst eine Klasse auswählen."
                );

                return;
            }

            string bezeichnung =
                editKlasseName.Text.Trim();

            if (bezeichnung == "")
            {
                MessageBox.Show(
                    "Bitte eine Klassenbezeichnung eingeben."
                );

                return;
            }

            try
            {
                using SchulAppContext context =
                    new SchulAppContext();

                int klassenId =
                    ausgewaehlteKlassenId.Value;

                // Prüft, ob eine andere Klasse
                // bereits dieselbe Bezeichnung hat.
                bool existiertBereits = context.Klassen
                    .Any(k =>
                        k.Bezeichnung == bezeichnung &&
                        k.KlassenId != klassenId
                    );

                if (existiertBereits)
                {
                    MessageBox.Show(
                        "Diese Klassenbezeichnung existiert bereits."
                    );

                    return;
                }

                // Sucht die Klasse anhand des Primärschlüssels.
                // Entity Framework erstellt die SELECT-Abfrage selbst.
                KlasseModel? klasse =
                    context.Klassen.Find(klassenId);

                if (klasse == null)
                {
                    MessageBox.Show(
                        "Die Klasse wurde nicht gefunden."
                    );

                    return;
                }

                // Ändert die Werte am geladenen Objekt.
                klasse.Bezeichnung = bezeichnung;

                klasse.KlassenlehrerId =
                    LehrerIdAusComboBox(
                        editKlassenlehrer
                    );

                // Erkennt die Änderungen und erstellt
                // automatisch das UPDATE.
                context.SaveChanges();

                MessageBox.Show(
                    "Klasse wurde bearbeitet."
                );

                KlassenListeAktualisieren();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(
                    "Die Klasse konnte nicht bearbeitet werden.\n\n" +
                    ex.Message
                );
            }
        }

        private void deleteKlasseBtn_Click(
            object sender,
            EventArgs e)
        {
            if (ausgewaehlteKlassenId == null)
            {
                MessageBox.Show(
                    "Bitte zuerst eine Klasse auswählen."
                );

                return;
            }

            DialogResult antwort =
                MessageBox.Show(
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
                using SchulAppContext context =
                    new SchulAppContext();

                // Sucht zuerst die zu löschende Klasse.
                KlasseModel? klasse =
                    context.Klassen.Find(
                        ausgewaehlteKlassenId.Value
                    );

                if (klasse == null)
                {
                    MessageBox.Show(
                        "Die Klasse wurde nicht gefunden."
                    );

                    return;
                }

                // Markiert die Klasse zum Löschen.
                context.Klassen.Remove(klasse);

                // Entity Framework erstellt das DELETE automatisch.
                context.SaveChanges();

                KlassenListeAktualisieren();

                MessageBox.Show(
                    "Klasse wurde gelöscht."
                );
            }
            catch (DbUpdateException)
            {
                // Dieser Fehler tritt zum Beispiel auf,
                // wenn noch Schüler oder Kurse mit der Klasse verbunden sind.
                MessageBox.Show(
                    "Diese Klasse kann noch nicht gelöscht werden, weil ihr Schüler oder Kurse zugeordnet sind.\n\n" +
                    "Ordne die Schüler zuerst einer anderen Klasse zu und entferne oder ändere die Kurse."
                );
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
                using SchulAppContext context =
                    new SchulAppContext();

                int klassenId =
                    ausgewaehlteKlassenId.Value;

                // Lädt nur die Schüler der ausgewählten Klasse.
                // WHERE und ORDER BY werden automatisch als SQL erzeugt.
                var schueler = context.Schueler
                    .AsNoTracking()
                    .Where(s => s.KlasseId == klassenId)
                    .OrderBy(s => s.SchuelerId)
                    .Select(s => new
                    {
                        s.SchuelerId,
                        s.Name
                    })
                    .ToList();

                klassenSchuelerGrid.DataSource =
                    schueler;

                if (klassenSchuelerGrid.Columns.Contains("SchuelerId"))
                {
                    klassenSchuelerGrid.Columns["SchuelerId"].HeaderText =
                        "ID";

                    klassenSchuelerGrid.Columns["SchuelerId"].Width =
                        55;
                }

                if (klassenSchuelerGrid.Columns.Contains("Name"))
                {
                    klassenSchuelerGrid.Columns["Name"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Die Schüler der Klasse konnten nicht geladen werden.\n\n" +
                    ex.Message
                );
            }
        }

        private void reloadBtn_Click(
            object sender,
            EventArgs e)
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

        private void back_Click(
            object sender,
            EventArgs e)
        {
            Close();
        }
    }
}