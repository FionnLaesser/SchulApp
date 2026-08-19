using SchulApp.Services;

namespace SchulApp
{
    public sealed class PingPongJoinGameDialog : CustomForm
    {
        private readonly TextBox hostTextBox;
        private readonly TextBox gameCodeTextBox;
        private readonly Button joinButton;
        private readonly Button cancelButton;

        public string HostAddress { get; private set; } = "localhost";
        public string GameCode { get; private set; } = string.Empty;

        public PingPongJoinGameDialog()
        {
            Text = "SchulApp - Join Ping Pong Game";
            StartPosition = FormStartPosition.Manual;
            ClientSize = new Size(520, 360);
            MinimumSize = new Size(520, 360);

            Label titleLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                Location = new Point(45, 50),
                Size = new Size(430, 40),
                Text = "Join Game",
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label hostLabel = new Label
            {
                AutoSize = false,
                Location = new Point(95, 105),
                Size = new Size(330, 25),
                Text = "Host / IP-Adresse",
                TextAlign = ContentAlignment.MiddleLeft
            };

            hostTextBox = new TextBox
            {
                Font = new Font("Segoe UI", 11F),
                Location = new Point(95, 132),
                Size = new Size(330, 32),
                Text = "localhost",
                TextAlign = HorizontalAlignment.Center
            };

            Label hostHelpLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 8.5F),
                Location = new Point(95, 166),
                Size = new Size(330, 24),
                Text = $"Same PC: localhost | LAN: z.B. 192.168.1.42 | Port {PingPongMultiplayerEndpoint.DefaultPort}",
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label codeLabel = new Label
            {
                AutoSize = false,
                Location = new Point(95, 200),
                Size = new Size(330, 25),
                Text = "Game Code",
                TextAlign = ContentAlignment.MiddleLeft
            };

            gameCodeTextBox = new TextBox
            {
                CharacterCasing = CharacterCasing.Upper,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(95, 227),
                MaxLength = 6,
                Size = new Size(330, 36),
                TextAlign = HorizontalAlignment.Center
            };

            joinButton = new Button
            {
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(95, 290),
                Size = new Size(155, 38),
                Text = "Join Game",
                UseVisualStyleBackColor = true
            };

            cancelButton = new Button
            {
                Location = new Point(270, 290),
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
            Controls.Add(hostLabel);
            Controls.Add(hostTextBox);
            Controls.Add(hostHelpLabel);
            Controls.Add(codeLabel);
            Controls.Add(gameCodeTextBox);
            Controls.Add(joinButton);
            Controls.Add(cancelButton);

            ThemeManager.Anwenden(this);
        }

        private void JoinButton_Click(object? sender, EventArgs e)
        {
            string host = hostTextBox.Text.Trim();
            string code = gameCodeTextBox.Text.Trim().ToUpperInvariant();

            if (!PingPongMultiplayerEndpoint.TryNormalizeHost(host, out _))
            {
                MessageBox.Show(
                    this,
                    "Bitte gib localhost, eine gültige IP-Adresse oder einen Hostnamen ein.",
                    "Join Game",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

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

            HostAddress = host;
            GameCode = code;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
