using SchulApp.Data;
using SchulApp.Models;
using System.Drawing;
using System.Windows.Forms;

namespace SchulApp
{
    public static class ThemeManager
    {
        private static int? aktuellerBenutzerId;

        public static Color HintergrundFarbe { get; private set; } = SystemColors.ActiveCaption;
        public static Color TextFarbe { get; private set; } = SystemColors.ControlText;

        public static void Laden(int benutzerId)
        {
            aktuellerBenutzerId = benutzerId;

            // Jeder Benutzer startet mit den Standardfarben, falls noch keine
            // persönliche Einstellung in der Datenbank gespeichert wurde.
            HintergrundFarbe = SystemColors.ActiveCaption;
            TextFarbe = SystemColors.ControlText;

            using SchulAppContext context = new SchulAppContext();

            // Die Id der Einstellung entspricht der Id des angemeldeten Benutzers.
            Einstellung? einstellung = context.Einstellungen.Find(benutzerId);

            if (einstellung == null)
            {
                return;
            }

            HintergrundFarbe = Color.FromArgb(
                einstellung.HintergrundFarbe
            );

            TextFarbe = Color.FromArgb(
                einstellung.TextFarbe
            );
        }

        public static void Speichern()
        {
            if (aktuellerBenutzerId == null)
            {
                return;
            }

            using SchulAppContext context = new SchulAppContext();

            int benutzerId = aktuellerBenutzerId.Value;
            Einstellung? einstellung = context.Einstellungen.Find(benutzerId);

            if (einstellung == null)
            {
                einstellung = new Einstellung
                {
                    Id = benutzerId,
                    HintergrundFarbe = HintergrundFarbe.ToArgb(),
                    TextFarbe = TextFarbe.ToArgb()
                };

                context.Einstellungen.Add(einstellung);
            }
            else
            {
                einstellung.HintergrundFarbe = HintergrundFarbe.ToArgb();
                einstellung.TextFarbe = TextFarbe.ToArgb();
            }

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

        public static void SitzungBeenden()
        {
            aktuellerBenutzerId = null;
            HintergrundFarbe = SystemColors.ActiveCaption;
            TextFarbe = SystemColors.ControlText;
        }
    }
}
