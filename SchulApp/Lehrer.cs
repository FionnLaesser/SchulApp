using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;
using SchulApp.Services;

namespace SchulApp
{
    public partial class Lehrer : CustomForm
    {
        private readonly LehrerApiService lehrerApiService =
            new LehrerApiService();

        private int? ausgewaehlteLehrerId = null;

        public Lehrer()
        {
            InitializeComponent();
            ThemeManager.Anwenden(this);
            LehrerListeAktualisieren();
        }

        // =========================================================
        // LEHRER MIT ENTITY FRAMEWORK LADEN
        // =========================================================

        private void LehrerListeAktualisieren()
        {
            try
            {
                // Erstellt den Entity-Framework-Datenbankkontext
                using SchulAppContext context = new SchulAppContext();

                // Lädt alle Lehrer.
                // SELECT und ORDER BY werden automatisch von EF erstellt.
                var lehrer = context.Lehrer
                    .AsNoTracking()
                    .OrderBy(l => l.LehrerId)
                    .Select(l => new
                    {
                        l.LehrerId,
                        l.Name,
                        l.Email,
                        l.Telefon
                    })
                    .ToList();

                lehrerGrid.DataSource = lehrer;

                if (lehrerGrid.Columns["LehrerId"] is DataGridViewColumn lehrerId)
                {
                    lehrerId.HeaderText = "ID";
                    lehrerId.Width = 55;
                }

                if (lehrerGrid.Columns["Name"] is DataGridViewColumn name)
                {
                    name.AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;
                }

                if (lehrerGrid.Columns["Email"] is DataGridViewColumn email)
                {
                    email.Width = 190;
                }

                if (lehrerGrid.Columns["Telefon"] is DataGridViewColumn telefon)
                {
                    telefon.Width = 115;
                }

                lehrerGrid.ClearSelection();

                AuswahlZuruecksetzen();
            }
            catch (Exception ex)
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

        private async void OKnewTeacherBtn_Click(
            object sender,
            EventArgs e)
        {
            string name = newTeacherName.Text.Trim();
            string email = newTeacherEmail.Text.Trim();
            string telefon = newTeacherTelefon.Text.Trim();

            if (name == "")
            {
                MessageBox.Show(
                    "Bitte einen Namen eingeben."
                );

                return;
            }

            try
            {
                // Erstellt ein neues Lehrer-Objekt
                LehrerModel neuerLehrer = new LehrerModel
                {
                    Name = name,

                    // Leere Felder werden als NULL gespeichert
                    Email = email == "" ? null : email,

                    Telefon = telefon == "" ? null : telefon
                };

                // Speichert den Lehrer über die REST API
                await lehrerApiService.ErstellenAsync(neuerLehrer);

                newTeacherName.Text = "";
                newTeacherEmail.Text = "";
                newTeacherTelefon.Text = "";

                LehrerListeAktualisieren();

                MessageBox.Show(
                    "Lehrer wurde gespeichert."
                );
            }
            catch (Exception ex)
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

        private void lehrerGrid_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (lehrerGrid.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row =
                lehrerGrid.SelectedRows[0];

            if (row.Cells["LehrerId"].Value == null)
            {
                return;
            }

            ausgewaehlteLehrerId =
                Convert.ToInt32(
                    row.Cells["LehrerId"].Value
                );

            string name =
                Convert.ToString(
                    row.Cells["Name"].Value
                ) ?? "";

            string email =
                Convert.ToString(
                    row.Cells["Email"].Value
                ) ?? "";

            string telefon =
                Convert.ToString(
                    row.Cells["Telefon"].Value
                ) ?? "";

            currentTeacherName.Text = name;

            editTeacherName.Text = name;
            editTeacherEmail.Text = email;
            editTeacherTelefon.Text = telefon;

            deleteTeacherName.Text = name;

            LehrerInformationenLaden();
        }

        // =========================================================
        // LEHRER BEARBEITEN
        // =========================================================

        private async void OKchangeTeacherBtn_Click(
            object sender,
            EventArgs e)
        {
            if (ausgewaehlteLehrerId == null)
            {
                MessageBox.Show(
                    "Bitte zuerst einen Lehrer in der Tabelle auswählen."
                );

                return;
            }

            string name = editTeacherName.Text.Trim();
            string email = editTeacherEmail.Text.Trim();
            string telefon = editTeacherTelefon.Text.Trim();

            if (name == "")
            {
                MessageBox.Show(
                    "Bitte einen Namen eingeben."
                );

                return;
            }

            try
            {
                LehrerModel lehrer = new LehrerModel
                {
                    LehrerId = ausgewaehlteLehrerId.Value,
                    Name = name,
                    Email = email == "" ? null : email,
                    Telefon = telefon == "" ? null : telefon
                };

                // Ändert den Lehrer über die REST API
                bool bearbeitet =
                    await lehrerApiService.BearbeitenAsync(
                        ausgewaehlteLehrerId.Value,
                        lehrer
                    );

                if (!bearbeitet)
                {
                    MessageBox.Show(
                        "Der Lehrer wurde nicht gefunden."
                    );

                    LehrerListeAktualisieren();

                    return;
                }

                LehrerListeAktualisieren();

                MessageBox.Show(
                    "Lehrer wurde bearbeitet."
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Der Lehrer konnte nicht bearbeitet werden.\n\n" +
                    ex.Message
                );
            }
        }

        // =========================================================
        // LEHRER LÖSCHEN
        // =========================================================

        private async void OKdeleteTeacherBtn_Click(
            object sender,
            EventArgs e)
        {
            if (ausgewaehlteLehrerId == null)
            {
                MessageBox.Show(
                    "Bitte zuerst einen Lehrer in der Tabelle auswählen."
                );

                return;
            }

            string name = deleteTeacherName.Text;

            DialogResult antwort =
                MessageBox.Show(
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
                // Löscht den Lehrer über die REST API
                bool geloescht =
                    await lehrerApiService.LoeschenAsync(
                        ausgewaehlteLehrerId.Value
                    );

                if (!geloescht)
                {
                    MessageBox.Show(
                        "Der Lehrer wurde nicht gefunden."
                    );

                    LehrerListeAktualisieren();

                    return;
                }

                LehrerListeAktualisieren();

                MessageBox.Show(
                    "Lehrer wurde gelöscht."
                );
            }
            catch (Exception)
            {
                // Wird zum Beispiel ausgelöst, wenn der Lehrer
                // noch mit einer Klasse oder einem Kurs verbunden ist
                MessageBox.Show(
                    "Dieser Lehrer kann noch nicht gelöscht werden, weil er einer Klasse oder einem Kurs zugeordnet ist.\n\n" +
                    "Entferne zuerst diese Zuordnung."
                );
            }
        }

        // =========================================================
        // INFORMATIONEN DES LEHRERS LADEN
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
                using SchulAppContext context = new SchulAppContext();

                int lehrerId =
                    ausgewaehlteLehrerId.Value;

                // Lädt nur die Informationen des ausgewählten Lehrers.
                // WHERE und ORDER BY erzeugt EF automatisch.
                var informationen =
                    context.LehrerInformationen
                        .AsNoTracking()
                        .Where(i => i.LehrerId == lehrerId)
                        .OrderBy(i => i.LehrerInformationId)
                        .Select(i => new
                        {
                            i.LehrerInformationId,
                            i.Titel,
                            i.Information
                        })
                        .ToList();

                lehrerInfoGrid.DataSource =
                    informationen;

                if (lehrerInfoGrid.Columns.Contains(
                    "LehrerInformationId"))

                if (lehrerInfoGrid.Columns["LehrerInformationId"] is DataGridViewColumn lehrerInformationId)
                {
                  lehrerInformationId.Visible = false;
                }

                if (lehrerInfoGrid.Columns["Titel"] is DataGridViewColumn titel)
                {
                    titel.Width = 150;
                }

                if (lehrerInfoGrid.Columns["Information"] is DataGridViewColumn information)
                {
                    information.AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.Fill;
                }

                lehrerInfoGrid.ClearSelection();

                infoTitel.Text = "";
                info.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Die Lehrerinformationen konnten nicht geladen werden.\n\n" +
                    ex.Message
                );
            }
        }

