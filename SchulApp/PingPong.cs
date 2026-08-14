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

        private int punkteLinks = 0;
        private int punkteRechts = 0;

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

            PunkteAnzeigen();
            SpielNeuStarten(1);

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
            if (hochGedruecktLinks &&
                panelLinks.Top > panelTop.Bottom)
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
            if (hochGedruecktRechts &&
                panelRechts.Top > panelTop.Bottom)
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

            // Obere Wand
            if (ball.Top <= panelTop.Bottom)
            {
                ball.Top = panelTop.Bottom;
                ballSpeedY = Math.Abs(ballSpeedY);
            }

            // Untere Wand
            if (ball.Bottom >= ClientSize.Height)
            {
                ball.Top = ClientSize.Height - ball.Height;
                ballSpeedY = -Math.Abs(ballSpeedY);
            }

            // Linker Schläger
            if (ball.Bounds.IntersectsWith(panelLinks.Bounds) &&
                ballSpeedX < 0)
            {
                ball.Left = panelLinks.Right;
                ballSpeedX = Math.Abs(ballSpeedX);
            }

            // Rechter Schläger
            if (ball.Bounds.IntersectsWith(panelRechts.Bounds) &&
                ballSpeedX > 0)
            {
                ball.Left = panelRechts.Left - ball.Width;
                ballSpeedX = -Math.Abs(ballSpeedX);
            }

            PunktePruefen();
        }

        private void PunktePruefen()
        {
            // Ball links raus
            if (ball.Right < 0)
            {
                punkteRechts++;

                PunkteAnzeigen();

                // Ball startet danach nach links
                SpielNeuStarten(-1);
            }

            // Ball rechts raus
            else if (ball.Left > ClientSize.Width)
            {
                punkteLinks++;

                PunkteAnzeigen();

                // Ball startet danach nach rechts
                SpielNeuStarten(1);
            }
        }

        private void PunkteAnzeigen()
        {
            punkteZahlLabel.Text =
                $"{punkteLinks} : {punkteRechts}";
        }

        private void SpielNeuStarten(int richtung)
        {
            // Ball in die Mitte
            ball.Left =
                (ClientSize.Width - ball.Width) / 2;

            int spielfeldHoehe =
                ClientSize.Height - panelTop.Bottom;

            ball.Top =
                panelTop.Bottom +
                (spielfeldHoehe - ball.Height) / 2;

            // Linken Schläger zurücksetzen
            panelLinks.Top =
                panelTop.Bottom +
                (spielfeldHoehe - panelLinks.Height) / 2;

            // Rechten Schläger zurücksetzen
            panelRechts.Top =
                panelTop.Bottom +
                (spielfeldHoehe - panelRechts.Height) / 2;

            // Ballrichtung setzen
            ballSpeedX = Math.Abs(ballSpeedX) * richtung;
            ballSpeedY = 5;
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