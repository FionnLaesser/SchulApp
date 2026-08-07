using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace SchulApp
{
    public partial class Schueler : Form
    {
        private readonly string dateiPfad =
            Path.Combine(Application.StartupPath, "schueler.json");

        private List<SchuelerDaten> schuelerListe = new List<SchuelerDaten>();

        public Schueler()
        {
            InitializeComponent();

            SchuelerLaden();
            SchuelerListeAktualisieren();
        }


        // =========================================================
        // SCHÜLER AUS JSON LADEN
        // =========================================================

        private void SchuelerLaden()
        {
            if (!File.Exists(dateiPfad))
            {
                schuelerListe = new List<SchuelerDaten>();
                return;
            }

            try
            {
                string json = File.ReadAllText(dateiPfad);

                schuelerListe =
                    JsonSerializer.Deserialize<List<SchuelerDaten>>(json) //Der JSON-Text wird in eine Liste von SchuelerDaten umgewandelt. 
                    ?? new List<SchuelerDaten>();                         //Falls das Ergebnis null ist ("??"), wird stattdessen eine neue leere Liste erstellt.
            }
            catch
            {
                MessageBox.Show("Die Schüler konnten nicht geladen werden.");

                schuelerListe = new List<SchuelerDaten>();
            }
        }


        // =========================================================
        // SCHÜLER IN JSON SPEICHERN
        // =========================================================

        private void SchuelerSpeichern()
        {
            JsonSerializerOptions options = new JsonSerializerOptions //Für JsonSearializer die Optionen
            {
                WriteIndented = true //eingerückt schreiben
            };

            string json = JsonSerializer.Serialize(schuelerListe, options); // Wandelt die Schülerliste mit den angegebenen Optionen in JSON um

            File.WriteAllText(dateiPfad, json);
        }


        // =========================================================
        // SCHÜLERLISTE AUF DER SEITE ANZEIGEN
        // =========================================================

        private void SchuelerListeAktualisieren()
        {
            if (schuelerListe.Count == 0)
            {
                studentListLabel.Text = "Noch keine Schüler vorhanden.";
                return;
            }

            studentListLabel.Text = string.Join( //macht die Namen aus dem select zu einem String mit jeweils neuen Zeilen
                "\n",
                schuelerListe.Select(schueler => schueler.Name) //lambda expression. Nimmt jeweils den Namen.
            );
        }


        // =========================================================
        // NEUEN SCHÜLER ERSTELLEN
        // OKnewStudent
        // =========================================================

        private void OKnewStudent_Click(object sender, EventArgs e)
        {
            string name = newStudentName.Text.Trim();

            if (name == "")
            {
                MessageBox.Show("Bitte einen Namen eingeben.");
                return;
            }

            bool existiertSchon = schuelerListe.Any(
                schueler => schueler.Name.Equals(
                    name,
                    StringComparison.OrdinalIgnoreCase
                )
            );

            if (existiertSchon)
            {
                MessageBox.Show("Dieser Schüler existiert bereits.");
                return;
            }

            SchuelerDaten neuerSchueler = new SchuelerDaten
            {
                Name = name
            };

            schuelerListe.Add(neuerSchueler);

            SchuelerSpeichern();
            SchuelerListeAktualisieren();

            newStudentName.Text = "";

            MessageBox.Show("Schüler wurde gespeichert.");
        }


        // =========================================================
        // SCHÜLER ANZEIGEN
        // showStudents
        // =========================================================

        private void showStudents_Click(object sender, EventArgs e)
        {
            if (schuelerListe.Count == 0)
            {
                MessageBox.Show("Noch keine Schüler vorhanden.");
                return;
            }

            string text = string.Join(
                "\n",
                schuelerListe.Select(schueler => schueler.Name)
            );

            MessageBox.Show(text);

            SchuelerListeAktualisieren();
        }


        // =========================================================
        // SCHÜLER BEARBEITEN
        // OKchangeName
        // =========================================================

        private void OKchangeName_Click(object sender, EventArgs e)
        {
            string alterName1 = oldName.Text.Trim();
            string neuerName1 = newName.Text.Trim();

            if (alterName1 == "" || neuerName1 == "")
            {
                MessageBox.Show("Bitte den alten und neuen Namen eingeben.");
                return;
            }

            //Sucht den ersten Schüler mit dem angegebenen Namen. Gross- und Kleinschreibung werden dabei ignoriert.
            //Wenn kein Schüler gefunden wird, ist das Ergebnis null.
            SchuelerDaten? schueler = schuelerListe.FirstOrDefault(
                x => x.Name.Equals(
                    alterName1,
                    StringComparison.OrdinalIgnoreCase
                )
            );

            if (schueler == null)
            {
                MessageBox.Show(alterName1 + " wurde nicht gefunden.");
                return;
            }
            //prüft ob der Name schon existiert
            bool neuerNameExistiert = schuelerListe.Any(
                x =>
                    x != schueler &&
                    x.Name.Equals(
                        neuerName1,
                        StringComparison.OrdinalIgnoreCase //Grosskleinschreibung wird ignoriert
                    )
            );

            if (neuerNameExistiert)
            {
                MessageBox.Show("Ein Schüler mit diesem Namen existiert bereits.");
                return;
            }

            schueler.Name = neuerName1;

            SchuelerSpeichern();
            SchuelerListeAktualisieren();

            oldName.Text = "";
            newName.Text = "";

            MessageBox.Show(
                alterName1 + " wurde zu " + neuerName1 + " geändert."
            );
        }


        // =========================================================
        // SCHÜLER LÖSCHEN
        // OKdeleteBtn
        // =========================================================

        private void OKdeleteBtn_Click(object sender, EventArgs e)
        {
            string name = deleteNameBox.Text.Trim();

            if (name == "")
            {
                MessageBox.Show("Bitte einen Namen eingeben.");
                return;
            }

            SchuelerDaten? schueler = schuelerListe.FirstOrDefault(
                x => x.Name.Equals(
                    name,
                    StringComparison.OrdinalIgnoreCase
                )
            );

            if (schueler == null)
            {
                MessageBox.Show(name + " wurde nicht gefunden.");
                return;
            }

            schuelerListe.Remove(schueler);

            SchuelerSpeichern();
            SchuelerListeAktualisieren();

            deleteNameBox.Text = "";

            MessageBox.Show(name + " wurde gelöscht.");
        }


        // =========================================================
        // EasterEgg
        // =========================================================

        private void newStudentName_TextChanged(object sender, EventArgs e)
        {
            if (newStudentName.Text == "EasterEgg")
            {
                MessageBox.Show("EasterEgg xD");
            }
        }


        private void label1_Click(object sender, EventArgs e)
        {
        }


        // =========================================================
        // ZURÜCK
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
    // DATEN EINES SCHÜLERS
    // =============================================================

    public class SchuelerDaten
    {
        public string Name { get; set; } = ""; //Getter & Setter (C# vereinfacht) gleich wie Javas Getter Setter einfach verkürtzt
    }
}