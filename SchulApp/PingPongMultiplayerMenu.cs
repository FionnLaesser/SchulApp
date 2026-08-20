using SchulApp.Models;
using SchulApp.Services;

namespace SchulApp
{
    public sealed class PingPongMultiplayerMenu : CustomForm
    {
        private readonly PingPongLobbyApiService lobbyApiService =
            new PingPongLobbyApiService();

        private readonly Button createGameButton;
        private readonly Button joinGameButton;
        private readonly Button backButton;
        private readonly Label statusLabel;

        public PingPongMultiplayerMenu()
        {
            Text = "SchulApp - Ping Pong Multiplayer";
            StartPosition = FormStartPosition.Manual;
            ClientSize = new Size(640, 420);
            MinimumSize = new Size(640, 420);

            Label titleLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                Location = new Point(55, 55),
                Size = new Size(530, 50),
                Text = "Multiplayer",
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label descriptionLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(70, 110),
                Size = new Size(500, 45),
                Text = "Erstelle ein neues Spiel oder tritt per Game Code und Host-IP bei.",
                TextAlign = ContentAlignment.MiddleCenter
            };

            createGameButton = CreateActionButton(
                "Create Game",
                "Neue Lobby auf diesem PC erstellen",
                new Point(95, 175)
            );

            joinGameButton = CreateActionButton(
                "Join Game",
                "Same PC oder LAN/IP",
                new Point(335, 175)
            );

            statusLabel = new Label
            {
                AutoSize = false,
                Location = new Point(70, 295),
                Size = new Size(500, 35),
                Text = string.Empty,
                TextAlign = ContentAlignment.MiddleCenter
            };

            backButton = new Button
            {
                Location = new Point(245, 345),
                Size = new Size(150, 38),
                Text = "Zurück",
                UseVisualStyleBackColor = true
            };

            createGameButton.Click += CreateGameButton_Click;
            joinGameButton.Click += JoinGameButton_Click;
            backButton.Click += (_, _) => Close();

            Controls.Add(titleLabel);
            Controls.Add(descriptionLabel);
            Controls.Add(createGameButton);
            Controls.Add(joinGameButton);
            Controls.Add(statusLabel);
            Controls.Add(backButton);

            ThemeManager.Anwenden(this);
        }

        private static Button CreateActionButton(
            string title,
            string description,
            Point location)
        {
            return new Button
            {
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = location,
                Size = new Size(210, 105),
                Text = $"{title}{Environment.NewLine}{Environment.NewLine}{description}",
                TextAlign = ContentAlignment.MiddleCenter,
                UseVisualStyleBackColor = true
            };
        }

        private async void CreateGameButton_Click(object? sender, EventArgs e)
        {
            SetBusy(true, "Lokale Lobby wird erstellt...");

            try
            {
                PingPongMultiplayerEndpoint.UseLocalhost();

                PingPongLobbyModel lobby =
                    await lobbyApiService.CreateLobbyAsync(
                        BenutzerSession.BenutzerId
                    );

                statusLabel.Text = string.Empty;
                OeffneBereich(new PingPongLobbyForm(lobby, isHost: true));
            }
            catch (Exception ex)
            {
                ShowLobbyError(PingPongMultiplayerError.GetUserMessage(ex));
            }
            finally
            {
                SetBusy(false, statusLabel.Text);
            }
        }

        private async void JoinGameButton_Click(object? sender, EventArgs e)
        {
            PingPongJoinGameDialog dialog = new PingPongJoinGameDialog();
            OeffneBereich(dialog);

            if (dialog.DialogResult != DialogResult.OK ||
                string.IsNullOrWhiteSpace(dialog.GameCode))
            {
                return;
            }

            SetBusy(true, "Lobby wird über den angegebenen Host gesucht...");

            try
            {
                string baseAddress =
                    PingPongMultiplayerEndpoint.ConfigureHost(dialog.HostAddress);

                PingPongLobbyModel lobby =
                    await lobbyApiService.JoinLobbyAsync(
                        dialog.GameCode,
                        BenutzerSession.BenutzerId,
                        BenutzerSession.Benutzername
                    );

                statusLabel.Text = "Verbunden mit " + baseAddress;
                OeffneBereich(new PingPongLobbyForm(lobby, isHost: false));
            }
            catch (Exception ex)
            {
                ShowLobbyError(PingPongMultiplayerError.GetUserMessage(ex));
            }
            finally
            {
                SetBusy(false, statusLabel.Text);
            }
        }

        private void SetBusy(bool busy, string text)
        {
            createGameButton.Enabled = !busy;
            joinGameButton.Enabled = !busy;
            backButton.Enabled = !busy;
            statusLabel.Text = text;
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        private void ShowLobbyError(string message)
        {
            statusLabel.Text = "Lobby-Aktion fehlgeschlagen.";

            MessageBox.Show(
                this,
                message,
                "Ping Pong Multiplayer",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
    }
}
