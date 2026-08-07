using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SchulApp
{
    public partial class Hauptmenue : Form
    {
        private static bool schonGestartet = false;

        private Image originalBild;
        private float transparenz = 1.0f;
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public Hauptmenue()
        {
            InitializeComponent();

            startBild.Visible = false;

            if (schonGestartet == false)
            {
                startBild.Visible = true;
                originalBild = startBild.Image;

                startBild.BringToFront();
                StartBildVerschwinden();

                schonGestartet = true;
            }
        }

        private void lehrerPage_Click(object sender, EventArgs e)
        {
            Lehrer lehrerFenster = new Lehrer();
            lehrerFenster.Show();

            this.Hide();
        }

        private void schuelerPage_Click(object sender, EventArgs e)
        {
            Schueler schuelerFenster = new Schueler();
            schuelerFenster.Show();

            this.Hide();
        }

        private async void StartBildVerschwinden()
        {
            await Task.Delay(2000);

            timer.Interval = 30;

            timer.Tick += (sender, e) =>
            {
                transparenz -= 0.03f;

                if (transparenz <= 0)
                {
                    timer.Stop();
                    startBild.Visible = false;
                    return;
                }

                startBild.Image = Opacity(originalBild, transparenz);
            };

            timer.Start();
        }

        private Image Opacity(Image image, float opacity)
        {
            Bitmap bmp = new Bitmap(image.Width, image.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = opacity;

                using (ImageAttributes attributes = new ImageAttributes())
                {
                    attributes.SetColorMatrix(matrix);

                    g.DrawImage(
                        image,
                        new Rectangle(0, 0, bmp.Width, bmp.Height),
                        0,
                        0,
                        image.Width,
                        image.Height,
                        GraphicsUnit.Pixel,
                        attributes
                    );
                }
            }

            return bmp;
        }
    }
}