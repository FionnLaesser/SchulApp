using SchulApp.Models;
using SchulApp.Services;
using System.Text.Json;

namespace SchulApp
{
    public sealed class PingPongMultiplayerGameForm : CustomForm
    {
        private const int PaddleSpeed = 7;

        private readonly PingPongLobbyModel lobby;
        private readonly PingPongMultiplayerConnection connection =
            new PingPongMultiplayerConnection();
        private readonly System.Windows.Forms.Timer movementTimer;
        private readonly Panel playfield;
        private readonly Panel playerOnePaddle;
        private readonly Panel playerTwoPaddle;
        private readonly Panel localPaddle;
        private readonly Panel remotePaddle;
        private readonly Label statusLabel;
        private readonly bool isPlayerOne;
        private readonly int remoteUserId;

        private bool moveUpPressed;
        private bool moveDownPressed;
        private bool movementPending;
        private bool sendLoopRunning;
        private bool communicationAvailable;
        private double pendingNormalizedTop;
        private long localSequence;
        private long lastRemoteSequence = -1;

        public PingPongMultiplayerGameForm(PingPongLobbyModel lobby)
        {
            ArgumentNullException.ThrowIfNull(lobby);

            if (!lobby.IsReady || !lobby.GuestUserId.HasValue)
            {
                throw new InvalidOperationException(
                    "Die Multiplayer-Lobby ist noch nicht bereit."
                );
            }

            this.lobby = lobby;

            if (BenutzerSession.BenutzerId == lobby.HostUserId)
            {
                isPlayerOne = true;
                remoteUserId = lobby.GuestUserId.Value;
            }
            else if (BenutzerSession.BenutzerId == lobby.GuestUserId.Value)
            {
                isPlayerOne = false;
                remoteUserId = lobby.HostUserId;
            }
            else
            {
                throw new InvalidOperationException(
                    "Der angemeldete Benutzer gehört nicht zu dieser Lobby."
                );
            }

            Text = "SchulApp - Ping Pong Multiplayer";
            StartPosition = FormStartPosition.Manual;
            ClientSize = new Size(840, 620);
            MinimumSize = new Size(840, 620);
            KeyPreview = true;

            Label titleLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                Location = new Point(40, 48),
                Size = new Size(760, 45),
                Text = "Ping Pong Multiplayer",
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label playerOneLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(30, 92),
                Size = new Size(330, 28),
                Text = "Player 1: " + lobby.HostUsername,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label playerTwoLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(480, 92),
                Size = new Size(330, 28),
                Text = "Player 2: " + lobby.GuestUsername,
                TextAlign = ContentAlignment.MiddleRight
            };

            playfield = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(30, 125),
                Size = new Size(780, 390)
            };

            playerOnePaddle = new Panel
            {
                Location = new Point(18, 145),
                Size = new Size(16, 100)
            };

            playerTwoPaddle = new Panel
            {
                Location = new Point(744, 145),
                Size = new Size(16, 100)
            };

            Panel centerLine = new Panel
            {
                Location = new Point(387, 0),
                Size = new Size(4, 390)
            };

            playfield.Controls.Add(centerLine);
            playfield.Controls.Add(playerOnePaddle);
            playfield.Controls.Add(playerTwoPaddle);

            localPaddle = isPlayerOne
                ? playerOnePaddle
                : playerTwoPaddle;
            remotePaddle = isPlayerOne
                ? playerTwoPaddle
                : playerOnePaddle;

            string controlsText = isPlayerOne
                ? "Du bist Player 1. Steuerung: W / S"
                : "Du bist Player 2. Steuerung: Pfeil hoch / Pfeil runter";

            Label controlsLabel = new Label
            {
                AutoSize = false,
                Location = new Point(30, 525),
                Size = new Size(500, 28),
                Text = controlsText,
                TextAlign = ContentAlignment.MiddleLeft
            };

            statusLabel = new Label
            {
                AutoSize = false,
                Location = new Point(30, 553),
                Size = new Size(600, 28),
                Text = "Multiplayer-Verbindung wird hergestellt...",
                TextAlign = ContentAlignment.MiddleLeft
            };

            Button backButton = new Button
            {
                Location = new Point(650, 540),
                Size = new Size(160, 42),
                TabStop = false,
                Text = "Zurück",
                UseVisualStyleBackColor = true
            };
            backButton.Click += (_, _) => Close();

            Controls.Add(titleLabel);
            Controls.Add(playerOneLabel);
            Controls.Add(playerTwoLabel);
            Controls.Add(playfield);
            Controls.Add(controlsLabel);
            Controls.Add(statusLabel);
            Controls.Add(backButton);

            movementTimer = new System.Windows.Forms.Timer
            {
                Interval = 16
            };
            movementTimer.Tick += MovementTimer_Tick;

            KeyDown += PingPongMultiplayerGameForm_KeyDown;
            KeyUp += PingPongMultiplayerGameForm_KeyUp;
            Shown += PingPongMultiplayerGameForm_Shown;
            FormClosed += PingPongMultiplayerGameForm_FormClosed;

            connection.MessageReceived += Connection_MessageReceived;
            connection.Disconnected += Connection_Disconnected;

