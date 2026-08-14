using SchulApp.Models;
using SchulApp.Services;

namespace SchulApp
{
    public partial class Bestenliste : CustomForm
    {
        private readonly PingPongApiService pingPongApiService =
            new PingPongApiService();

        public Bestenliste()
        {
            InitializeComponent();
            ThemeManager.Anwenden(this);
        }

        private async void Bestenliste_Shown(object sender, EventArgs e)
        {
            await BestenlisteLadenAsync();
        }

        private async Task BestenlisteLadenAsync()
        {
            statusLabel.Text = "Bestenliste wird geladen...";
            bestenlisteGridView.DataSource = null;

            try
            {
                List<BestenlisteEintragModel> eintraege =
                    await pingPongApiService.BestenlisteLadenAsync();

                bestenlisteGridView.DataSource = eintraege;

                statusLabel.Text = eintraege.Count == 0
                    ? "Noch keine Benutzer vorhanden."
                    : $"{eintraege.Count} Benutzer geladen.";
            }
            catch (Exception)
            {
                statusLabel.Text = "REST API nicht erreichbar.";

                MessageBox.Show(
                    "Die Bestenliste konnte nicht über die REST API geladen werden. Bitte prüfe, ob schulAppREST gestartet ist.",
                    "Bestenliste",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void zurueckBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
