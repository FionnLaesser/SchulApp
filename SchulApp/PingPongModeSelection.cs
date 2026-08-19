namespace SchulApp
{
    public sealed class PingPongModeSelection : CustomForm
    {
        private readonly Button samePcButton;
        private readonly Button multiplayerButton;
        private readonly Button backButton;

        public PingPongModeSelection()
        {
            Text = "SchulApp - Ping Pong";
            StartPosition = FormStartPosition.Manual;
            ClientSize = new Size(640, 390);
            MinimumSize = new Size(640, 390);

            Label titleLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                Location = new Point(55, 55),
                Size = new Size(530, 50),
                Text = "Ping Pong",
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label descriptionLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(70, 110),
                Size = new Size(500, 45),
                Text = "Wähle aus, wie du spielen möchtest.",
                TextAlign = ContentAlignment.MiddleCenter
            };

            samePcButton = CreateModeButton(
                "Same PC",
                "Zwei Spieler verwenden dieselbe Tastatur.",
                new Point(95, 175)
            );

            multiplayerButton = CreateModeButton(
                "Multiplayer",
                "Mit einem zweiten SchulApp-Client spielen.",
                new Point(335, 175)
            );

            backButton = new Button
            {
                Location = new Point(245, 315),
                Size = new Size(150, 38),
                Text = "Zurück",
                UseVisualStyleBackColor = true
            };

            samePcButton.Click += SamePcButton_Click;
            multiplayerButton.Click += MultiplayerButton_Click;
            backButton.Click += (_, _) => Close();

            Controls.Add(titleLabel);
            Controls.Add(descriptionLabel);
            Controls.Add(samePcButton);
            Controls.Add(multiplayerButton);
            Controls.Add(backButton);

            ThemeManager.Anwenden(this);
        }

        private static Button CreateModeButton(
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

        private void SamePcButton_Click(object? sender, EventArgs e)
        {
            OeffneBereich(new PingPong());
        }

        private void MultiplayerButton_Click(object? sender, EventArgs e)
        {
            OeffneBereich(new PingPongMultiplayerMenu());
        }
    }
}
