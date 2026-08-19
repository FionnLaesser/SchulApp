namespace SchulApp
{
    public sealed class PingPongMultiplayerMenu : CustomForm
    {
        private readonly Button createGameButton;
        private readonly Button joinGameButton;
        private readonly Button backButton;

        public PingPongMultiplayerMenu()
        {
            Text = "SchulApp - Ping Pong Multiplayer";
            StartPosition = FormStartPosition.Manual;
            ClientSize = new Size(640, 390);
            MinimumSize = new Size(640, 390);

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
                Text = "Erstelle ein neues Spiel oder tritt einem bestehenden Spiel bei.",
                TextAlign = ContentAlignment.MiddleCenter
            };

            createGameButton = CreateActionButton(
                "Create Game",
                "Neue Lobby erstellen",
                new Point(95, 175)
            );

            joinGameButton = CreateActionButton(
                "Join Game",
                "Bestehender Lobby beitreten",
                new Point(335, 175)
            );

            backButton = new Button
            {
                Location = new Point(245, 315),
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

        private void CreateGameButton_Click(object? sender, EventArgs e)
        {
            MessageBox.Show(
                this,
                "Die Lobby-Erstellung wird im nächsten Projektschritt ergänzt.",
                "Create Game",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void JoinGameButton_Click(object? sender, EventArgs e)
        {
            MessageBox.Show(
                this,
                "Das Beitreten zu einer Lobby wird im nächsten Projektschritt ergänzt.",
                "Join Game",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}
