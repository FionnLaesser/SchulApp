using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;

namespace SchulApp
{
    public partial class Stundenplan : Form
    {
        private int? ausgewaehlteStundenplanId;

        // Hilfsklasse für die Wochentag-ComboBoxen
        private class WochentagAuswahl
        {
            public int Wert { get; set; }

            public string Text { get; set; } = "";
        }

        public Stundenplan()
        {
            InitializeComponent();

            WochentageEinrichten();
            KurseLaden();

            ThemeManager.Anwenden(this);

            StundenplanLaden();
            DatumUndUhrzeitLabel();
        }

        private void WochentageEinrichten()
        {
            // Wochentage werden jetzt als normale C#-Liste erstellt.
            // Dadurch wird keine DataTable mehr benötigt.
            List<WochentagAuswahl> tage = new List<WochentagAuswahl>
            {
                new WochentagAuswahl
                {
                    Wert = 1,
                    Text = "Montag"
                },
                new WochentagAuswahl
                {
                    Wert = 2,
                    Text = "Dienstag"
                },
                new WochentagAuswahl
                {
                    Wert = 3,
                    Text = "Mittwoch"
                },
                new WochentagAuswahl
                {
                    Wert = 4,
                    Text = "Donnerstag"
                },
                new WochentagAuswahl
                {
                    Wert = 5,
                    Text = "Freitag"
                }
            };

            newWochentag.DisplayMember = "Text";
            newWochentag.ValueMember = "Wert";

            newWochentag.DataSource =
                new List<WochentagAuswahl>(tage);

            editWochentag.DisplayMember = "Text";
            editWochentag.ValueMember = "Wert";

            editWochentag.DataSource =
                new List<WochentagAuswahl>(tage);
        }

        private void KurseLaden()
        {
            try
            {
                // Erstellt einen Entity-Framework-Datenbankkontext
                using SchulAppContext context =
                    new SchulAppContext();

                // Lädt Kurse zusammen mit Klasse und Lehrer.
                // Die benötigten JOINs erstellt Entity Framework selbst.
                var kurse = context.Kurse
                    .AsNoTracking()
                    .OrderBy(k => k.Name)
                    .ThenBy(k => k.Klasse.Bezeichnung)
                    .Select(k => new
                    {
                        k.KursId,

                        Anzeige =
                            k.Name +
                            " | " +
                            k.Klasse.Bezeichnung +
                            " | " +
                            k.Lehrer.Name
                    })
                    .ToList();

                newKurs.DisplayMember = "Anzeige";
                newKurs.ValueMember = "KursId";
                newKurs.DataSource = kurse;

                editKurs.DisplayMember = "Anzeige";
                editKurs.ValueMember = "KursId";

                // Zweite eigene Liste für die Bearbeiten-ComboBox
                editKurs.DataSource = context.Kurse
                    .AsNoTracking()
                    .OrderBy(k => k.Name)
                    .ThenBy(k => k.Klasse.Bezeichnung)
                    .Select(k => new
                    {
                        k.KursId,

                        Anzeige =
                            k.Name +
                            " | " +
                            k.Klasse.Bezeichnung +
                            " | " +
                            k.Lehrer.Name
                    })
                    .ToList();

                bool vorhanden = kurse.Count > 0;

                createEintragBtn.Enabled = vorhanden;
                updateEintragBtn.Enabled = vorhanden;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Die Kurse konnten nicht geladen werden.\n\n" +
                    ex.Message
                );
            }
        }