            ThemeManager.Anwenden(this);
            playfield.BackColor = Color.White;
            playerOnePaddle.BackColor = Color.Black;
            playerTwoPaddle.BackColor = Color.Black;
            centerLine.BackColor = Color.LightGray;
        }

        private async void PingPongMultiplayerGameForm_Shown(
            object? sender,
            EventArgs e)
        {
            try
            {
                await connection.ConnectAsync(
                    lobby.Code,
                    BenutzerSession.BenutzerId
                );

                if (IsDisposed)
                {
                    return;
                }

                communicationAvailable = true;
                statusLabel.Text =
                    "Paddle-Synchronisierung aktiv. Ball und Score folgen in #60.";
                movementTimer.Start();
                Focus();
            }
            catch (Exception ex)
            {
                communicationAvailable = false;
                statusLabel.Text =
                    "Multiplayer-Verbindung fehlgeschlagen: " + ex.Message;
            }
        }

        private void MovementTimer_Tick(object? sender, EventArgs e)
        {
            if (!communicationAvailable)
            {
                return;
            }

            int oldTop = localPaddle.Top;
            int maxTop = playfield.ClientSize.Height - localPaddle.Height;

            if (moveUpPressed)
            {
                localPaddle.Top = Math.Max(0, localPaddle.Top - PaddleSpeed);
            }

            if (moveDownPressed)
            {
                localPaddle.Top = Math.Min(
                    maxTop,
                    localPaddle.Top + PaddleSpeed
                );
            }

            if (localPaddle.Top == oldTop)
            {
                return;
            }

            pendingNormalizedTop = PingPongPlayerMovementPosition.Normalize(
                localPaddle.Top,
                maxTop
            );
            movementPending = true;

            if (!sendLoopRunning)
            {
                _ = SendPendingMovementsAsync();
            }
        }

        private async Task SendPendingMovementsAsync()
        {
            if (sendLoopRunning)
            {
                return;
            }

            sendLoopRunning = true;

            try
            {
                while (movementPending && communicationAvailable)
                {
                    movementPending = false;
                    double normalizedTop = pendingNormalizedTop;
                    long sequence = ++localSequence;

                    await connection.SendAsync(
                        PingPongMultiplayerMessageTypes.PlayerMovement,
                        new PingPongPlayerMovementPayload
                        {
                            NormalizedTop = normalizedTop
                        },
                        sequence
                    );
                }
            }
            catch (Exception ex)
            {
                communicationAvailable = false;
                movementTimer.Stop();
                moveUpPressed = false;
                moveDownPressed = false;

                if (!IsDisposed)
                {
                    statusLabel.Text =
                        "Paddle-Synchronisierung gestoppt: " + ex.Message;
                }
            }
            finally
            {
                sendLoopRunning = false;
            }
        }

        private void Connection_MessageReceived(
            object? sender,
            PingPongMultiplayerMessage message)
        {
            if (
                message.Type != PingPongMultiplayerMessageTypes.PlayerMovement ||
                message.SenderUserId != remoteUserId ||
                message.Sequence <= lastRemoteSequence)
            {
                return;
            }

            PingPongPlayerMovementPayload? payload;

            try
            {
                payload = message.Payload.Deserialize<PingPongPlayerMovementPayload>();
            }
            catch (JsonException)
            {
                return;
            }

            if (payload == null)
            {
                return;
            }

            int maxTop = playfield.ClientSize.Height - remotePaddle.Height;

            if (!PingPongPlayerMovementPosition.TryDenormalize(
                payload.NormalizedTop,
                maxTop,
                out int remoteTop))
            {
                return;
            }

            lastRemoteSequence = message.Sequence;

            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            BeginInvoke(() =>
            {
                if (!IsDisposed)
                {
                    remotePaddle.Top = remoteTop;
                }
            });
        }

        private void Connection_Disconnected(object? sender, EventArgs e)
        {
            communicationAvailable = false;
            movementPending = false;
            moveUpPressed = false;
            moveDownPressed = false;

            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            BeginInvoke(() =>
            {
                if (!IsDisposed)
                {
                    movementTimer.Stop();
                    statusLabel.Text = "Multiplayer-Verbindung wurde getrennt.";
                }
            });
        }

        private void PingPongMultiplayerGameForm_KeyDown(
            object? sender,
            KeyEventArgs e)
        {
            if (!communicationAvailable)
            {
                return;
            }

            if (isPlayerOne && e.KeyCode == Keys.W)
            {
                moveUpPressed = true;
            }
            else if (isPlayerOne && e.KeyCode == Keys.S)
            {
                moveDownPressed = true;
            }
            else if (!isPlayerOne && e.KeyCode == Keys.Up)
            {
                moveUpPressed = true;
            }
            else if (!isPlayerOne && e.KeyCode == Keys.Down)
            {
                moveDownPressed = true;
            }
            else
            {
                return;
            }

            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        private void PingPongMultiplayerGameForm_KeyUp(
            object? sender,
            KeyEventArgs e)
        {
            if (isPlayerOne && e.KeyCode == Keys.W)
            {
                moveUpPressed = false;
            }
            else if (isPlayerOne && e.KeyCode == Keys.S)
            {
                moveDownPressed = false;
            }
            else if (!isPlayerOne && e.KeyCode == Keys.Up)
            {
                moveUpPressed = false;
            }
            else if (!isPlayerOne && e.KeyCode == Keys.Down)
            {
                moveDownPressed = false;
            }
            else
            {
                return;
            }

            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        private async void PingPongMultiplayerGameForm_FormClosed(
            object? sender,
            FormClosedEventArgs e)
        {
            communicationAvailable = false;
            movementPending = false;
            movementTimer.Stop();
            movementTimer.Dispose();

            connection.MessageReceived -= Connection_MessageReceived;
            connection.Disconnected -= Connection_Disconnected;

            try
            {
                await connection.DisposeAsync();
            }
            catch
            {
                // A closing multiplayer window must not affect the rest of SchulApp.
            }
        }
    }
}
