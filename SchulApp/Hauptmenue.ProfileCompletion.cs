using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;
using SchulApp.Services;

namespace SchulApp
{
    public partial class Hauptmenue
    {
        private readonly Label profilUnvollstaendigLabel = new Label();
        private bool profilHinweisInitialisiert;

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            ProfilHinweisInitialisieren();
            await ProfilHinweisAktualisierenAsync();
        }

        protected override async void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (profilHinweisInitialisiert)
            {
                await ProfilHinweisAktualisierenAsync();
            }
        }

        private void ProfilHinweisInitialisieren()
        {
            if (profilHinweisInitialisiert ||
                rolle != LoginBenutzer.RolleSchueler)
            {
                return;
            }

            profilHinweisInitialisiert = true;

            profilUnvollstaendigLabel.AutoSize = true;
            profilUnvollstaendigLabel.Font = new Font(
                "Segoe UI",
                9F,
                FontStyle.Bold
            );
            profilUnvollstaendigLabel.ForeColor = Color.Red;
            profilUnvollstaendigLabel.Text =
                "Dein Profil ist noch nicht vollständig. Bitte vervollständige dein Profil.";
            profilUnvollstaendigLabel.Visible = false;

            Controls.Add(profilUnvollstaendigLabel);
            ProfilHinweisPositionieren();
            profilUnvollstaendigLabel.BringToFront();
        }

        private void ProfilHinweisPositionieren()
        {
            int x = titleLabel.Left +
                (titleLabel.Width - profilUnvollstaendigLabel.PreferredWidth) / 2;

            int y = titleLabel.Top -
                profilUnvollstaendigLabel.PreferredHeight - 6;

            profilUnvollstaendigLabel.Location = new Point(
                Math.Max(12, x),
                Math.Max(4, y)
            );
        }

        private async Task ProfilHinweisAktualisierenAsync()
        {
            if (rolle != LoginBenutzer.RolleSchueler ||
                !profilHinweisInitialisiert)
            {
                return;
            }

            try
            {
                await using SchulAppContext db = new SchulAppContext();

                LoginBenutzer? benutzer = await db.Benutzer
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Id == benutzerId);

                profilUnvollstaendigLabel.Visible =
                    benutzer != null &&
                    !StudentProfileLinkService.ProfilIstVollstaendig(
                        benutzer
                    );

                if (profilUnvollstaendigLabel.Visible)
                {
                    ProfilHinweisPositionieren();
                }
            }
            catch (Exception)
            {
                profilUnvollstaendigLabel.Visible = false;
            }
        }
    }
}
