using SchulApp.Models;
using SchulApp.Services;

namespace SchulApp
{
    public sealed class PingPongLobbyForm : CustomForm
    {
        private readonly PingPongLobbyApiService lobbyApiService =
            new PingPongLobbyApiService();

        private readonly System.Windows.Forms.Timer refreshTimer;
        private readonly bool isHost;
        private readonly string gameCode;

        private readonly Label gameCodeValueLabel;
        private readonly Label playerOneValueLabel;
        private readonly Label playerTwoValueLabel;
        private readonly Label statusValueLabel;
        private readonly Label informationLabel;
        private readonly Button backButton;

        private bool refreshRunning;
        private bool cleanupStarted;

        public PingPongLobbyForm(PingPongLobbyModel lobby, bool isHost)
        {
            ArgumentNullException.ThrowIfNull(lobby);

            this.isHost = isHost;
            gameCode = lobby.Code;

            Text = "SchulApp - Ping Pong Lobby";
            StartPosition = FormStartPosition.Manual;
            ClientSize = new Size(680, 470);
            MinimumSize = new Size(680, 470);
            MaximumSize = new Size(680, 470);

            Label titleLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                Location = new Point(65, 55),
                Size = new Size(550, 50),
                Text = "Ping Pong Lobby",
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label gameCodeLabel = new Label
            {
                AutoSize = false,
                Location = new Point(115, 125),
                Size = new Size(180, 30),
                Text = "Game Code:",
                TextAlign = ContentAlignment.MiddleLeft
            };

            gameCodeValueLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Location = new Point(300, 120),
                Size = new Size(260, 40),
                Text = lobby.Code,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label playerOneLabel = CreateCaptionLabel("Player 1:", 185);
            playerOneValueLabel = CreateValueLabel(185);

            Label playerTwoLabel = CreateCaptionLabel("Player 2:", 225);
            playerTwoValueLabel = CreateValueLabel(225);

            Label statusLabel = CreateCaptionLabel("Status:", 265);
            statusValueLabel = CreateValueLabel(265);

            informationLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(95, 315),
                Size = new Size(490, 55),
                TextAlign = ContentAlignment.MiddleCenter
            };

            backButton = new Button
            {
                Location = new Point(250, 395),
                Size = new Size(180, 40),
                Text = isHost ? "Lobby schliessen" : "Lobby verlassen",
                UseVisualStyleBackColor = true
            };

            backButton.Click += (_, _) => Close();

            Controls.Add(titleLabel);
            Controls.Add(gameCodeLabel);
            Controls.Add(gameCodeValueLabel);
            Controls.Add(playerOneLabel);
            Controls.Add(playerOneValueLabel);
            Controls.Add(playerTwoLabel);
            Controls.Add(playerTwoValueLabel);
            Controls.Add(statusLabel);
            Controls.Add(statusValueLabel);
            Controls.Add(informationLabel);
            Controls.Add(backButton);

            refreshTimer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            refreshTimer.Tick += RefreshTimer_Tick;

            Shown += PingPongLobbyForm_Shown;
            FormClosed += PingPongLobbyForm_FormClosed;

            ApplyLobby(lobby);
            ThemeManager.Anwenden(this);
        }

        private static Label CreateCaptionLabel(string text, int top)
        {
            return new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(115, top),
                Size = new Size(180, 30),
                Text = text,
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private static Label CreateValueLabel(int top)
        {
            return new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(300, top),
                Size = new Size(260, 30),
                TextAlign = ContentAlignment.MiddleLeft
            };
        }

        private async void PingPongLobbyForm_Shown(object? sender, EventArgs e)
        {
            await RefreshLobbyAsync();

            if (!IsDisposed)
            {
                refreshTimer.Start();
            }
        }

        private async void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            await RefreshLobbyAsync();
        }

        private async Task RefreshLobbyAsync()
        {
            if (refreshRunning || cleanupStarted || IsDisposed)
            {
                return;
            }

            refreshRunning = true;

            try
            {
                PingPongLobbyModel? lobby =
                    await lobbyApiService.GetLobbyAsync(gameCode);

                if (lobby == null)
                {
                    refreshTimer.Stop();
                    statusValueLabel.Text = "Closed";
                    informationLabel.Text =
                        "Die Lobby wurde geschlossen oder ist abgelaufen.";
                    backButton.Text = "Zurück";
                    return;
                }

                ApplyLobby(lobby);
            }
            catch (Exception ex)
            {
                refreshTimer.Stop();
                statusValueLabel.Text = "Connection error";
                informationLabel.Text =
                    "Lobby konnte nicht aktualisiert werden: " + ex.Message;
            }
            finally
            {
                refreshRunning = false;
            }
        }

        private void ApplyLobby(PingPongLobbyModel lobby)
        {
            gameCodeValueLabel.Text = lobby.Code;
            playerOneValueLabel.Text = lobby.HostUsername;
            playerTwoValueLabel.Text = string.IsNullOrWhiteSpace(lobby.GuestUsername)
                ? "Waiting for Player 2..."
                : lobby.GuestUsername;
            statusValueLabel.Text = lobby.IsReady ? "Ready" : "Waiting";

            informationLabel.Text = lobby.IsReady
                ? "Beide Spieler sind verbunden. Die Lobby ist bereit für den nächsten Multiplayer-Schritt."
                : isHost
                    ? "Teile den Game Code mit Player 2 und warte auf den Beitritt."
                    : "Die Lobby wartet auf einen zweiten Spieler.";
        }

        private void PingPongLobbyForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            refreshTimer.Stop();
            refreshTimer.Dispose();
            _ = CleanupLobbyAsync();
        }

        private async Task CleanupLobbyAsync()
        {
            if (cleanupStarted)
            {
                return;
            }

            cleanupStarted = true;

            try
            {
                if (isHost)
                {
                    await lobbyApiService.CloseLobbyAsync(
                        gameCode,
                        BenutzerSession.BenutzerId
                    );
                }
                else
                {
                    await lobbyApiService.LeaveLobbyAsync(
                        gameCode,
                        BenutzerSession.BenutzerId
                    );
                }
            }
            catch
            {
                // Die Lobby laeuft automatisch ab, falls das Aufraeumen nicht mehr moeglich ist.
            }
        }
    }
}
