using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using ServiceReference1;

namespace SchulApp
{
    public partial class Schueler : CustomForm
    {
        private readonly SchuelerServiceClient soapClient;
        private readonly bool nurHinzufuegen;
        private int? ausgewaehlteSchuelerId;

        public Schueler(bool nurHinzufuegen = false)
        {
            this.nurHinzufuegen = nurHinzufuegen;

            InitializeComponent();

            soapClient = new SchuelerServiceClient();

            ThemeManager.Anwenden(this);
            RechteAnwenden();

            KlassenLaden();

            Shown += async (_, _) => await SchuelerListeAktualisierenAsync();
        }

        private void KlassenLaden()
        {
            try
            {
                using SchulAppContext context = new SchulAppContext();

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
                newStudentKlasse.DataSource = klassen.ToList();

                editKlasse.DisplayMember = "Bezeichnung";
                editKlasse.ValueMember = "KlassenId";
                editKlasse.DataSource = klassen.ToList();

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

        private async Task SchuelerListeAktualisierenAsync()
        {
            try
            {
                var schueler = await soapClient.GetSchuelerAsync();

                using SchulAppContext context = new SchulAppContext();

                var klassen = context.Klassen
                    .AsNoTracking()
                    .ToDictionary(
                        k => k.KlassenId,
                        k => k.Bezeichnung
                    );

                var anzeige = schueler
                    .OrderBy(s => s.SchuelerId)
                    .Select(s => new
                    {
                        s.SchuelerId,
                        s.Name,
                        s.KlasseId,
                        Klasse = klassen.TryGetValue(
                            s.KlasseId,
                            out string? bezeichnung
                        )
                            ? bezeichnung
                            : "Unbekannt"
                    })
                    .ToList();

                studentGrid.DataSource = anzeige;

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
                    "Die Schüler konnten nicht über SOAP geladen werden.\n\n" +
                    ex.Message
                );
            }
        }

        private async void OKnewStudent_Click(
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

            if (string.IsNullOrWhiteSpace(newStudentName.Text))
            {
                MessageBox.Show(
                    "Bitte einen Namen eingeben."
                );

                return;
            }

            try
            {
                var schueler = new ServiceReference1.SchuelerModel
                {
                    Name = newStudentName.Text.Trim(),
                    KlasseId = Convert.ToInt32(
                        newStudentKlasse.SelectedValue
                    ),
                    AuditUserName =
                        BenutzerSession.Benutzername ?? string.Empty,
                    AuditUserRole =
                        BenutzerSession.Rolle ?? string.Empty
                };

                bool erstellt =
                    await soapClient.AddSchuelerAsync(schueler);

                if (!erstellt)
                {
                    MessageBox.Show(
                        "Der Schüler konnte nicht gespeichert werden."
                    );

                    return;
                }

                newStudentName.Text = "";

                await SchuelerListeAktualisierenAsync();

                MessageBox.Show(
                    "Schüler wurde gespeichert."
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Der Schüler konnte nicht über SOAP gespeichert werden.\n\n" +
                    ex.Message
                );
            }
        }

        private async void showStudents_Click(
            object sender,
            EventArgs e)
        {
            KlassenLaden();
            await SchuelerListeAktualisierenAsync();
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

        private async void OKchangeName_Click(
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

            if (string.IsNullOrWhiteSpace(newName.Text))
            {
                MessageBox.Show(
                    "Bitte einen Namen eingeben."
                );

                return;
            }

            try
            {
                var schueler = new ServiceReference1.SchuelerModel
                {
                    SchuelerId = ausgewaehlteSchuelerId.Value,
                    Name = newName.Text.Trim(),
                    KlasseId = Convert.ToInt32(
                        editKlasse.SelectedValue
                    ),
                    AuditUserName =
                        BenutzerSession.Benutzername ?? string.Empty,
                    AuditUserRole =
                        BenutzerSession.Rolle ?? string.Empty
                };

                bool bearbeitet =
                    await soapClient.UpdateSchuelerAsync(schueler);

                if (!bearbeitet)
                {
                    MessageBox.Show(
                        "Der Schüler wurde nicht gefunden oder konnte nicht bearbeitet werden."
                    );

                    await SchuelerListeAktualisierenAsync();

                    return;
                }

                await SchuelerListeAktualisierenAsync();

                MessageBox.Show(
                    "Schüler wurde bearbeitet."
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Der Schüler konnte nicht über SOAP bearbeitet werden.\n\n" +
                    ex.Message
                );
            }
        }

        private async void OKdeleteBtn_Click(
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
                bool geloescht =
                    await soapClient.DeleteSchuelerMitAuditAsync(
                        ausgewaehlteSchuelerId.Value,
                        BenutzerSession.Benutzername ?? string.Empty,
                        BenutzerSession.Rolle ?? string.Empty
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

                await SchuelerListeAktualisierenAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Der Schüler konnte nicht über SOAP gelöscht werden.\n\n" +
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
