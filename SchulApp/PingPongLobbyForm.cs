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
        private readonly Button copyGameCodeButton;
        private readonly Button openGameButton;
        private readonly Button backButton;

        private PingPongLobbyModel currentLobby;
        private bool refreshRunning;
        private bool cleanupStarted;
        private bool gameOpen;

        public PingPongLobbyForm(PingPongLobbyModel lobby, bool isHost)
        {
            ArgumentNullException.ThrowIfNull(lobby);

            this.isHost = isHost;
            gameCode = lobby.Code;
            currentLobby = lobby;

            Text = "SchulApp - Ping Pong Lobby";
            StartPosition = FormStartPosition.Manual;
            ClientSize = new Size(680, 490);
            MinimumSize = new Size(680, 490);

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
                Size = new Size(165, 40),
                Text = lobby.Code,
                TextAlign = ContentAlignment.MiddleLeft
            };

            copyGameCodeButton = new Button
            {
                Location = new Point(475, 120),
                Size = new Size(105, 36),
                Text = "Kopieren",
                UseVisualStyleBackColor = true
            };
            copyGameCodeButton.Click += CopyGameCodeButton_Click;

            Label playerOneLabel = CreateCaptionLabel("Player 1:", 185);
            playerOneValueLabel = CreateValueLabel(185);

            Label playerTwoLabel = CreateCaptionLabel("Player 2:", 225);
            playerTwoValueLabel = CreateValueLabel(225);

            Label statusLabel = CreateCaptionLabel("Status:", 265);
            statusValueLabel = CreateValueLabel(265);

            informationLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(70, 305),
                Size = new Size(540, 82),
                TextAlign = ContentAlignment.MiddleCenter
            };

            openGameButton = new Button
            {
                Enabled = lobby.IsReady,
                Location = new Point(145, 415),
                Size = new Size(180, 40),
                Text = "Multiplayer öffnen",
                UseVisualStyleBackColor = true
            };

            backButton = new Button
            {
                Location = new Point(355, 415),
                Size = new Size(180, 40),
                Text = isHost ? "Lobby schliessen" : "Lobby verlassen",
                UseVisualStyleBackColor = true
            };

            openGameButton.Click += OpenGameButton_Click;
            backButton.Click += BackButton_Click;

            Controls.Add(titleLabel);
            Controls.Add(gameCodeLabel);
            Controls.Add(gameCodeValueLabel);
            Controls.Add(copyGameCodeButton);
            Controls.Add(playerOneLabel);
            Controls.Add(playerOneValueLabel);
            Controls.Add(playerTwoLabel);
            Controls.Add(playerTwoValueLabel);
            Controls.Add(statusLabel);
            Controls.Add(statusValueLabel);
            Controls.Add(informationLabel);
            Controls.Add(openGameButton);
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

        private async void CopyGameCodeButton_Click(object? sender, EventArgs e)
        {
            string code = gameCodeValueLabel.Text.Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                return;
            }

            try
            {
                Clipboard.SetText(code);
                copyGameCodeButton.Enabled = false;
                copyGameCodeButton.Text = "Kopiert";

                await Task.Delay(1200);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    "Der Game Code konnte nicht kopiert werden.\n\n" + ex.Message,
                    "Game Code kopieren",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (!IsDisposed)
                {
                    copyGameCodeButton.Text = "Kopieren";
                    copyGameCodeButton.Enabled = true;
                }
            }
        }

        private async void PingPongLobbyForm_Shown(object? sender, EventArgs e)
        {
            await RefreshLobbyAsync();

            if (!IsDisposed && !gameOpen)
            {
                refreshTimer.Start();
            }
        }

        private async void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            await RefreshLobbyAsync();
        }

        private async void OpenGameButton_Click(object? sender, EventArgs e)
        {
            if (!currentLobby.IsReady || gameOpen || cleanupStarted)
            {
                return;
            }

            gameOpen = true;
            openGameButton.Enabled = false;
            refreshTimer.Stop();

            Point lobbyLocation = Location;
            Hide();

            try
            {
                using PingPongMultiplayerGameForm gameForm =
                    new PingPongMultiplayerGameForm(currentLobby)
                    {
                        StartPosition = FormStartPosition.Manual,
                        Location = lobbyLocation
                    };

                gameForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Ping Pong Multiplayer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                gameOpen = false;

                if (!IsDisposed && !cleanupStarted)
                {
                    Location = lobbyLocation;
                    Show();
                    Activate();

                    await RefreshLobbyAsync();

                    if (!IsDisposed && !cleanupStarted)
                    {
                        refreshTimer.Start();
                    }
                }
            }
        }

        private async void BackButton_Click(object? sender, EventArgs e)
        {
            backButton.Enabled = false;
            openGameButton.Enabled = false;
            refreshTimer.Stop();

            await CleanupLobbyAsync();

            Close();
        }

        private async Task RefreshLobbyAsync()
        {
            if (refreshRunning || cleanupStarted || gameOpen || IsDisposed)
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
                    openGameButton.Enabled = false;
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
                openGameButton.Enabled = false;
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
            currentLobby = lobby;
            gameCodeValueLabel.Text = lobby.Code;
            copyGameCodeButton.Enabled = !string.IsNullOrWhiteSpace(lobby.Code);
            playerOneValueLabel.Text = lobby.HostUsername;
            playerTwoValueLabel.Text = string.IsNullOrWhiteSpace(lobby.GuestUsername)
                ? "Waiting for Player 2..."
                : lobby.GuestUsername;
            statusValueLabel.Text = lobby.IsReady ? "Ready" : "Waiting";
            openGameButton.Enabled = lobby.IsReady && !gameOpen && !cleanupStarted;

            string connectionText = GetConnectionInformation();

            informationLabel.Text = lobby.IsReady
                ? "Beide Spieler sind verbunden. Öffne den Multiplayer-Modus.\n" +
                    connectionText
                : isHost
                    ? "Teile Game Code und LAN-IP mit Player 2.\n" + connectionText
                    : "Warte auf die Lobby.\n" + connectionText;
        }

        private string GetConnectionInformation()
        {
            if (!isHost)
            {
                return "Server: " + PingPongMultiplayerEndpoint.CurrentBaseAddress;
            }

            IReadOnlyList<string> addresses =
                PingPongMultiplayerEndpoint.GetLocalIPv4Addresses();

            if (addresses.Count == 0)
            {
                return $"Same PC: localhost | LAN-Port: {PingPongMultiplayerEndpoint.DefaultPort}";
            }

            return "LAN: " +
                string.Join(
                    " | ",
                    addresses.Select(
                        address => $"{address}:{PingPongMultiplayerEndpoint.DefaultPort}"
                    )
                );
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
            }
        }
    }
}
