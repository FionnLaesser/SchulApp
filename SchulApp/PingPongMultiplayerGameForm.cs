using SchulApp.Models;
using SchulApp.Services;
using System.Text.Json;

namespace SchulApp
{
    public sealed class PingPongMultiplayerGameForm : CustomForm
    {
        private const int PaddleSpeed = 7;
        private const int BallSize = 40;

        private readonly PingPongLobbyModel lobby;
        private readonly PingPongMultiplayerConnection connection =
            new PingPongMultiplayerConnection();
        private readonly PingPongBackgroundKeyboardInput backgroundInput;
        private readonly PingPongMultiplayerGameEngine gameEngine =
            new PingPongMultiplayerGameEngine();
        private readonly PingPongApiService pingPongApiService =
            new PingPongApiService();
        private readonly System.Windows.Forms.Timer movementTimer;
        private readonly System.Windows.Forms.Timer gameTimer;
        private readonly Panel playfield;
        private readonly Panel playerOnePaddle;
        private readonly Panel playerTwoPaddle;
        private readonly Panel centerLine;
        private readonly Panel ball;
        private readonly Panel localPaddle;
        private readonly Panel remotePaddle;
        private readonly Label scoreLabel;
        private readonly Label statusLabel;
        private readonly bool isPlayerOne;
        private readonly int remoteUserId;

        private bool moveUpPressed;
        private bool moveDownPressed;
        private bool movementPending;
        private bool movementSendLoopRunning;
        private bool ballSendRunning;
        private bool communicationAvailable;
        private bool gameRunning;
        private bool resultSaved;
        private double pendingNormalizedTop;
        private long localSequence;
        private long lastRemoteMovementSequence = -1;
        private long lastBallSequence = -1;
        private long lastScoreSequence = -1;
        private long lastStartSequence = -1;
        private long lastEndSequence = -1;
        private int playerOneScore;
        private int playerTwoScore;
        private int authoritativeTickCounter;

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

            scoreLabel = new Label
            {
                Anchor = AnchorStyles.Top,
                AutoSize = false,
                Font = new Font("Segoe UI", 24F, FontStyle.Bold),
                Location = new Point(520, 32),
                Size = new Size(160, 50),
                Text = "0 : 0",
                TextAlign = ContentAlignment.MiddleCenter
            };

            statusLabel = new Label
            {
                AutoSize = false,
                Location = new Point(20, 82),
                Size = new Size(800, 20),
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

            ball = new Panel
            {
                Location = new Point(580, 302),
                Size = new Size(BallSize, BallSize)
            };

            playfield.Controls.Add(centerLine);
            playfield.Controls.Add(ball);
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
            Controls.Add(scoreLabel);
            Controls.Add(statusLabel);
            Controls.Add(backButton);
            Controls.Add(playfield);

            movementTimer = new System.Windows.Forms.Timer
            {
                Interval = 16
            };
            movementTimer.Tick += MovementTimer_Tick;

            gameTimer = new System.Windows.Forms.Timer
            {
                Interval = 16
            };
            gameTimer.Tick += GameTimer_Tick;

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
            ball.BackColor = Color.Black;
            centerLine.BackColor = Color.LightGray;

            CenterGameObjects();
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
                }
                catch (Exception ex)
                {
                    statusLabel.Text =
                        "Hintergrund-Steuerung nicht verfügbar: " + ex.Message;
                }

