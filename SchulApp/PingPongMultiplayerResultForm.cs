using System.Linq;
using System.Windows.Forms;

namespace SchulApp
{
    public sealed class PingPongMultiplayerResultForm : Form
    {
        private readonly Form gameForm;

        private PingPongMultiplayerResultForm(
            Form gameForm,
            string title,
            string message,
            string? scoreText)
        {
            this.gameForm = gameForm;

            Text = "SchulApp - Ping Pong Ergebnis";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(520, 300);

            Label titleLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 26F, FontStyle.Bold),
                Location = new Point(35, 30),
                Size = new Size(450, 60),
                Text = title,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label messageLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 11F),
                Location = new Point(45, 100),
                Size = new Size(430, 55),
                Text = message,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label scoreLabel = new Label
            {
                AutoSize = false,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(45, 160),
                Size = new Size(430, 40),
                Text = scoreText ?? string.Empty,
                TextAlign = ContentAlignment.MiddleCenter,
                Visible = !string.IsNullOrWhiteSpace(scoreText)
            };

            Button returnButton = new Button
            {
                Location = new Point(155, 225),
                Size = new Size(210, 44),
                Text = "Zurück zur Lobby",
                UseVisualStyleBackColor = true
            };
            returnButton.Click += ReturnButton_Click;

            Controls.Add(titleLabel);
            Controls.Add(messageLabel);
            Controls.Add(scoreLabel);
            Controls.Add(returnButton);

            ThemeManager.Anwenden(this);
        }

        public static void ShowGameResult(
            int localUserId,
            int winnerUserId,
            int playerOneScore,
            int playerTwoScore)
        {
            PingPongMultiplayerGameForm? gameForm = FindGameForm();

            if (gameForm == null || HasOpenResultForm())
            {
                return;
            }

            bool won = localUserId == winnerUserId;
            string title = won ? "Gewonnen!" : "Verloren";
            string message = won
                ? "Du hast das Ping Pong Match gewonnen."
                : "Du hast das Ping Pong Match verloren.";
            string score = $"Endstand: {playerOneScore} : {playerTwoScore}";

            using PingPongMultiplayerResultForm resultForm =
                new PingPongMultiplayerResultForm(
                    gameForm,
                    title,
                    message,
                    score
                );

            resultForm.ShowDialog(gameForm);
        }

        public static void ShowOpponentDisconnected()
        {
            PingPongMultiplayerGameForm? gameForm = FindGameForm();

            if (gameForm == null || HasOpenResultForm())
            {
                return;
            }

            using PingPongMultiplayerResultForm resultForm =
                new PingPongMultiplayerResultForm(
                    gameForm,
                    "Spiel beendet",
                    "Der andere Spieler hat das Spiel verlassen. Das Match wurde gestoppt.",
                    null
                );

            resultForm.ShowDialog(gameForm);
        }

        private static PingPongMultiplayerGameForm? FindGameForm()
        {
            return Application.OpenForms
                .OfType<PingPongMultiplayerGameForm>()
                .FirstOrDefault(form => !form.IsDisposed);
        }

        private static bool HasOpenResultForm()
        {
            return Application.OpenForms
                .OfType<PingPongMultiplayerResultForm>()
                .Any(form => !form.IsDisposed);
        }

        private void ReturnButton_Click(object? sender, EventArgs e)
        {
            Close();

            if (!gameForm.IsDisposed)
            {
                gameForm.Close();
            }
        }
    }
}