        private void StundenplanLaden()
        {
            try
            {
                using SchulAppContext context =
                    new SchulAppContext();

                // Lädt alle Stundenplaneinträge.
                // Kurs, Klasse und Lehrer werden über die
                // Beziehungen der Models abgefragt.
                var plan = context.Stundenplan
                    .AsNoTracking()
                    .OrderBy(s => s.Wochentag)
                    .ThenBy(s => s.Startzeit)
                    .Select(s => new
                    {
                        s.StundenplanId,
                        s.KursId,

                        WochentagNr = s.Wochentag,

                        Wochentag =
                            s.Wochentag == 1 ? "Montag" :
                            s.Wochentag == 2 ? "Dienstag" :
                            s.Wochentag == 3 ? "Mittwoch" :
                            s.Wochentag == 4 ? "Donnerstag" :
                            s.Wochentag == 5 ? "Freitag" :
                            "",

                        s.Startzeit,
                        s.Endzeit,

                        Kurs = s.Kurs.Name,

                        Klasse =
                            s.Kurs.Klasse.Bezeichnung,

                        Lehrer =
                            s.Kurs.Lehrer.Name,

                        s.Raum
                    })
                    .ToList();

                stundenplanGrid.DataSource = plan;

                if (stundenplanGrid.Columns.Contains("StundenplanId"))
                {
                    stundenplanGrid.Columns["StundenplanId"].HeaderText =
                        "ID";

                    stundenplanGrid.Columns["StundenplanId"].Width =
                        45;
                }

                if (stundenplanGrid.Columns.Contains("KursId"))
                {
                    stundenplanGrid.Columns["KursId"].Visible =
                        false;
                }

                if (stundenplanGrid.Columns.Contains("WochentagNr"))
                {
                    stundenplanGrid.Columns["WochentagNr"].Visible =
                        false;
                }

                if (stundenplanGrid.Columns.Contains("Wochentag"))
                {
                    stundenplanGrid.Columns["Wochentag"].Width =
                        90;
                }

                if (stundenplanGrid.Columns.Contains("Startzeit"))
                {
                    stundenplanGrid.Columns["Startzeit"].HeaderText =
                        "Von";

                    stundenplanGrid.Columns["Startzeit"]
                        .DefaultCellStyle.Format = @"hh\:mm";

                    stundenplanGrid.Columns["Startzeit"].Width =
                        60;
                }

                if (stundenplanGrid.Columns.Contains("Endzeit"))
                {
                    stundenplanGrid.Columns["Endzeit"].HeaderText =
                        "Bis";

                    stundenplanGrid.Columns["Endzeit"]
                        .DefaultCellStyle.Format = @"hh\:mm";

                    stundenplanGrid.Columns["Endzeit"].Width =
                        60;
                }

                if (stundenplanGrid.Columns.Contains("Kurs"))
                {
                    stundenplanGrid.Columns["Kurs"].AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;
                }

                if (stundenplanGrid.Columns.Contains("Klasse"))
                {
                    stundenplanGrid.Columns["Klasse"].Width =
                        75;
                }

                if (stundenplanGrid.Columns.Contains("Lehrer"))
                {
                    stundenplanGrid.Columns["Lehrer"].Width =
                        140;
                }

                if (stundenplanGrid.Columns.Contains("Raum"))
                {
                    stundenplanGrid.Columns["Raum"].Width =
                        60;
                }

                stundenplanGrid.ClearSelection();

                AuswahlZuruecksetzen();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Der Stundenplan konnte nicht geladen werden.\n\n" +
                    ex.Message
                );
            }
        }

        private static TimeSpan ZeitAusPicker(
            DateTimePicker picker)
        {
            return new TimeSpan(
                picker.Value.Hour,
                picker.Value.Minute,
                0
            );
        }

        private bool EingabePruefen(
            ComboBox kursBox,
            ComboBox tagBox,
            DateTimePicker startPicker,
            DateTimePicker endePicker,
            out int kursId,
            out int wochentag,
            out TimeSpan start,
            out TimeSpan ende)
        {
            kursId = 0;
            wochentag = 0;

            start = ZeitAusPicker(startPicker);
            ende = ZeitAusPicker(endePicker);

            if (kursBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Bitte einen Kurs auswählen."
                );

                return false;
            }

            if (tagBox.SelectedValue == null)
            {
                MessageBox.Show(
                    "Bitte einen Wochentag auswählen."
                );

                return false;
            }

            if (ende <= start)
            {
                MessageBox.Show(
                    "Die Endzeit muss nach der Startzeit liegen."
                );

                return false;
            }

            kursId =
                Convert.ToInt32(kursBox.SelectedValue);

            wochentag =
                Convert.ToInt32(tagBox.SelectedValue);

            return true;
        }

        private void createEintragBtn_Click(
            object sender,
            EventArgs e)
        {
            if (!EingabePruefen(
                newKurs,
                newWochentag,
                newStartzeit,
                newEndzeit,
                out int kursId,
                out int tag,
                out TimeSpan start,
                out TimeSpan ende))
            {
                return;
            }

            try
            {
                using SchulAppContext context =
                    new SchulAppContext();

                // Erstellt einen neuen Stundenplaneintrag
                // als normales C#-Objekt.
                StundenplanModel neuerEintrag =
                    new StundenplanModel
                    {
                        KursId = kursId,

                        Wochentag =
                            Convert.ToByte(tag),

                        Startzeit = start,

                        Endzeit = ende,

                        // null wird gespeichert,
                        // wenn kein Raum eingegeben wurde.
                        Raum =
                            string.IsNullOrWhiteSpace(newRaum.Text)
                                ? null
                                : newRaum.Text.Trim()
                    };

                // Markiert den neuen Eintrag zum Einfügen
                context.Stundenplan.Add(neuerEintrag);

                // Entity Framework erstellt das INSERT automatisch
                context.SaveChanges();

                newRaum.Text = "";

                StundenplanLaden();

                MessageBox.Show(
                    "Stundenplaneintrag wurde gespeichert."
                );
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(
                    "Der Stundenplaneintrag konnte nicht gespeichert werden.\n\n" +
                    ex.Message
                );
            }
        }

        private void stundenplanGrid_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (stundenplanGrid.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row =
                stundenplanGrid.SelectedRows[0];

            if (row.Cells["StundenplanId"].Value == null)
            {
                return;
            }

            ausgewaehlteStundenplanId =
                Convert.ToInt32(
                    row.Cells["StundenplanId"].Value
                );

            editKurs.SelectedValue =
                Convert.ToInt32(
                    row.Cells["KursId"].Value
                );

            editWochentag.SelectedValue =
                Convert.ToInt32(
                    row.Cells["WochentagNr"].Value
                );

            TimeSpan start =
                (TimeSpan)row.Cells["Startzeit"].Value;

            TimeSpan ende =
                (TimeSpan)row.Cells["Endzeit"].Value;

            editStartzeit.Value =
                DateTime.Today.Add(start);

            editEndzeit.Value =
                DateTime.Today.Add(ende);

            // Bei Entity Framework ist ein leerer Datenbankwert null
            // und nicht mehr DBNull.Value.
            editRaum.Text =
                Convert.ToString(
                    row.Cells["Raum"].Value
                ) ?? "";

            deleteInfo.Text =
                $"{row.Cells["Wochentag"].Value}, " +
                $"{start:hh\\:mm} - " +
                $"{row.Cells["Kurs"].Value}";
        }

        private void updateEintragBtn_Click(
            object sender,
            EventArgs e)
        {
            if (ausgewaehlteStundenplanId == null)
            {
                MessageBox.Show(
                    "Bitte zuerst einen Stundenplaneintrag auswählen."
                );

                return;
            }

            if (!EingabePruefen(
                editKurs,
                editWochentag,
                editStartzeit,
                editEndzeit,
                out int kursId,
                out int tag,
                out TimeSpan start,
                out TimeSpan ende))
            {
                return;
            }

            try
            {
                using SchulAppContext context =
                    new SchulAppContext();

                // Sucht den Stundenplaneintrag anhand der ID.
                StundenplanModel? eintrag =
                    context.Stundenplan.Find(
                        ausgewaehlteStundenplanId.Value
                    );

                if (eintrag == null)
                {
                    MessageBox.Show(
                        "Der Eintrag wurde nicht gefunden."
                    );

                    return;
                }

                // Ändert die Eigenschaften des geladenen Objekts.
                eintrag.KursId = kursId;

                eintrag.Wochentag =
                    Convert.ToByte(tag);

                eintrag.Startzeit = start;
                eintrag.Endzeit = ende;

                eintrag.Raum =
                    string.IsNullOrWhiteSpace(editRaum.Text)
                        ? null
                        : editRaum.Text.Trim();

                // Entity Framework erkennt die Änderungen
                // und erstellt das UPDATE automatisch.
                context.SaveChanges();

                MessageBox.Show(
                    "Stundenplaneintrag wurde bearbeitet."
                );

                StundenplanLaden();
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(
                    "Der Stundenplaneintrag konnte nicht bearbeitet werden.\n\n" +
                    ex.Message
                );
            }
        }

        private void deleteEintragBtn_Click(
            object sender,
            EventArgs e)
        {
            if (ausgewaehlteStundenplanId == null)
            {
                MessageBox.Show(
                    "Bitte zuerst einen Stundenplaneintrag auswählen."
                );

                return;
            }

            DialogResult antwort =
                MessageBox.Show(
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
                using SchulAppContext context =
                    new SchulAppContext();

                // Sucht den Eintrag anhand seiner ID.
                StundenplanModel? eintrag =
                    context.Stundenplan.Find(
                        ausgewaehlteStundenplanId.Value
                    );

                if (eintrag == null)
                {
                    MessageBox.Show(
                        "Der Eintrag wurde nicht gefunden."
                    );

                    return;
                }

                // Markiert den Eintrag zum Löschen.
                context.Stundenplan.Remove(eintrag);

                // Entity Framework erstellt das DELETE automatisch.
                context.SaveChanges();

                StundenplanLaden();

                MessageBox.Show(
                    "Stundenplaneintrag wurde gelöscht."
                );
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(
                    "Der Stundenplaneintrag konnte nicht gelöscht werden.\n\n" +
                    ex.Message
                );
            }
        }

        private void reloadBtn_Click(
            object sender,
            EventArgs e)
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

        private void back_Click(
            object sender,
            EventArgs e)
        {
            Close();
        }

        private async void DatumUndUhrzeitLabel()
        {
            while (true)
            {
                DateTime jetzt = DateTime.Now;

                string datum =
                    jetzt.ToString("dddd, dd.MM.yyyy");

                string uhrzeit =
                    jetzt.ToString("HH:mm:ss");

                this.datum.Text =
                    $"Datum: {datum}";

                this.uhrzeit.Text =
                    $"Uhrzeit: {uhrzeit}";

                await Task.Delay(1000);
            }
        }
    }
}