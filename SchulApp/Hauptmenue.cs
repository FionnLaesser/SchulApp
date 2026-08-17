using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;
using System.IO;

namespace SchulApp
{
    public partial class Hauptmenue : CustomForm
    {
        private readonly int benutzerId;
        private readonly string rolle;

        public bool AbmeldenAngefordert { get; private set; }

        public Hauptmenue(int benutzerId, string rolle)
        {
            this.benutzerId = benutzerId;

            this.rolle = rolle switch
            {
                LoginBenutzer.RolleAdmin => LoginBenutzer.RolleAdmin,
                LoginBenutzer.RolleLehrer => LoginBenutzer.RolleLehrer,
                LoginBenutzer.RolleSchueler => LoginBenutzer.RolleSchueler,
                _ => LoginBenutzer.RolleSchueler
            };

            InitializeComponent();
            ThemeManager.Anwenden(this);
            RechteAnwenden();
            hauptbildLaden();
            StandardProfilButtonBildLaden();
        }

        private bool IstAdmin =>
            rolle == LoginBenutzer.RolleAdmin;

        private bool IstLehrer =>
            rolle == LoginBenutzer.RolleLehrer;

        private async void Hauptmenue_Shown(object sender, EventArgs e)
        {
            await ProfilButtonBildLadenAsync();
        }

        private void RechteAnwenden()
        {
            if (IstAdmin)
            {
                subtitleLabel.Text =
                    "Admin: Vollzugriff auf die Schulverwaltung.";

                return;
            }

            lehrerPage.Visible = false;
            klassenPage.Visible = false;
            kursePage.Visible = false;
            auditLogPage.Visible = false;

            if (IstLehrer)
            {
                subtitleLabel.Text =
                    "Lehrer: Schüler verwalten und Stundenpläne bearbeiten.";

                schuelerPage.Location = new Point(338, 231);
                stundenplanPage.Location = new Point(338, 290);

                return;
            }

            schuelerPage.Visible = false;

            subtitleLabel.Text =
                "Schüler: Stundenplan ansehen.";

            stundenplanPage.Location = new Point(338, 260);
        }

        private void schuelerPage_Click(object sender, EventArgs e)
        {
            if (!IstAdmin && !IstLehrer)
            {
                return;
            }

            OeffneBereich(new Schueler());
        }

        private void lehrerPage_Click(object sender, EventArgs e)
        {
            if (!IstAdmin)
            {
                return;
            }

            OeffneBereich(new Lehrer());
        }

        private void klassenPage_Click(object sender, EventArgs e)
        {
            if (!IstAdmin)
            {
                return;
            }

            OeffneBereich(new Klassen());
        }

        private void kursePage_Click(object sender, EventArgs e)
        {
            if (!IstAdmin)
            {
                return;
            }

            OeffneBereich(new Kurse());
        }

        private void stundenplanPage_Click(object sender, EventArgs e)
        {
            OeffneBereich(
                new Stundenplan(
                    darfBearbeiten: IstAdmin || IstLehrer
                )
            );
        }

        private void auditLogPage_Click(object sender, EventArgs e)
        {
            if (!IstAdmin)
            {
                return;
            }

            OeffneBereich(new AuditLog());
        }

        private void hauptbildLaden()
        {
            hauptbild.Image =
                Image.FromFile("images/Hauptbild.png");
        }

        private async Task ProfilButtonBildLadenAsync()
        {
            try
            {
                await using SchulAppContext db =
                    new SchulAppContext();

                byte[]? profilbild = await db.Benutzer
                    .AsNoTracking()
                    .Where(x => x.Id == benutzerId)
                    .Select(x => x.Profilbild)
                    .SingleOrDefaultAsync();

                if (profilbild == null ||
                    profilbild.Length == 0)
                {
                    StandardProfilButtonBildLaden();
                    return;
                }

                using MemoryStream stream =
                    new MemoryStream(profilbild);

                using Image original =
                    Image.FromStream(stream);

                SetProfilButtonBild(
                    new Bitmap(
                        original,
                        new Size(34, 34)
                    )
                );
            }
            catch (Exception)
            {
                StandardProfilButtonBildLaden();
            }
        }

        private void StandardProfilButtonBildLaden()
        {
            string pfad = Path.Combine(
                AppContext.BaseDirectory,
                "images",
                "profilePicture.png"
            );

            if (!File.Exists(pfad))
            {
                SetProfilButtonBild(null);
                profileBtn.Text = "Profil";
                return;
            }

            using Image original =
                Image.FromFile(pfad);

            SetProfilButtonBild(
                new Bitmap(
                    original,
                    new Size(30, 30)
                )
            );
        }

        private void SetProfilButtonBild(Image? neuesBild)
        {
            Image? altesBild = profileBtn.Image;

            profileBtn.Image = neuesBild;
            profileBtn.ImageAlign =
                ContentAlignment.MiddleCenter;
            profileBtn.Text =
                neuesBild == null
                    ? "Profil"
                    : string.Empty;

            altesBild?.Dispose();
        }

        private void abmeldenBtn_Click(object sender, EventArgs e)
        {
            AbmeldenAngefordert = true;
            Close();
        }

        private void einstellungPage_Click(object sender, EventArgs e)
        {
            OeffneBereich(new Einstellungen());
        }

        private async void profileBtn_Click(object sender, EventArgs e)
        {
            OeffneBereich(new Profil(benutzerId));
            await ProfilButtonBildLadenAsync();
        }

        private void pingPong_Click(object sender, EventArgs e)
        {
            OeffneBereich(new PingPong());
        }

        private void bestenlistePage_Click(
            object sender,
            EventArgs e)
        {
            OeffneBereich(new Bestenliste());
        }
    }
}