        // =========================================================
        // INFORMATION SPEICHERN ODER AKTUALISIEREN
        // =========================================================

        private void OKchangingInfo_Click(
            object sender,
            EventArgs e)
        {
            if (ausgewaehlteLehrerId == null)
            {
                MessageBox.Show(
                    "Bitte zuerst einen Lehrer in der Tabelle auswählen."
                );

                return;
            }

            string titel = infoTitel.Text.Trim();
            string information = info.Text.Trim();

            if (titel == "" || information == "")
            {
                MessageBox.Show(
                    "Bitte Titel und Information ausfüllen."
                );

                return;
            }

            try
            {
                using SchulAppContext context =
                    new SchulAppContext();

                int lehrerId =
                    ausgewaehlteLehrerId.Value;

                // Sucht nach einer bestehenden Information
                // mit gleichem Lehrer und gleichem Titel
                LehrerInformationModel? bestehendeInformation =
                    context.LehrerInformationen
                        .FirstOrDefault(i =>
                            i.LehrerId == lehrerId &&
                            i.Titel == titel
                        );

                if (bestehendeInformation == null)
                {
                    // Noch keine Information mit diesem Titel vorhanden.
                    // Deshalb wird eine neue erstellt.
                    LehrerInformationModel neueInformation =
                        new LehrerInformationModel
                        {
                            LehrerId = lehrerId,
                            Titel = titel,
                            Information = information
                        };

                    context.LehrerInformationen.Add(
                        neueInformation
                    );
                }
                else
                {
                    // Information existiert bereits.
                    // Deshalb wird nur der Inhalt aktualisiert.
                    bestehendeInformation.Information =
                        information;
                }

                // EF führt je nach Fall INSERT oder UPDATE aus
                context.SaveChanges();

                LehrerInformationenLaden();

                MessageBox.Show(
                    "Information wurde gespeichert."
                );
            }
            catch (DbUpdateException ex)
            {
                MessageBox.Show(
                    "Die Information konnte nicht gespeichert werden.\n\n" +
                    ex.Message
                );
            }
        }

        // =========================================================
        // INFO AUS TABELLE ÜBERNEHMEN
        // =========================================================

        private void lehrerInfoGrid_SelectionChanged(
            object sender,
            EventArgs e)
        {
            if (lehrerInfoGrid.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row =
                lehrerInfoGrid.SelectedRows[0];

            infoTitel.Text =
                Convert.ToString(
                    row.Cells["Titel"].Value
                ) ?? "";

            info.Text =
                Convert.ToString(
                    row.Cells["Information"].Value
                ) ?? "";
        }

        // =========================================================
        // LEHRER NEU LADEN
        // =========================================================

        private void reloadTeacherBtn_Click(
            object sender,
            EventArgs e)
        {
            LehrerListeAktualisieren();
        }

        // =========================================================
        // AUSWAHL ZURÜCKSETZEN
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
        // ZURÜCK
        // =========================================================

        private void back_Click(
            object sender,
            EventArgs e)
        {
            Close();
        }
    }
}