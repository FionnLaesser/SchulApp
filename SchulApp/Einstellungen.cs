namespace SchulApp
{
    public partial class Einstellungen : Form
    {
        public Einstellungen()
        {
            InitializeComponent();

            ThemeManager.Anwenden(this);
        }

        private void backgroundColorBtn_Click(object sender, EventArgs e)
        {
            colorDialogBgColor.Color = ThemeManager.HintergrundFarbe;

            if (colorDialogBgColor.ShowDialog() == DialogResult.OK)
            {
                ThemeManager.HintergrundSetzen(
                    colorDialogBgColor.Color
                );
            }
        }

        private void textfarbeBtn_Click(object sender, EventArgs e)
        {
            colorDialogTextFarbe.Color = ThemeManager.TextFarbe;

            if (colorDialogTextFarbe.ShowDialog() == DialogResult.OK)
            {
                ThemeManager.TextfarbeSetzen(
                    colorDialogTextFarbe.Color
                );
            }

        }

        private void back_Click_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void resetSettingsBtn_Click(object sender, EventArgs e)
        {
            ThemeManager.Zurücksetzen();
        }
    }
}