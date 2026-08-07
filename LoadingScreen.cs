using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchulApp
{
    public partial class LoadingScreen : Form
    {
        public LoadingScreen()
        {
            InitializeComponent();
            Shown += LoadingScreen_Shown;
        }

        private async void LoadingScreen_Shown(object sender, EventArgs e)
        { 
            try
            {
                loadingLabel.Text = "Verbinde mit Datenbank...";

                await DatenbankPruefen();
                await Task.Delay(1000);
                loadingLabel.Text = "Datenbank verbunden";

                Hauptmenue hauptmenue = new Hauptmenue();

                Hide();
                hauptmenue.ShowDialog();

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Verbindung zur Datenbank fehlgeschlagen:\n\n" + ex.Message,
                    "Datenbankfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                Close();
            }
        }

        private async Task DatenbankPruefen()
        {
            using var connection = Database.GetConnection();

            await connection.OpenAsync();
        }
    }
}