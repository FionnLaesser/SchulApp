using SchulApp.Models;
using SchulApp.Services;

namespace SchulApp
{
    public partial class PingPong : CustomForm
    {
        private const int MaxPunkte = 5;

        private readonly PingPongApiService pingPongApiService =
            new PingPongApiService();

        private int ballSpeedX = 5;
        private int ballSpeedY = 5;
        private int spielerSpeed = 7;

        private int punkteLinks;
        private int punkteRechts;

        private int spielerRechtsId;
        private string spielerLinksName = string.Empty;
        private string spielerRechtsName = string.Empty;

        private bool spielLaeuft;
        private bool hochGedruecktLinks;
        private bool runterGedruecktLinks;
        private bool hochGedruecktRechts;
        private bool runterGedruecktRechts;

        public PingPong()
        {
            InitializeComponent();

            ThemeManager.Anwenden(this);
            Minimizing += PingPong_Minimizing;
            panelLinks.BackColor = Color.Black;
            panelRechts.BackColor = Color.Black;
            panelTop.BackColor = Color.Black;

            KeyPreview = true;

            KeyDown += PingPong_KeyDown;
            KeyUp += PingPong_KeyUp;

            gameTimer.Interval = 16;
            gameTimer.Stop();

            PunkteAnzeigen();
            SpielfeldZuruecksetzen(1);
        }

        private void PingPong_Minimizing(object? sender, EventArgs e)
        {
            if (!spielLaeuft)
            {
                return;
            }

            SpielPausieren(
                "Das Spiel wurde durch das Minimieren pausiert."
            );
        }

        private void SpielPausieren(string text)
        {
            gameTimer.Stop();

            hochGedruecktLinks = false;
            runterGedruecktLinks = false;
            hochGedruecktRechts = false;
            runterGedruecktRechts = false;

            pauseTextLabel.Text = text;
            pausePanel.Visible = true;
            pausePanel.BringToFront();
        }

        private void fortsetzenBtn_Click(object sender, EventArgs e)
        {
            if (!spielLaeuft)
            {
                return;
            }

            pausePanel.Visible = false;
            gameTimer.Start();
            Focus();
        }

        private async void PingPong_Shown(object sender, EventArgs e)
        {
            await SpielerLadenAsync();
        }

