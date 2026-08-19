namespace SchulApp
{
    public sealed class PingPongJoinGameDialog : CustomForm
    {
        private readonly TextBox gameCodeTextBox;
        private readonly Button joinButton;
        private readonly Button cancelButton;

        public string GameCode { get; private set; } = string.Empty;

        public PingPongJoinGameDialog()
        {
            Text = "SchulApp - Join Ping Pong Game";
            StartPosition = FormStartPosition.Manual;
            ClientSize = new Size(520, 280);
            MinimumSize = new Size(520, 280);
            MaximumSize = new Size(520, 280);

            Label titleLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Location = new Point(45, 55),
                Size = new Size(430, 40),
                Text = "Join Game",
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label codeLabel = new Label
            {
                AutoSize = false,
                Location = new Point(95, 115),
                Size = new Size(330, 25),
                Text = "Game Code",
                TextAlign = ContentAlignment.MiddleLeft
            };

            gameCodeTextBox = new TextBox
            {
                CharacterCasing = CharacterCasing.Upper,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(95, 145),
                MaxLength = 6,
                Size = new Size(330, 36),
                TextAlign = HorizontalAlignment.Center
            };

            joinButton = new Button
            {
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(95, 205),
                Size = new Size(155, 38),
                Text = "Join Game",
                UseVisualStyleBackColor = true
            };

            cancelButton = new Button
            {
                Location = new Point(270, 205),
                Size = new Size(155, 38),
                Text = "Abbrechen",
                UseVisualStyleBackColor = true
            };

            joinButton.Click += JoinButton_Click;
            cancelButton.Click += (_, _) =>
            {
                DialogResult = DialogResult.Cancel;
                Close();
            };

            AcceptButton = joinButton;
            CancelButton = cancelButton;

            Controls.Add(titleLabel);
            Controls.Add(codeLabel);
            Controls.Add(gameCodeTextBox);
            Controls.Add(joinButton);
            Controls.Add(cancelButton);

            ThemeManager.Anwenden(this);
        }

        private void JoinButton_Click(object? sender, EventArgs e)
        {
            string code = gameCodeTextBox.Text.Trim().ToUpperInvariant();

            if (code.Length != 6 || !code.All(char.IsLetterOrDigit))
            {
                MessageBox.Show(
                    this,
                    "Bitte gib einen gültigen 6-stelligen Game Code ein.",
                    "Join Game",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            GameCode = code;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
