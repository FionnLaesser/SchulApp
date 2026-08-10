using Microsoft.Data.SqlClient;
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
            const string sql = @"
                SELECT HintergrundFarbe, TextFarbe
                FROM dbo.Einstellung
                WHERE Id = 1;";

            using SqlConnection connection = Database.GetConnection();
            connection.Open();

            using SqlCommand command = new SqlCommand(sql, connection);
            using SqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                HintergrundFarbe = Color.FromArgb(
                    reader.GetInt32(0)
                );

                TextFarbe = Color.FromArgb(
                    reader.GetInt32(1)
                );
            }
        }

        public static void Speichern()
        {
            const string sql = @"
                UPDATE dbo.Einstellung
                SET HintergrundFarbe = @HintergrundFarbe,
                    TextFarbe = @TextFarbe
                WHERE Id = 1;";

            using SqlConnection connection = Database.GetConnection();
            connection.Open();

            using SqlCommand command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@HintergrundFarbe",
                HintergrundFarbe.ToArgb()
            );

            command.Parameters.AddWithValue(
                "@TextFarbe",
                TextFarbe.ToArgb()
            );

            command.ExecuteNonQuery();
        }

        public static void HintergrundSetzen(Color farbe)
        {
            HintergrundFarbe = farbe;
            Speichern();
            AlleOffenenFormsAktualisieren();
        }

        public static void TextfarbeSetzen(Color farbe)
        {
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
    }
}