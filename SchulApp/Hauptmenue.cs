using SchulApp.Models;

namespace SchulApp
{
    public partial class Hauptmenue : Form
    {
        private readonly string rolle;

        public bool AbmeldenAngefordert { get; private set; }

        public Hauptmenue(string rolle)
        {
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
        }

        private bool IstAdmin =>
            rolle == LoginBenutzer.RolleAdmin;

        private bool IstLehrer =>
            rolle == LoginBenutzer.RolleLehrer;

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
            einstellungPage.Visible = false;

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

        private void OeffneBereich(Form formular)
        {
            formular.StartPosition = FormStartPosition.Manual;
            formular.Location = Location;

            // App-Icon vom Hauptmenü übernehmen
            formular.Icon = Icon;

            Hide();
            formular.ShowDialog();
            Show();
            Activate();
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
            OeffneBereich(new Stundenplan(darfBearbeiten: IstAdmin || IstLehrer));
        }

        private void hauptbildLaden()
        {
            hauptbild.Image = Image.FromFile("images/Hauptbild.png");
        }

        private void abmeldenBtn_Click(object sender, EventArgs e)
        {
            AbmeldenAngefordert = true;
            Close();
        }

        private void einstellungPage_Click(object sender, EventArgs e)
        {
            if (!IstAdmin)
            {
                return;
            }

            OeffneBereich(new Einstellungen());
        }
    }
}
