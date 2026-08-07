using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace SchulApp
{
    public partial class Lehrer : Form
    {
        private readonly string dateiPfad =
            Path.Combine(Application.StartupPath, "lehrer.json");

        private List<LehrerDaten> lehrerListe = new List<LehrerDaten>();

        public Lehrer()
        {
            InitializeComponent();

            // Buttons mit Methoden verbinden
            OKnewTeacherBtn.Click += OKnewTeacherBtn_Click;
            OKchangeNameBtn.Click += OKchangeNameBtn_Click;
            OKdeleteBtn.Click += OKdeleteBtn_Click;
            OKchangingInfo.Click += OKchangingInfo_Click;

            LehrerLaden();
            LehrerAnzeigen();
        }


        // =========================================================
        // LEHRER LADEN
        // =========================================================

        private void LehrerLaden()
        {
            if (!File.Exists(dateiPfad))
            {
                lehrerListe = new List<LehrerDaten>();
                return;
            }

            try
            {
                string json = File.ReadAllText(dateiPfad);

                lehrerListe =
                    JsonSerializer.Deserialize<List<LehrerDaten>>(json)
                    ?? new List<LehrerDaten>();
            }
            catch
            {
                MessageBox.Show("Die Lehrer konnten nicht geladen werden.");

                lehrerListe = new List<LehrerDaten>();
            }
        }


        // =========================================================
        // LEHRER SPEICHERN
        // =========================================================

        private void LehrerSpeichern()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(lehrerListe, options);

            File.WriteAllText(dateiPfad, json);
        }


        // =========================================================
        // LEHRER ANZEIGEN
        // =========================================================

        private void LehrerAnzeigen()
        {
            if (lehrerListe.Count == 0)
            {
                showLehrer.Text = "Noch keine Lehrer vorhanden.";
                return;
            }

            StringBuilder text = new StringBuilder();

            foreach (LehrerDaten lehrer in lehrerListe)
            {
                text.AppendLine(lehrer.Name);

                foreach (var information in lehrer.Informationen)
                {
                    text.AppendLine(
                        "   " +
                        information.Key +
                        ": " +
                        information.Value
                    );
                }

                text.AppendLine();
            }

            showLehrer.Text = text.ToString();
        }


        // =========================================================
        // NEUEN LEHRER ERSTELLEN
        // =========================================================

        private void OKnewTeacherBtn_Click(object? sender, EventArgs e)
        {
            string name = newTeacherTextBox.Text.Trim();

            if (name == "")
            {
                MessageBox.Show("Bitte einen Namen eingeben.");
                return;
            }

            bool existiertSchon = lehrerListe.Any(
                lehrer => lehrer.Name.Equals(
                    name,
                    StringComparison.OrdinalIgnoreCase
                )
            );

            if (existiertSchon)
            {
                MessageBox.Show("Dieser Lehrer existiert bereits.");
                return;
            }

            LehrerDaten neuerLehrer = new LehrerDaten
            {
                Name = name
            };

            lehrerListe.Add(neuerLehrer);

            LehrerSpeichern();
            LehrerAnzeigen();

            newTeacherTextBox.Text = "";

            MessageBox.Show("Lehrer wurde erstellt.");
        }


        // =========================================================
        // LEHRER BEARBEITEN
        // =========================================================

        private void OKchangeNameBtn_Click(object? sender, EventArgs e)
        {
            string alterName = oldName.Text.Trim();
            string neuerName = newName.Text.Trim();

            if (alterName == "" || neuerName == "")
            {
                MessageBox.Show("Bitte beide Namen eingeben.");
                return;
            }

            LehrerDaten? lehrer = lehrerListe.FirstOrDefault(
                x => x.Name.Equals(
                    alterName,
                    StringComparison.OrdinalIgnoreCase
                )
            );

            if (lehrer == null)
            {
                MessageBox.Show("Lehrer wurde nicht gefunden.");
                return;
            }

            bool neuerNameExistiert = lehrerListe.Any(
                x =>
                    x != lehrer &&
                    x.Name.Equals(
                        neuerName,
                        StringComparison.OrdinalIgnoreCase
                    )
            );

            if (neuerNameExistiert)
            {
                MessageBox.Show("Der neue Name existiert bereits.");
                return;
            }

            lehrer.Name = neuerName;

            LehrerSpeichern();
            LehrerAnzeigen();

            oldName.Text = "";
            newName.Text = "";

            MessageBox.Show("Lehrer wurde bearbeitet.");
        }


        // =========================================================
        // LEHRER LÖSCHEN
        // =========================================================

        private void OKdeleteBtn_Click(object? sender, EventArgs e)
        {
            string name = nameToDelete.Text.Trim();

            if (name == "")
            {
                MessageBox.Show("Bitte einen Namen eingeben.");
                return;
            }

            LehrerDaten? lehrer = lehrerListe.FirstOrDefault(
                x => x.Name.Equals(
                    name,
                    StringComparison.OrdinalIgnoreCase
                )
            );

            if (lehrer == null)
            {
                MessageBox.Show("Lehrer wurde nicht gefunden.");
                return;
            }

            lehrerListe.Remove(lehrer);

            LehrerSpeichern();
            LehrerAnzeigen();

            nameToDelete.Text = "";

            MessageBox.Show("Lehrer wurde gelöscht.");
        }


        // =========================================================
        // INFORMATION ZU EINEM LEHRER HINZUFÜGEN
        // =========================================================

        private void OKchangingInfo_Click(object? sender, EventArgs e)
        {
            string lehrerName = nameTeacherForInfo.Text.Trim();
            string titel = infoTitel.Text.Trim();
            string information = info.Text.Trim();

            if (lehrerName == "" || titel == "" || information == "")
            {
                MessageBox.Show("Bitte alle Felder ausfüllen.");
                return;
            }

            LehrerDaten? lehrer = lehrerListe.FirstOrDefault(
                x => x.Name.Equals(
                    lehrerName,
                    StringComparison.OrdinalIgnoreCase
                )
            );

            if (lehrer == null)
            {
                MessageBox.Show("Lehrer wurde nicht gefunden.");
                return;
            }

            // Falls der Titel bereits existiert,
            // wird die bestehende Info geändert.
            lehrer.Informationen[titel] = information;

            LehrerSpeichern();
            LehrerAnzeigen();

            nameTeacherForInfo.Text = "";
            infoTitel.Text = "";
            info.Text = "";

            MessageBox.Show("Information wurde gespeichert.");
        }


        // =========================================================
        // ZURÜCK ZUM HAUPTMENÜ
        // =========================================================

        private void back_Click(object sender, EventArgs e)
        {
            Hauptmenue hauptmenue = new Hauptmenue();
            hauptmenue.StartPosition = FormStartPosition.Manual;
            hauptmenue.Location = this.Location;
            hauptmenue.Show();

            this.Close();
        }
    }


    // =============================================================
    // KLASSE FÜR EINEN LEHRER
    // =============================================================

    public class LehrerDaten
    {
        public string Name { get; set; } = "";

        public Dictionary<string, string> Informationen { get; set; }
            = new Dictionary<string, string>();
    }
}