using System;
using System.Windows.Forms;

namespace SchulApp
{
    public partial class Hauptmenue : Form
    {
        public Hauptmenue()
        {
            InitializeComponent();
            ThemeManager.Anwenden(this);
            hauptbildLaden();
        }

        private void OeffneBereich(Form formular)
        {
            formular.StartPosition = FormStartPosition.Manual;
            formular.Location = Location;

            Hide();
            formular.ShowDialog();
            Show();
            Activate();
        }

        private void schuelerPage_Click(object sender, EventArgs e)
        {
            OeffneBereich(new Schueler());
        }

        private void lehrerPage_Click(object sender, EventArgs e)
        {
            OeffneBereich(new Lehrer());
        }

        private void klassenPage_Click(object sender, EventArgs e)
        {
            OeffneBereich(new Klassen());
        }

        private void kursePage_Click(object sender, EventArgs e)
        {
            OeffneBereich(new Kurse());
        }

        private void stundenplanPage_Click(object sender, EventArgs e)
        {
            OeffneBereich(new Stundenplan());
        }
        private void hauptbildLaden()
        {
            hauptbild.Image = Image.FromFile("images/Hauptbild.png");
        }

        private void einstellungPage_Click(object sender, EventArgs e)
        {
          OeffneBereich(new Einstellungen());      
        }
   }
}
