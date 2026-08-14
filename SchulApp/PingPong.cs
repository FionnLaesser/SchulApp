using System;
using System.Drawing;
using System.Windows.Forms;

namespace SchulApp
{
    public partial class PingPong : CustomForm
    {
        private int ballSpeedX = 5;
        private int ballSpeedY = 5;

        private int spielerSpeed = 7;

        private bool hochGedruecktLinks = false;
        private bool runterGedruecktLinks = false;
        private bool hochGedruecktRechts = false;
        private bool runterGedruecktRechts = false;

        public PingPong()
        {
            InitializeComponent();

            ThemeManager.Anwenden(this);

            panelLinks.BackColor = Color.Black;
            panelRechts.BackColor = Color.Black;

            KeyPreview = true;

            KeyDown += PingPong_KeyDown;
            KeyUp += PingPong_KeyUp;

            gameTimer.Interval = 16;
            gameTimer.Start();
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            BallBewegen();
            SpielerBewegenLinks();
            SpielerBewegenRechts();
        }

        private void SpielerBewegenLinks()
        {
            if (hochGedruecktLinks && panelLinks.Top > 0)
            {
                panelLinks.Top -= spielerSpeed;
            }

            if (runterGedruecktLinks &&
                panelLinks.Bottom < ClientSize.Height)
            {
                panelLinks.Top += spielerSpeed;
            }
        }

        private void SpielerBewegenRechts()
        {
            if (hochGedruecktRechts && panelRechts.Top > 0)
            {
                panelRechts.Top -= spielerSpeed;
            }

            if (runterGedruecktRechts &&
                panelRechts.Bottom < ClientSize.Height)
            {
                panelRechts.Top += spielerSpeed;
            }
        }

        private void BallBewegen()
        {
            ball.Left += ballSpeedX;
            ball.Top += ballSpeedY;

            if (ball.Top <= 0 ||
                ball.Bottom >= ClientSize.Height)
            {
                ballSpeedY = -ballSpeedY;
            }

            if (ball.Bounds.IntersectsWith(panelLinks.Bounds))
            {
                ballSpeedX = Math.Abs(ballSpeedX);
            }

            if (ball.Bounds.IntersectsWith(panelRechts.Bounds))
            {
                ballSpeedX = -Math.Abs(ballSpeedX);
            }
        }

        private void PingPong_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                hochGedruecktLinks = true;
            }

            if (e.KeyCode == Keys.S)
            {
                runterGedruecktLinks = true;
            }

            if (e.KeyCode == Keys.Up)
            {
                hochGedruecktRechts = true;
            }

            if (e.KeyCode == Keys.Down)
            {
                runterGedruecktRechts = true;
            }
        }

        private void PingPong_KeyUp(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
            {
                hochGedruecktLinks = false;
            }

            if (e.KeyCode == Keys.S)
            {
                runterGedruecktLinks = false;
            }

            if (e.KeyCode == Keys.Up)
            {
                hochGedruecktRechts = false;
            }

            if (e.KeyCode == Keys.Down)
            {
                runterGedruecktRechts = false;
            }
        }
    }
}