                if (isPlayerOne)
                {
                    await StartAuthoritativeGameAsync();
                }
                else
                {
                    statusLabel.Text =
                        "Verbunden. Warte auf den synchronisierten Spielstart von Player 1...";
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

        private async Task StartAuthoritativeGameAsync()
        {
            gameEngine.Start(
                playfield.ClientSize.Width,
                playfield.ClientSize.Height,
                ball.Width,
                ball.Height
            );

            playerOneScore = 0;
            playerTwoScore = 0;
            resultSaved = false;
            gameRunning = true;
            authoritativeTickCounter = 0;
            UpdateScoreDisplay();
            ApplyEngineBallPosition();

            statusLabel.Text =
                "Spiel läuft. Player 1 berechnet den offiziellen Ball und Score.";

            await connection.SendAsync(
                PingPongMultiplayerMessageTypes.GameStart,
                new PingPongGameStartPayload
                {
                    PlayerOneScore = 0,
                    PlayerTwoScore = 0
                },
                ++localSequence
            );

            await SendScoreAsync();
            await SendBallStateAsync();
            gameTimer.Start();
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            if (!isPlayerOne || !communicationAvailable || !gameRunning)
            {
                return;
            }

            PingPongGameTickResult result = gameEngine.Tick(
                playerOnePaddle.Bounds,
                playerTwoPaddle.Bounds,
                playfield.ClientSize.Width,
                playfield.ClientSize.Height,
                ball.Width,
                ball.Height
            );

            ApplyEngineBallPosition();

            if (result.ScoreChanged)
            {
                playerOneScore = gameEngine.PlayerOneScore;
                playerTwoScore = gameEngine.PlayerTwoScore;
                UpdateScoreDisplay();
                _ = SendScoreAsync();
            }

            if (result.GameEnded)
            {
                gameRunning = false;
                gameTimer.Stop();
                _ = FinishAuthoritativeGameAsync(result.WinnerPlayer);
                return;
            }

            authoritativeTickCounter++;

            if (result.BallReset || authoritativeTickCounter % 2 == 0)
            {
                _ = SendBallStateAsync();
            }
        }

        private async Task SendBallStateAsync()
        {
            if (!isPlayerOne || !communicationAvailable || ballSendRunning)
            {
                return;
            }

            ballSendRunning = true;

            try
            {
                int maxLeft = Math.Max(0, playfield.ClientSize.Width - ball.Width);
                int maxTop = Math.Max(0, playfield.ClientSize.Height - ball.Height);

                await connection.SendAsync(
                    PingPongMultiplayerMessageTypes.BallState,
                    new PingPongBallStatePayload
                    {
                        NormalizedLeft = PingPongBallStatePosition.Normalize(
                            ball.Left,
                            maxLeft
                        ),
                        NormalizedTop = PingPongBallStatePosition.Normalize(
                            ball.Top,
                            maxTop
                        ),
                        DirectionX = gameEngine.DirectionX,
                        DirectionY = gameEngine.DirectionY
                    },
                    ++localSequence
                );
            }
            catch (Exception ex)
            {
                StopCommunication("Ball-Synchronisierung gestoppt: " + ex.Message);
            }
            finally
            {
                ballSendRunning = false;
            }
        }

        private async Task SendScoreAsync()
        {
            if (!isPlayerOne || !communicationAvailable)
            {
                return;
            }

            try
            {
                await connection.SendAsync(
                    PingPongMultiplayerMessageTypes.Score,
                    new PingPongScorePayload
                    {
                        PlayerOneScore = playerOneScore,
                        PlayerTwoScore = playerTwoScore
                    },
                    ++localSequence
                );
            }
            catch (Exception ex)
            {
                StopCommunication("Score-Synchronisierung gestoppt: " + ex.Message);
            }
        }

        private async Task FinishAuthoritativeGameAsync(int winnerPlayer)
        {
            int winnerUserId = winnerPlayer == 1
                ? lobby.HostUserId
                : lobby.GuestUserId!.Value;

            try
            {
                await SendScoreAsync();
                await connection.SendAsync(
                    PingPongMultiplayerMessageTypes.GameEnd,
                    new PingPongGameEndPayload
                    {
                        WinnerUserId = winnerUserId,
                        PlayerOneScore = playerOneScore,
                        PlayerTwoScore = playerTwoScore
                    },
                    ++localSequence
                );
            }
            catch (Exception ex)
            {
                StopCommunication("Spielende konnte nicht synchronisiert werden: " + ex.Message);
                return;
            }

            ShowWinner(winnerUserId);

            if (resultSaved)
            {
                return;
            }

            resultSaved = true;

            try
            {
                await pingPongApiService.SpielSpeichernAsync(
                    lobby.HostUserId,
                    lobby.GuestUserId!.Value,
                    playerOneScore,
                    playerTwoScore
                );
                statusLabel.Text += " Ergebnis gespeichert.";
            }
            catch
            {
                statusLabel.Text += " Ergebnis konnte nicht gespeichert werden.";
            }
        }

        private void ApplyEngineBallPosition()
        {
            ball.Left = gameEngine.BallLeft;
            ball.Top = gameEngine.BallTop;
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

            int paddleMaxTop = Math.Max(
                0,
                playfield.ClientSize.Height - playerOnePaddle.Height
            );
            playerOnePaddle.Top = Math.Clamp(playerOnePaddle.Top, 0, paddleMaxTop);
            playerTwoPaddle.Top = Math.Clamp(playerTwoPaddle.Top, 0, paddleMaxTop);

            int ballMaxLeft = Math.Max(0, playfield.ClientSize.Width - ball.Width);
            int ballMaxTop = Math.Max(0, playfield.ClientSize.Height - ball.Height);
            ball.Left = Math.Clamp(ball.Left, 0, ballMaxLeft);
            ball.Top = Math.Clamp(ball.Top, 0, ballMaxTop);
        }

        private void CenterGameObjects()
        {
            int paddleTop = Math.Max(
                0,
                (playfield.ClientSize.Height - playerOnePaddle.Height) / 2
            );
            playerOnePaddle.Top = paddleTop;
            playerTwoPaddle.Top = paddleTop;
            ball.Left = Math.Max(0, (playfield.ClientSize.Width - ball.Width) / 2);
            ball.Top = Math.Max(0, (playfield.ClientSize.Height - ball.Height) / 2);
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

            if (!movementSendLoopRunning)
            {
                _ = SendPendingMovementsAsync();
            }
        }

        private async Task SendPendingMovementsAsync()
        {
            if (movementSendLoopRunning)
            {
                return;
            }

            movementSendLoopRunning = true;

            try
            {
                while (movementPending && communicationAvailable)
                {
                    movementPending = false;
                    double normalizedTop = pendingNormalizedTop;

                    await connection.SendAsync(
                        PingPongMultiplayerMessageTypes.PlayerMovement,
                        new PingPongPlayerMovementPayload
                        {
                            NormalizedTop = normalizedTop
                        },
                        ++localSequence
                    );
                }
            }
            catch (Exception ex)
            {
                StopCommunication(
                    "Paddle-Synchronisierung gestoppt: " + ex.Message
                );
            }
            finally
            {
                movementSendLoopRunning = false;
            }
        }

        private void Connection_MessageReceived(
            object? sender,
            PingPongMultiplayerMessage message)
        {
            switch (message.Type)
            {
                case PingPongMultiplayerMessageTypes.PlayerMovement:
                    HandleRemoteMovement(message);
                    break;

                case PingPongMultiplayerMessageTypes.GameStart:
                    HandleGameStart(message);
                    break;

                case PingPongMultiplayerMessageTypes.BallState:
                    HandleBallState(message);
                    break;

                case PingPongMultiplayerMessageTypes.Score:
                    HandleScore(message);
                    break;

                case PingPongMultiplayerMessageTypes.GameEnd:
                    HandleGameEnd(message);
                    break;
            }
        }

        private void HandleRemoteMovement(PingPongMultiplayerMessage message)
        {
            if (message.SenderUserId != remoteUserId ||
                message.Sequence <= lastRemoteMovementSequence)
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

            lastRemoteMovementSequence = message.Sequence;
            InvokeUi(() => remotePaddle.Top = remoteTop);
        }

        private void HandleGameStart(PingPongMultiplayerMessage message)
        {
            if (isPlayerOne ||
                message.SenderUserId != lobby.HostUserId ||
                message.Sequence <= lastStartSequence)
            {
                return;
            }

            PingPongGameStartPayload? payload = TryDeserialize<PingPongGameStartPayload>(
                message
            );

            if (payload == null || !IsValidScore(
                payload.PlayerOneScore,
                payload.PlayerTwoScore))
            {
                return;
            }

            lastStartSequence = message.Sequence;

            InvokeUi(() =>
            {
                playerOneScore = payload.PlayerOneScore;
                playerTwoScore = payload.PlayerTwoScore;
                gameRunning = true;
                UpdateScoreDisplay();
                statusLabel.Text = "Spiel läuft. Ball und Score werden von Player 1 synchronisiert.";
            });
        }

        private void HandleBallState(PingPongMultiplayerMessage message)
        {
            if (isPlayerOne ||
                message.SenderUserId != lobby.HostUserId ||
                message.Sequence <= lastBallSequence)
            {
                return;
            }

            PingPongBallStatePayload? payload = TryDeserialize<PingPongBallStatePayload>(
                message
            );

            if (payload == null ||
                Math.Abs(payload.DirectionX) != 1 ||
                Math.Abs(payload.DirectionY) != 1)
            {
                return;
            }

            int maxLeft = Math.Max(0, playfield.ClientSize.Width - ball.Width);
            int maxTop = Math.Max(0, playfield.ClientSize.Height - ball.Height);

            if (!PingPongBallStatePosition.TryDenormalize(
                    payload.NormalizedLeft,
                    maxLeft,
                    out int left) ||
                !PingPongBallStatePosition.TryDenormalize(
                    payload.NormalizedTop,
                    maxTop,
                    out int top))
            {
                return;
            }

            lastBallSequence = message.Sequence;
            InvokeUi(() =>
            {
                ball.Left = left;
                ball.Top = top;
            });
        }

        private void HandleScore(PingPongMultiplayerMessage message)
        {
            if (isPlayerOne ||
                message.SenderUserId != lobby.HostUserId ||
                message.Sequence <= lastScoreSequence)
            {
                return;
            }

            PingPongScorePayload? payload = TryDeserialize<PingPongScorePayload>(message);

            if (payload == null || !IsValidScore(
                payload.PlayerOneScore,
                payload.PlayerTwoScore))
            {
                return;
            }

            lastScoreSequence = message.Sequence;
            InvokeUi(() =>
            {
                playerOneScore = payload.PlayerOneScore;
                playerTwoScore = payload.PlayerTwoScore;
                UpdateScoreDisplay();
            });
        }

        private void HandleGameEnd(PingPongMultiplayerMessage message)
        {
            if (isPlayerOne ||
                message.SenderUserId != lobby.HostUserId ||
                message.Sequence <= lastEndSequence)
            {
                return;
            }

            PingPongGameEndPayload? payload = TryDeserialize<PingPongGameEndPayload>(message);

            if (payload == null ||
                !IsValidScore(payload.PlayerOneScore, payload.PlayerTwoScore) ||
                (payload.WinnerUserId != lobby.HostUserId &&
                 payload.WinnerUserId != lobby.GuestUserId))
            {
                return;
            }

            lastEndSequence = message.Sequence;
            InvokeUi(() =>
            {
                gameRunning = false;
                playerOneScore = payload.PlayerOneScore;
                playerTwoScore = payload.PlayerTwoScore;
                UpdateScoreDisplay();
                ShowWinner(payload.WinnerUserId);
            });
        }

        private static TPayload? TryDeserialize<TPayload>(
            PingPongMultiplayerMessage message)
            where TPayload : class
        {
            try
            {
                return message.Payload.Deserialize<TPayload>();
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static bool IsValidScore(int playerOne, int playerTwo)
        {
            return playerOne >= 0 &&
                playerTwo >= 0 &&
                playerOne <= PingPongMultiplayerGameEngine.MaxScore &&
                playerTwo <= PingPongMultiplayerGameEngine.MaxScore;
        }

        private void UpdateScoreDisplay()
        {
            scoreLabel.Text = $"{playerOneScore} : {playerTwoScore}";
        }

        private void ShowWinner(int winnerUserId)
        {
            string winner = winnerUserId == lobby.HostUserId
                ? lobby.HostUsername
                : lobby.GuestUsername ?? "Player 2";

            statusLabel.Text =
                $"Spiel beendet. Gewinner: {winner}. Ergebnis: {playerOneScore} : {playerTwoScore}.";
        }

        private void InvokeUi(Action action)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            BeginInvoke(new Action(() =>
            {
                if (!IsDisposed)
                {
                    action();
                }
            }));
        }

        private void StopCommunication(string message)
        {
            communicationAvailable = false;
            gameRunning = false;
            movementPending = false;
            moveUpPressed = false;
            moveDownPressed = false;
            backgroundInput.Stop();

            InvokeUi(() =>
            {
                movementTimer.Stop();
                gameTimer.Stop();
                statusLabel.Text = message;
            });
        }

        private void Connection_Disconnected(object? sender, EventArgs e)
        {
            StopCommunication("Multiplayer-Verbindung wurde getrennt.");
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
            gameRunning = false;
            movementPending = false;
            moveUpPressed = false;
            moveDownPressed = false;
            movementTimer.Stop();
            gameTimer.Stop();
            movementTimer.Dispose();
            gameTimer.Dispose();

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
            }
        }
    }
}
