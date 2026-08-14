using SchulApp.Services;

namespace SchulApp
{
    public partial class Einstellungen : CustomForm
    {
        private readonly EinstellungApiService einstellungApiService =
            new EinstellungApiService();

        private bool ballSpeedGeladen;

        public Einstellungen()
        {
            InitializeComponent();

            ThemeManager.Anwenden(this);
            userNameLabel.Text = $"Angemeldet als: {BenutzerSession.Benutzername}";
            Shown += Einstellungen_Shown;
        }

        private async void Einstellungen_Shown(object? sender, EventArgs e)
        {
            if (ballSpeedGeladen)
            {
                return;
            }

            ballSpeedGeladen = true;
            await BallSpeedLadenAsync();
        }

        private async Task BallSpeedLadenAsync()
        {
            ballSpeedSpeichernBtn.Enabled = false;
            ballSpeedStatusLabel.Text = "Ballgeschwindigkeit wird geladen...";

            try
            {
                int ballSpeed = await einstellungApiService.BallSpeedLadenAsync(
                    BenutzerSession.BenutzerId
                );

                ballSpeedNumericUpDown.Value = ballSpeed;
                ballSpeedStatusLabel.Text = "Gespeicherte Ballgeschwindigkeit geladen.";
            }
            catch (Exception)
            {
                ballSpeedNumericUpDown.Value = 6;
                ballSpeedStatusLabel.Text =
                    "REST API nicht erreichbar. Standardwert 6 wird angezeigt.";
            }
            finally
            {
                ballSpeedSpeichernBtn.Enabled = true;
            }
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

        private async void resetSettingsBtn_Click(object sender, EventArgs e)
        {
            ThemeManager.Zurücksetzen();

            ballSpeedNumericUpDown.Value = 6;
            await BallSpeedSpeichernAsync();
        }

        private async void ballSpeedSpeichernBtn_Click(object sender, EventArgs e)
        {
            await BallSpeedSpeichernAsync();
        }

        private async Task BallSpeedSpeichernAsync()
        {
            int ballSpeed = Decimal.ToInt32(ballSpeedNumericUpDown.Value);

            ballSpeedSpeichernBtn.Enabled = false;
            ballSpeedStatusLabel.Text = "Ballgeschwindigkeit wird gespeichert...";

            try
            {
                await einstellungApiService.BallSpeedSpeichernAsync(
                    BenutzerSession.BenutzerId,
                    ballSpeed,
                    ThemeManager.HintergrundFarbe.ToArgb(),
                    ThemeManager.TextFarbe.ToArgb()
                );

                ballSpeedStatusLabel.Text =
                    $"Ballgeschwindigkeit {ballSpeed} wurde gespeichert.";
            }
            catch (Exception)
            {
                ballSpeedStatusLabel.Text =
                    "Ballgeschwindigkeit konnte nicht gespeichert werden.";

                MessageBox.Show(
                    "Die Ballgeschwindigkeit konnte nicht über die REST API gespeichert werden. Bitte prüfe, ob schulAppREST läuft.",
                    "REST API Fehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                ballSpeedSpeichernBtn.Enabled = true;
            }
        }
    }
}
