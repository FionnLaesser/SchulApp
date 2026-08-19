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
        private readonly PingPongBackgroundKeyboardInput backgroundInput;
        private readonly System.Windows.Forms.Timer movementTimer;
        private readonly Panel playfield;
        private readonly Panel playerOnePaddle;
        private readonly Panel playerTwoPaddle;
        private readonly Panel centerLine;
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

            backgroundInput = new PingPongBackgroundKeyboardInput(
                isPlayerOne
                    ? new[] { Keys.W, Keys.S }
                    : new[] { Keys.Up, Keys.Down }
            );

            Text = "SchulApp - Ping Pong Multiplayer";
            StartPosition = FormStartPosition.Manual;
            ClientSize = new Size(1200, 750);
            MinimumSize = new Size(900, 600);
            KeyPreview = true;

            Label controlsLabel = new Label
            {
                AutoSize = false,
                Location = new Point(20, 18),
                Size = new Size(500, 24),
                Text = isPlayerOne
                    ? "Player 1: W / S | Steuerung auch ohne Fensterfokus"
                    : "Player 2: Pfeil hoch / Pfeil runter | Steuerung auch ohne Fensterfokus",
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label playerOneLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(20, 55),
                Size = new Size(330, 24),
                Text = "Player 1: " + lobby.HostUsername,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label playerTwoLabel = new Label
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                AutoSize = false,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Location = new Point(850, 55),
                Size = new Size(330, 24),
                Text = "Player 2: " + lobby.GuestUsername,
                TextAlign = ContentAlignment.MiddleRight
            };

            statusLabel = new Label
            {
                AutoSize = false,
                Location = new Point(20, 82),
                Size = new Size(760, 20),
                Text = "Multiplayer-Verbindung wird hergestellt...",
                TextAlign = ContentAlignment.MiddleLeft
            };

            NoFocusButton backButton = new NoFocusButton
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(1085, 48),
                Size = new Size(95, 23),
                Text = "Zurück",
                TabStop = false,
                UseVisualStyleBackColor = true
            };
            backButton.Click += (_, _) => Close();

            playfield = new Panel
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                    AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle = BorderStyle.None,
                Location = new Point(0, 105),
                Size = new Size(1200, 645)
            };

            playerOnePaddle = new Panel
            {
                Location = new Point(0, 224),
                Size = new Size(34, 196)
            };

            playerTwoPaddle = new Panel
            {
                Location = new Point(1166, 224),
                Size = new Size(34, 196)
            };

            centerLine = new Panel
            {
                Location = new Point(598, 0),
                Size = new Size(4, 645)
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

            Controls.Add(controlsLabel);
            Controls.Add(playerOneLabel);
            Controls.Add(playerTwoLabel);
            Controls.Add(statusLabel);
            Controls.Add(backButton);
            Controls.Add(playfield);

            movementTimer = new System.Windows.Forms.Timer
            {
                Interval = 16
            };
            movementTimer.Tick += MovementTimer_Tick;

            KeyDown += PingPongMultiplayerGameForm_KeyDown;
            KeyUp += PingPongMultiplayerGameForm_KeyUp;
            PreviewKeyDown += PingPongMultiplayerGameForm_PreviewKeyDown;
            Shown += PingPongMultiplayerGameForm_Shown;
            FormClosed += PingPongMultiplayerGameForm_FormClosed;
            playfield.Resize += Playfield_Resize;

            backgroundInput.KeyStateChanged += BackgroundInput_KeyStateChanged;
            connection.MessageReceived += Connection_MessageReceived;
            connection.Disconnected += Connection_Disconnected;

            ThemeManager.Anwenden(this);
            playfield.BackColor = Color.White;
            playerOnePaddle.BackColor = Color.Black;
            playerTwoPaddle.BackColor = Color.Black;
            centerLine.BackColor = Color.LightGray;

            CenterPaddles();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;

            if (IsAssignedMovementKey(keyCode))
            {
                if (communicationAvailable)
                {
                    SetMovementKeyState(keyCode, true);
                }

                return true;
            }

            if (keyCode == Keys.W ||
                keyCode == Keys.S ||
                keyCode == Keys.Up ||
                keyCode == Keys.Down)
            {
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
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
                movementTimer.Start();

                try
                {
                    backgroundInput.Start();
                    statusLabel.Text =
                        "Paddle-Synchronisierung aktiv. Steuerung funktioniert auch ohne Fensterfokus.";
                }
                catch (Exception ex)
                {
                    statusLabel.Text =
                        "Paddle-Synchronisierung aktiv. Hintergrund-Steuerung nicht verfügbar: " +
                        ex.Message;
                }

                ActiveControl = null;
                Focus();
            }
            catch (Exception ex)
            {
                communicationAvailable = false;
                statusLabel.Text =
                    "Multiplayer-Verbindung fehlgeschlagen: " + ex.Message;
            }
        }

        private void Playfield_Resize(object? sender, EventArgs e)
        {
            playerOnePaddle.Left = 0;
            playerTwoPaddle.Left = Math.Max(
                0,
                playfield.ClientSize.Width - playerTwoPaddle.Width
            );
            centerLine.Left = Math.Max(
                0,
                (playfield.ClientSize.Width - centerLine.Width) / 2
            );
            centerLine.Height = playfield.ClientSize.Height;

            int maxTop = Math.Max(
                0,
                playfield.ClientSize.Height - playerOnePaddle.Height
            );
            playerOnePaddle.Top = Math.Clamp(playerOnePaddle.Top, 0, maxTop);
            playerTwoPaddle.Top = Math.Clamp(playerTwoPaddle.Top, 0, maxTop);
        }

        private void CenterPaddles()
        {
            int top = Math.Max(
                0,
                (playfield.ClientSize.Height - playerOnePaddle.Height) / 2
            );
            playerOnePaddle.Top = top;
            playerTwoPaddle.Top = top;
            Playfield_Resize(this, EventArgs.Empty);
        }

        private void BackgroundInput_KeyStateChanged(
            object? sender,
            PingPongBackgroundKeyEventArgs e)
        {
            if (!communicationAvailable || IsDisposed)
            {
                return;
            }

            SetMovementKeyState(e.Key, e.IsDown);
        }

        private bool IsAssignedMovementKey(Keys key)
        {
            return isPlayerOne
                ? key == Keys.W || key == Keys.S
                : key == Keys.Up || key == Keys.Down;
        }

        private void SetMovementKeyState(Keys key, bool isDown)
        {
            if (!IsAssignedMovementKey(key))
            {
                return;
            }

            if (key == Keys.W || key == Keys.Up)
            {
                moveUpPressed = isDown;
            }
            else if (key == Keys.S || key == Keys.Down)
            {
                moveDownPressed = isDown;
            }
        }

        private void MovementTimer_Tick(object? sender, EventArgs e)
        {
            if (!communicationAvailable)
            {
                return;
            }

            int oldTop = localPaddle.Top;
            int maxTop = Math.Max(
                0,
                playfield.ClientSize.Height - localPaddle.Height
            );

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
            if (message.Type != PingPongMultiplayerMessageTypes.PlayerMovement ||
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

            int maxTop = Math.Max(
                0,
                playfield.ClientSize.Height - remotePaddle.Height
            );

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

            BeginInvoke(new Action(() =>
            {
                if (!IsDisposed)
                {
                    remotePaddle.Top = remoteTop;
                }
            }));
        }

        private void Connection_Disconnected(object? sender, EventArgs e)
        {
            communicationAvailable = false;
            movementPending = false;
            moveUpPressed = false;
            moveDownPressed = false;
            backgroundInput.Stop();

            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            BeginInvoke(new Action(() =>
            {
                if (!IsDisposed)
                {
                    movementTimer.Stop();
                    statusLabel.Text = "Multiplayer-Verbindung wurde getrennt.";
                }
            }));
        }

        private void PingPongMultiplayerGameForm_PreviewKeyDown(
            object? sender,
            PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.W ||
                e.KeyCode == Keys.S ||
                e.KeyCode == Keys.Up ||
                e.KeyCode == Keys.Down)
            {
                e.IsInputKey = true;
            }
        }

        private void PingPongMultiplayerGameForm_KeyDown(
            object? sender,
            KeyEventArgs e)
        {
            if (!communicationAvailable || !IsAssignedMovementKey(e.KeyCode))
            {
                return;
            }

            SetMovementKeyState(e.KeyCode, true);
            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        private void PingPongMultiplayerGameForm_KeyUp(
            object? sender,
            KeyEventArgs e)
        {
            if (!IsAssignedMovementKey(e.KeyCode))
            {
                return;
            }

            SetMovementKeyState(e.KeyCode, false);
            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        private async void PingPongMultiplayerGameForm_FormClosed(
            object? sender,
            FormClosedEventArgs e)
        {
            communicationAvailable = false;
            movementPending = false;
            moveUpPressed = false;
            moveDownPressed = false;
            movementTimer.Stop();
            movementTimer.Dispose();

            backgroundInput.KeyStateChanged -= BackgroundInput_KeyStateChanged;
            backgroundInput.Dispose();

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