        private async Task SpielerLadenAsync()
        {
            spielStartBtn.Enabled = false;
            gegnerComboBox.Enabled = false;
            statusLabel.Text = "Benutzer werden über die REST API geladen...";

            try
            {
                List<BestenlisteEintragModel> benutzer =
                    await pingPongApiService.BestenlisteLadenAsync();

                BestenlisteEintragModel? angemeldeterBenutzer =
                    benutzer.FirstOrDefault(
                        x => x.BenutzerId == BenutzerSession.BenutzerId
                    );

                if (angemeldeterBenutzer == null)
                {
                    MessageBox.Show(
                        "Der angemeldete Benutzer wurde über die REST API nicht gefunden.",
                        "Ping Pong",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );

                    Close();
                    return;
                }

                spielerLinksName = angemeldeterBenutzer.Benutzername;
                spielerLinksNameLabel.Text = $"Links: {spielerLinksName}";

                List<BestenlisteEintragModel> gegner = benutzer
                    .Where(x => x.BenutzerId != BenutzerSession.BenutzerId)
                    .OrderBy(x => x.Benutzername)
                    .ToList();

                gegnerComboBox.DataSource = gegner;
                gegnerComboBox.DisplayMember = nameof(
                    BestenlisteEintragModel.Benutzername
                );
                gegnerComboBox.ValueMember = nameof(
                    BestenlisteEintragModel.BenutzerId
                );

                bool gegnerVorhanden = gegner.Count > 0;

                gegnerComboBox.Enabled = gegnerVorhanden;
                spielStartBtn.Enabled = gegnerVorhanden;

                statusLabel.Text = gegnerVorhanden
                    ? "Gegner auswählen und Spiel starten."
                    : "Für ein Spiel wird ein zweiter Benutzer benötigt.";

                GegnerAnzeigeAktualisieren();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.GetType().Name + "\n\n" + ex.Message,
                    "REST API Fehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        private void gegnerComboBox_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            GegnerAnzeigeAktualisieren();
        }

        private void GegnerAnzeigeAktualisieren()
        {
            if (gegnerComboBox.SelectedItem is BestenlisteEintragModel gegner)
            {
                spielerRechtsId = gegner.BenutzerId;
                spielerRechtsName = gegner.Benutzername;
                spielerRechtsNameLabel.Text =
                    $"Rechts: {spielerRechtsName}";
            }
            else
            {
                spielerRechtsId = 0;
                spielerRechtsName = string.Empty;
                spielerRechtsNameLabel.Text = "Rechts: -";
            }
        }

        private void spielStartBtn_Click(object sender, EventArgs e)
        {
            if (spielerRechtsId == 0)
            {
                return;
            }

            NeuesSpielStarten();
        }

        private void NeuesSpielStarten()
        {
            punkteLinks = 0;
            punkteRechts = 0;

            hochGedruecktLinks = false;
            runterGedruecktLinks = false;
            hochGedruecktRechts = false;
            runterGedruecktRechts = false;

            PunkteAnzeigen();

            spielEndePanel.Visible = false;
            pausePanel.Visible = false;
            gegnerComboBox.Enabled = false;
            spielStartBtn.Enabled = false;

            statusLabel.Text =
                $"Spiel bis {MaxPunkte}: {spielerLinksName} gegen {spielerRechtsName}";

            int richtung = Random.Shared.Next(0, 2) == 0 ? -1 : 1;

            SpielfeldZuruecksetzen(richtung);

            spielLaeuft = true;
            gameTimer.Start();
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            if (!spielLaeuft)
            {
                return;
            }

            BallBewegen();
            SpielerBewegenLinks();
            SpielerBewegenRechts();
        }

        private void SpielerBewegenLinks()
        {
            if (hochGedruecktLinks &&
                panelLinks.Top > panelTop.Bottom)
            {
                panelLinks.Top = Math.Max(
                    panelTop.Bottom,
                    panelLinks.Top - spielerSpeed
                );
            }

            if (runterGedruecktLinks &&
                panelLinks.Bottom < ClientSize.Height)
            {
                panelLinks.Top = Math.Min(
                    ClientSize.Height - panelLinks.Height,
                    panelLinks.Top + spielerSpeed
                );
            }
        }

        private void SpielerBewegenRechts()
        {
            if (hochGedruecktRechts &&
                panelRechts.Top > panelTop.Bottom)
            {
                panelRechts.Top = Math.Max(
                    panelTop.Bottom,
                    panelRechts.Top - spielerSpeed
                );
            }

            if (runterGedruecktRechts &&
                panelRechts.Bottom < ClientSize.Height)
            {
                panelRechts.Top = Math.Min(
                    ClientSize.Height - panelRechts.Height,
                    panelRechts.Top + spielerSpeed
                );
            }
        }

        private void BallBewegen()
        {
            ball.Left += ballSpeedX;
            ball.Top += ballSpeedY;

            if (ball.Top <= panelTop.Bottom)
            {
                ball.Top = panelTop.Bottom;
                ballSpeedY = Math.Abs(ballSpeedY);
            }

            if (ball.Bottom >= ClientSize.Height)
            {
                ball.Top = ClientSize.Height - ball.Height;
                ballSpeedY = -Math.Abs(ballSpeedY);
            }

            if (ball.Bounds.IntersectsWith(panelLinks.Bounds) &&
                ballSpeedX < 0)
            {
                ball.Left = panelLinks.Right;
                ballSpeedX = Math.Abs(ballSpeedX);
            }

            if (ball.Bounds.IntersectsWith(panelRechts.Bounds) &&
                ballSpeedX > 0)
            {
                ball.Left = panelRechts.Left - ball.Width;
                ballSpeedX = -Math.Abs(ballSpeedX);
            }

            PunktePruefen();
        }

        private void PunktePruefen()
        {
            if (ball.Right < 0)
            {
                punkteRechts++;
                PunkteAnzeigen();

                if (punkteRechts >= MaxPunkte)
                {
                    _ = SpielBeendenAsync();
                    return;
                }

                SpielfeldZuruecksetzen(-1);
            }
            else if (ball.Left > ClientSize.Width)
            {
                punkteLinks++;
                PunkteAnzeigen();

                if (punkteLinks >= MaxPunkte)
                {
                    _ = SpielBeendenAsync();
                    return;
                }

                SpielfeldZuruecksetzen(1);
            }
        }

        private void PunkteAnzeigen()
        {
            punkteZahlLabel.Text =
                $"{punkteLinks} : {punkteRechts}";
        }

        private void SpielfeldZuruecksetzen(int richtung)
        {
            int spielfeldHoehe =
                ClientSize.Height - panelTop.Bottom;

            ball.Left =
                (ClientSize.Width - ball.Width) / 2;

            ball.Top =
                panelTop.Bottom +
                (spielfeldHoehe - ball.Height) / 2;

            panelLinks.Top =
                panelTop.Bottom +
                (spielfeldHoehe - panelLinks.Height) / 2;

            panelRechts.Top =
                panelTop.Bottom +
                (spielfeldHoehe - panelRechts.Height) / 2;

            ballSpeedX = Math.Abs(ballSpeedX) * richtung;

            ballSpeedY = Random.Shared.Next(0, 2) == 0
                ? -Math.Abs(ballSpeedY)
                : Math.Abs(ballSpeedY);
        }

        private async Task SpielBeendenAsync()
        {
            if (!spielLaeuft)
            {
                return;
            }

            spielLaeuft = false;
            gameTimer.Stop();
            pausePanel.Visible = false;

            string gewinner = punkteLinks > punkteRechts
                ? spielerLinksName
                : spielerRechtsName;

            spielEndeTitelLabel.Text = "Spiel beendet!";
            gewinnerLabel.Text = $"Gewinner: {gewinner}";
            ergebnisLabel.Text = $"Ergebnis: {punkteLinks} : {punkteRechts}";
            speicherStatusLabel.Text =
                "Ergebnis wird über die REST API gespeichert...";

            erneutSpielenBtn.Enabled = false;
            spielEndePanel.Visible = true;
            spielEndePanel.BringToFront();

            try
            {
                await pingPongApiService.SpielSpeichernAsync(
                    BenutzerSession.BenutzerId,
                    spielerRechtsId,
                    punkteLinks,
                    punkteRechts
                );

                speicherStatusLabel.Text =
                    "Ergebnis und Bestenliste wurden gespeichert.";

                erneutSpielenBtn.Enabled = true;
            }
            catch (Exception)
            {
                speicherStatusLabel.Text =
                    "Ergebnis konnte nicht gespeichert werden.";

                MessageBox.Show(
                    "Das Spielergebnis konnte nicht über die REST API gespeichert werden. Bitte prüfe, ob schulAppREST läuft.",
                    "Ping Pong",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void erneutSpielenBtn_Click(object sender, EventArgs e)
        {
            NeuesSpielStarten();
        }

        private void zurueckBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void pauseHauptmenueBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void PingPong_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Q && spielLaeuft)
            {
                if (!pausePanel.Visible)
                {
                    SpielPausieren(
                        "Spiel pausiert. Fortsetzen oder zum Hauptmenü zurückkehren."
                    );
                }

                e.SuppressKeyPress = true;
                e.Handled = true;
                return;
            }

            if (!spielLaeuft || pausePanel.Visible)
            {
                return;
            }

            if (e.KeyCode == Keys.W)
            {
                hochGedruecktLinks = true;
            }
            else if (e.KeyCode == Keys.S)
            {
                runterGedruecktLinks = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                hochGedruecktRechts = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                runterGedruecktRechts = true;
            }
            else
            {
                return;
            }

            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        private void PingPong_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                hochGedruecktLinks = false;
            }

            if (e.KeyCode == Keys.S)
            {
                runterGedruecktLinks = false;
            }

            if (e.KeyCode == Keys.Up)
            {
                hochGedruecktRechts = false;
            }

            if (e.KeyCode == Keys.Down)
            {
                runterGedruecktRechts = false;
            }
        }
    }
}
