using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;
using SchulApp.Services;

namespace SchulApp
{
    public partial class Profil
    {
        private readonly Label schuelerLinkLabel = new Label();
        private readonly ComboBox schuelerLinkComboBox = new ComboBox();
        private bool schuelerAuswahlWirdGeladen;

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);
            await SchuelerVerknuepfungInitialisierenAsync();
        }

        private async Task SchuelerVerknuepfungInitialisierenAsync()
        {
            await using SchulAppContext db = new SchulAppContext();

            LoginBenutzer? benutzer = await db.Benutzer
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == benutzerId);

            if (benutzer == null ||
                benutzer.Rolle != LoginBenutzer.RolleSchueler)
            {
                return;
            }

            if (!Controls.Contains(schuelerLinkLabel))
            {
                schuelerLinkLabel.AutoSize = true;
                schuelerLinkLabel.Font = new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );
                schuelerLinkLabel.Location = new Point(280, 560);
                schuelerLinkLabel.Text = "Verknüpfter Schüler";

                schuelerLinkComboBox.DropDownStyle =
                    ComboBoxStyle.DropDownList;
                schuelerLinkComboBox.Font = new Font("Segoe UI", 10F);
                schuelerLinkComboBox.Location = new Point(280, 585);
                schuelerLinkComboBox.Size = new Size(590, 25);
                schuelerLinkComboBox.SelectedIndexChanged +=
                    schuelerLinkComboBox_SelectedIndexChanged;

                Controls.Add(schuelerLinkLabel);
                Controls.Add(schuelerLinkComboBox);

                ClientSize = new Size(ClientSize.Width, 640);
            }

            await SchuelerAuswahlLadenAsync(benutzer.SchuelerId);
            VerknuepfungsStatusAnzeigen(benutzer.SchuelerId.HasValue);
        }

        private async Task SchuelerAuswahlLadenAsync(
            int? aktuelleSchuelerId)
        {
            schuelerAuswahlWirdGeladen = true;

            try
            {
                IReadOnlyList<StudentProfileOption> schueler =
                    await StudentProfileLinkService
                        .VerfuegbareSchuelerLadenAsync(benutzerId);

                List<SchuelerAuswahl> auswahl = new List<SchuelerAuswahl>
                {
                    new SchuelerAuswahl
                    {
                        SchuelerId = null,
                        Anzeige = "Nicht verknüpft"
                    }
                };

                auswahl.AddRange(
                    schueler.Select(x => new SchuelerAuswahl
                    {
                        SchuelerId = x.SchuelerId,
                        Anzeige = x.Anzeige
                    })
                );

                schuelerLinkComboBox.DisplayMember =
                    nameof(SchuelerAuswahl.Anzeige);
                schuelerLinkComboBox.ValueMember =
                    nameof(SchuelerAuswahl.SchuelerId);
                schuelerLinkComboBox.DataSource = auswahl;

                SchuelerAuswahl? aktuell = auswahl.FirstOrDefault(
                    x => x.SchuelerId == aktuelleSchuelerId
                );

                schuelerLinkComboBox.SelectedItem =
                    aktuell ?? auswahl[0];
            }
            finally
            {
                schuelerAuswahlWirdGeladen = false;
            }
        }

        private async void schuelerLinkComboBox_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            if (schuelerAuswahlWirdGeladen ||
                schuelerLinkComboBox.SelectedItem is not SchuelerAuswahl auswahl)
            {
                return;
            }

            try
            {
                schuelerLinkComboBox.Enabled = false;

                await StudentProfileLinkService.VerknuepfenAsync(
                    benutzerId,
                    auswahl.SchuelerId
                );

                if (auswahl.SchuelerId.HasValue)
                {
                    (string? vorname, string? nachname) =
                        StudentProfileLinkService.NameAufteilen(
                            auswahl.Anzeige
                        );

                    vornameText.Text = vorname ?? string.Empty;
                    nachnameText.Text = nachname ?? string.Empty;
                }

                VerknuepfungsStatusAnzeigen(
                    auswahl.SchuelerId.HasValue
                );

                await SchuelerAuswahlLadenAsync(auswahl.SchuelerId);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Schüler verknüpfen",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                await ProfilLadenAsync();
                await SchuelerVerknuepfungInitialisierenAsync();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show(
                    "Der Schüler konnte nicht verknüpft werden. Möglicherweise ist er bereits einem anderen Benutzer zugeordnet.",
                    "Schüler verknüpfen",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                await ProfilLadenAsync();
                await SchuelerVerknuepfungInitialisierenAsync();
            }
            finally
            {
                if (!IsDisposed)
                {
                    schuelerLinkComboBox.Enabled = true;
                }
            }
        }

        private void VerknuepfungsStatusAnzeigen(bool istVerknuepft)
        {
            vornameText.ReadOnly = istVerknuepft;
            nachnameText.ReadOnly = istVerknuepft;
            schuelerLinkLabel.ForeColor = istVerknuepft
                ? ForeColor
                : Color.Red;
        }

        private sealed class SchuelerAuswahl
        {
            public int? SchuelerId { get; init; }

            public string Anzeige { get; init; } = string.Empty;
        }
    }
}
