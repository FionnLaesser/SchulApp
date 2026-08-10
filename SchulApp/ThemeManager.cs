using SchulApp.Data;
using System.Drawing;
using System.Windows.Forms;

namespace SchulApp
{
    public static class ThemeManager
    {
        public static Color HintergrundFarbe { get; private set; } = SystemColors.Control;
        public static Color TextFarbe { get; private set; } = SystemColors.ControlText;

        public static void Laden()
        {
            // Erstellt eine Verbindung zur Datenbank über Entity Framework
            using SchulAppContext context = new SchulAppContext();

            // Sucht die Einstellung mit der Id 1
            // Entity Framework erstellt die SELECT-Abfrage automatisch
            var einstellung = context.Einstellungen.Find(1);

            // Prüft, ob die Einstellung gefunden wurde
            if (einstellung != null)
            {
                // Wandelt den gespeicherten Integer wieder in eine Farbe um
                HintergrundFarbe = Color.FromArgb(
                    einstellung.HintergrundFarbe
                );

                // Wandelt den gespeicherten Integer wieder in eine Farbe um
                TextFarbe = Color.FromArgb(
                    einstellung.TextFarbe
                );
            }
        }

        public static void Speichern()
        {
            // Erstellt eine Verbindung zur Datenbank über Entity Framework
            using SchulAppContext context = new SchulAppContext();

            // Lädt die Einstellung mit der Id 1 aus der Datenbank
            var einstellung = context.Einstellungen.Find(1);

            
            if (einstellung == null)
            {
                return;
            }

            
            einstellung.HintergrundFarbe = HintergrundFarbe.ToArgb();

            
            einstellung.TextFarbe = TextFarbe.ToArgb();

            // Speichert alle Änderungen automatisch in der Datenbank
            // Entity Framework erstellt die UPDATE-Abfrage selbst
            context.SaveChanges();
        }

        public static void HintergrundSetzen(Color farbe)
        {
            if (FarbenSindGleich(farbe, TextFarbe))
            {
                MessageBox.Show(
                    "Die Hintergrundfarbe kann nicht gleich wie die Textfarbe sein, da der Text sonst unlesbar wäre.",
                    "Ungültige Farbe",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            HintergrundFarbe = farbe;
            Speichern();
            AlleOffenenFormsAktualisieren();
        }

        public static void TextfarbeSetzen(Color farbe)
        {
            if (FarbenSindGleich(farbe, HintergrundFarbe))
            {
                MessageBox.Show(
                    "Die Textfarbe kann nicht gleich wie die Hintergrundfarbe sein, da der Text sonst unlesbar wäre.",
                    "Ungültige Farbe",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            Color alteTextFarbe = TextFarbe;

            TextFarbe = farbe;
            Speichern();
            AlleOffenenFormsAktualisieren(alteTextFarbe);
        }

        public static void Anwenden(Control control, Color? alteTextFarbe = null)
        {
            if (control is Form ||
                control is Panel ||
                control is GroupBox ||
                control is TabPage)
            {
                control.BackColor = HintergrundFarbe;
            }

            if (control.ForeColor == SystemColors.ControlText ||
                (alteTextFarbe.HasValue && control.ForeColor == alteTextFarbe.Value))
            {
                control.ForeColor = TextFarbe;
            }

            if (control is DataGridView tabelle)
            {
                tabelle.DefaultCellStyle.ForeColor = TextFarbe;
                tabelle.DefaultCellStyle.SelectionForeColor = Color.White;
            }

            foreach (Control child in control.Controls)
            {
                Anwenden(child, alteTextFarbe);
            }
        }

        private static void AlleOffenenFormsAktualisieren(Color? alteTextFarbe = null)
        {
            foreach (Form form in Application.OpenForms)
            {
                Anwenden(form, alteTextFarbe);
            }
        }

        private static bool FarbenSindGleich(Color farbe1, Color farbe2)
        {
            return farbe1.ToArgb() == farbe2.ToArgb();
        }

        public static void Zurücksetzen()
        {
            Color alteTextFarbe = TextFarbe;

            HintergrundFarbe = SystemColors.ActiveCaption;
            TextFarbe = SystemColors.ControlText;

            Speichern();
            AlleOffenenFormsAktualisieren(alteTextFarbe);
        }
    }
}