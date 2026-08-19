using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
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

        private async void LoadingScreen_Shown(object? sender, EventArgs e)
        {
            try
            {
                loadingLabel.Text = "Verbinde mit Datenbank...";

                await Task.Delay(500);

                // Datenbankprüfung im Hintergrund durchführen,
                // damit der LoadingScreen nicht hängen bleibt
                await Task.Run(async () =>
                {
                    await DatenbankPruefen();
                });

                loadingLabel.Text = "Datenbank verbunden";

                await Task.Delay(500);

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
            using SchulAppContext context = new SchulAppContext();

            bool verbunden =
                await context.Database.CanConnectAsync();

            if (!verbunden)
            {
                throw new Exception(
                    "Es konnte keine Verbindung zur Datenbank hergestellt werden."
                );
            }
        }
    }
}