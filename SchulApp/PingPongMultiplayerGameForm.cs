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
        private readonly PictureBox ball;
        private readonly Panel localPaddle;
        private readonly Panel remotePaddle;
        private readonly Label scoreLabel;
        private readonly Label statusLabel;
        private readonly NoFocusButton pauseButton;
        private readonly Panel pausePanel;
        private readonly Label pauseTextLabel;
        private readonly Button continueButton;
        private readonly Button leaveButton;
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
        private bool isPaused;
        private bool pauseRequestPending;
        private double pendingNormalizedTop;
        private long localSequence;
        private long lastRemoteMovementSequence = -1;
        private long lastBallSequence = -1;
        private long lastScoreSequence = -1;
        private long lastStartSequence = -1;
        private long lastEndSequence = -1;
        private long lastPauseRequestSequence = -1;
        private long lastPauseStateSequence = -1;
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
                    ? "Player 1: W / S | Pause: Q"
                    : "Player 2: Pfeil hoch / Pfeil runter | Pause: Q",
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

            pauseButton = new NoFocusButton
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(980, 48),
                Size = new Size(95, 23),
                Text = "Pause",
                TabStop = false,
                UseVisualStyleBackColor = true
            };
            pauseButton.Click += (_, _) => _ = RequestPauseChangeAsync(!isPaused);

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

            ball = new PictureBox
            {
                BackColor = Color.Transparent,
                Location = new Point(580, 302),
                Size = new Size(BallSize, BallSize),
                SizeMode = PictureBoxSizeMode.Zoom,
                TabStop = false
            };
            LoadBallImage();

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

            pausePanel = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(430, 260),
                Visible = false
            };

            Label pauseTitleLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                Location = new Point(35, 28),
                Size = new Size(360, 48),
                Text = "Spiel pausiert",
                TextAlign = ContentAlignment.MiddleCenter
            };

            pauseTextLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(40, 85),
                Size = new Size(350, 55),
                Text = "Das Multiplayer-Spiel ist für beide Spieler pausiert.",
                TextAlign = ContentAlignment.MiddleCenter
            };

            continueButton = new Button
            {
                Location = new Point(45, 175),
                Size = new Size(155, 42),
                Text = "Fortsetzen",
                UseVisualStyleBackColor = true
            };
            continueButton.Click += (_, _) => _ = RequestPauseChangeAsync(false);

            leaveButton = new Button
            {
                Location = new Point(230, 175),
                Size = new Size(155, 42),
                Text = "Spiel verlassen",
                UseVisualStyleBackColor = true
            };
            leaveButton.Click += (_, _) => Close();

            pausePanel.Controls.Add(pauseTitleLabel);
            pausePanel.Controls.Add(pauseTextLabel);
            pausePanel.Controls.Add(continueButton);
            pausePanel.Controls.Add(leaveButton);

            Controls.Add(controlsLabel);
            Controls.Add(playerOneLabel);
            Controls.Add(playerTwoLabel);
            Controls.Add(scoreLabel);
            Controls.Add(statusLabel);
            Controls.Add(pauseButton);
            Controls.Add(backButton);
            Controls.Add(playfield);
            Controls.Add(pausePanel);

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
            Resize += PingPongMultiplayerGameForm_Resize;
            playfield.Resize += Playfield_Resize;

            backgroundInput.KeyStateChanged += BackgroundInput_KeyStateChanged;
            connection.MessageReceived += Connection_MessageReceived;
            connection.Disconnected += Connection_Disconnected;

            ThemeManager.Anwenden(this);
            playfield.BackColor = Color.White;
            playerOnePaddle.BackColor = Color.Black;
            playerTwoPaddle.BackColor = Color.Black;
            centerLine.BackColor = Color.LightGray;

            CenterGameObjects();
            PositionPausePanel();
        }

        private void LoadBallImage()
        {
            string imagePath = Path.Combine(
                AppContext.BaseDirectory,
                "images",
                "ball.png"
            );

            if (!File.Exists(imagePath))
            {
                ball.BackColor = Color.Black;
                return;
            }

            using Image source = Image.FromFile(imagePath);
            ball.Image = new Bitmap(source);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;

            if (keyCode == Keys.Q)
            {
                if (communicationAvailable && gameRunning && !pauseRequestPending)
                {
                    _ = RequestPauseChangeAsync(!isPaused);
                }

                return true;
            }

            if (IsAssignedMovementKey(keyCode))
            {
                if (communicationAvailable && !isPaused)
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
            isPaused = false;
            authoritativeTickCounter = 0;
            UpdateScoreDisplay();
            ApplyEngineBallPosition();
            ApplyPauseVisualState(false);

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
            await SendPauseStateAsync(false);
            gameTimer.Start();
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            if (!isPlayerOne ||
                !communicationAvailable ||
                !gameRunning ||
                isPaused)
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
                pauseButton.Enabled = false;
                ApplyPauseVisualState(false);
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
            if (!isPlayerOne ||
                !communicationAvailable ||
                ballSendRunning ||
                isPaused)
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

        private async Task RequestPauseChangeAsync(bool shouldPause)
        {
            if (!communicationAvailable || !gameRunning || pauseRequestPending)
            {
                return;
            }

            pauseRequestPending = true;

            try
            {
                if (isPlayerOne)
                {
                    ApplyPauseState(shouldPause);
                    await SendPauseStateAsync(shouldPause);
                    pauseRequestPending = false;
                    return;
                }

                await connection.SendAsync(
                    PingPongMultiplayerMessageTypes.PauseRequest,
                    new PingPongPauseRequestPayload
                    {
                        IsPaused = shouldPause
                    },
                    ++localSequence
                );

                statusLabel.Text = shouldPause
                    ? "Pause wird angefordert..."
                    : "Fortsetzen wird angefordert...";
            }
            catch (Exception ex)
            {
                pauseRequestPending = false;
                StopCommunication("Pause-Anfrage fehlgeschlagen: " + ex.Message);
            }
        }

        private async Task SendPauseStateAsync(bool paused)
        {
            if (!isPlayerOne || !communicationAvailable)
            {
                return;
            }

            try
            {
                await connection.SendAsync(
                    PingPongMultiplayerMessageTypes.PauseState,
                    new PingPongPauseStatePayload
                    {
                        IsPaused = paused
                    },
                    ++localSequence
                );
            }
            catch (Exception ex)
            {
                StopCommunication("Pause-Synchronisierung gestoppt: " + ex.Message);
            }
        }

        private void ApplyPauseState(bool paused)
        {
            if (!gameRunning)
            {
                return;
            }

            isPaused = paused;
            pauseRequestPending = false;
            moveUpPressed = false;
            moveDownPressed = false;
            movementPending = false;

            if (paused)
            {
                movementTimer.Stop();
                gameTimer.Stop();
            }
            else
            {
                movementTimer.Start();

                if (isPlayerOne)
                {
                    gameTimer.Start();
                }
            }

            ApplyPauseVisualState(paused);
        }

        private void ApplyPauseVisualState(bool paused)
        {
            pausePanel.Visible = paused;
            pauseButton.Text = paused ? "Pausiert" : "Pause";
            pauseButton.Enabled = gameRunning && communicationAvailable && !paused;

            if (paused)
            {
                pauseTextLabel.Text =
                    "Das Multiplayer-Spiel ist für beide Spieler pausiert.\nBall, Schläger und Score bleiben unverändert.";
                continueButton.Enabled = communicationAvailable;
                pausePanel.BringToFront();
                statusLabel.Text = "Spiel pausiert.";
            }
            else if (gameRunning && communicationAvailable)
            {
                statusLabel.Text = isPlayerOne
                    ? "Spiel läuft. Player 1 berechnet den offiziellen Ball und Score."
                    : "Spiel läuft. Ball und Score werden von Player 1 synchronisiert.";
                ActiveControl = null;
                Focus();
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

        private void PingPongMultiplayerGameForm_Resize(object? sender, EventArgs e)
        {
            PositionPausePanel();
        }

        private void PositionPausePanel()
        {
            pausePanel.Left = Math.Max(0, (ClientSize.Width - pausePanel.Width) / 2);
            pausePanel.Top = Math.Max(105, (ClientSize.Height - pausePanel.Height) / 2);
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
            if (!communicationAvailable || IsDisposed || isPaused)
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
            if (!communicationAvailable || isPaused)
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
                while (movementPending && communicationAvailable && !isPaused)
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

                case PingPongMultiplayerMessageTypes.PauseRequest:
                    HandlePauseRequest(message);
                    break;

                case PingPongMultiplayerMessageTypes.PauseState:
                    HandlePauseState(message);
                    break;
            }
        }

        private void HandleRemoteMovement(PingPongMultiplayerMessage message)
        {
            if (isPaused ||
                message.SenderUserId != remoteUserId ||
                message.Sequence <= lastRemoteMovementSequence)
            {
                return;
            }

            PingPongPlayerMovementPayload? payload =
                TryDeserialize<PingPongPlayerMovementPayload>(message);

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

            PingPongGameStartPayload? payload =
                TryDeserialize<PingPongGameStartPayload>(message);

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
                isPaused = false;
                pauseButton.Enabled = true;
                movementTimer.Start();
                UpdateScoreDisplay();
                ApplyPauseVisualState(false);
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

            PingPongBallStatePayload? payload =
                TryDeserialize<PingPongBallStatePayload>(message);

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
                if (!isPaused)
                {
                    ball.Left = left;
                    ball.Top = top;
                }
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

            PingPongScorePayload? payload =
                TryDeserialize<PingPongScorePayload>(message);

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

        private void HandlePauseRequest(PingPongMultiplayerMessage message)
        {
            if (!isPlayerOne ||
                !gameRunning ||
                message.SenderUserId != remoteUserId ||
                message.Sequence <= lastPauseRequestSequence)
            {
                return;
            }

            PingPongPauseRequestPayload? payload =
                TryDeserialize<PingPongPauseRequestPayload>(message);

            if (payload == null)
            {
                return;
            }

            lastPauseRequestSequence = message.Sequence;

            InvokeUi(() =>
            {
                ApplyPauseState(payload.IsPaused);
                _ = SendPauseStateAsync(payload.IsPaused);
            });
        }

        private void HandlePauseState(PingPongMultiplayerMessage message)
        {
            if (isPlayerOne ||
                !gameRunning ||
                message.SenderUserId != lobby.HostUserId ||
                message.Sequence <= lastPauseStateSequence)
            {
                return;
            }

            PingPongPauseStatePayload? payload =
                TryDeserialize<PingPongPauseStatePayload>(message);

            if (payload == null)
            {
                return;
            }

            lastPauseStateSequence = message.Sequence;
            InvokeUi(() => ApplyPauseState(payload.IsPaused));
        }

        private void HandleGameEnd(PingPongMultiplayerMessage message)
        {
            if (isPlayerOne ||
                message.SenderUserId != lobby.HostUserId ||
                message.Sequence <= lastEndSequence)
            {
                return;
            }

            PingPongGameEndPayload? payload =
                TryDeserialize<PingPongGameEndPayload>(message);

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
                isPaused = false;
                movementTimer.Stop();
                pauseButton.Enabled = false;
                ApplyPauseVisualState(false);
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
            isPaused = false;
            pauseRequestPending = false;
            movementPending = false;
            moveUpPressed = false;
            moveDownPressed = false;
            backgroundInput.Stop();

            InvokeUi(() =>
            {
                movementTimer.Stop();
                gameTimer.Stop();
                pauseButton.Enabled = false;
                continueButton.Enabled = false;
                pausePanel.Visible = false;
                statusLabel.Text = message;
            });
        }

        private void Connection_Disconnected(object? sender, EventArgs e)
        {
            StopCommunication(
                PingPongMultiplayerError.GetDisconnectMessage(false)
            );
        }

        private void PingPongMultiplayerGameForm_PreviewKeyDown(
            object? sender,
            PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Q ||
                e.KeyCode == Keys.W ||
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
            if (e.KeyCode == Keys.Q)
            {
                if (communicationAvailable && gameRunning && !pauseRequestPending)
                {
                    _ = RequestPauseChangeAsync(!isPaused);
                }

                e.SuppressKeyPress = true;
                e.Handled = true;
                return;
            }

            if (!communicationAvailable ||
                isPaused ||
                !IsAssignedMovementKey(e.KeyCode))
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
            isPaused = false;
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

            ball.Image?.Dispose();

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
