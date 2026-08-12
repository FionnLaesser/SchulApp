using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace SchulApp
{
    public class CustomForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(
            IntPtr hWnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam
        );

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HT_CAPTION = 0x2;

        private const int TitleBarHoehe = 42;

        private Panel titleBar = null!;
        private Label titleLabel = null!;

        private Button btnMinimize = null!;
        private Button btnMaximize = null!;
        private Button btnClose = null!;

        private bool layoutAngepasst;

        protected CustomForm()
        {
            FormBorderStyle = FormBorderStyle.None;

            ErstelleTitleBar();
        }

        private void ErstelleTitleBar()
        {
            titleBar = new Panel
            {
                Height = TitleBarHoehe,
                Dock = DockStyle.Top,
                BackColor = BackColor
            };

            titleLabel = new Label
            {
                Dock = DockStyle.Fill,
                Text = Text,
                ForeColor = ForeColor,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                BackColor = Color.Transparent
            };

            btnClose = ErstelleButton("close.png");
            btnMinimize = ErstelleButton("minimize.png");
            btnMaximize = ErstelleButton("maximize.png");

            btnClose.Dock = DockStyle.Right;
            btnMaximize.Dock = DockStyle.Right;
            btnMinimize.Dock = DockStyle.Right;

            btnClose.Click += (_, _) =>
            {
                Environment.Exit(0); // beendet die ganze App (z.B bei register funktioniert normal nicht)
            };

            btnMinimize.Click += async (_, _) =>
            {
                await MinimizeMitAnimation();
            };

            // Maximize ist nur zur Anzeige da und absichtlich nicht anklickbar.
            btnMaximize.Cursor = Cursors.Default;

            if (btnMaximize.Image != null)
            {
                Image original = btnMaximize.Image;
                btnMaximize.Image = BildAusgrauen(original);
                original.Dispose();
            }

            titleBar.MouseDown += TitleBar_MouseDown;
            titleLabel.MouseDown += TitleBar_MouseDown;

            titleBar.Controls.Add(titleLabel);
            titleBar.Controls.Add(btnMinimize);
            titleBar.Controls.Add(btnMaximize);
            titleBar.Controls.Add(btnClose);

            Controls.Add(titleBar);

            TextChanged += (_, _) =>
            {
                titleLabel.Text = Text;
            };
        }

        private async Task MinimizeMitAnimation()
        {
            const int schritte = 10;
            const int dauerProSchritt = 10;

            int originalTop = Top;

            for (int i = schritte; i >= 0; i--)
            {
                Opacity = (double)i / schritte;

                Top += 3;

                await Task.Delay(dauerProSchritt);
            }

            WindowState = FormWindowState.Minimized;

            Top = originalTop;
            Opacity = 1;
        }

        private Button ErstelleButton(string dateiname)
        {
            Button button = new Button
            {
                Width = 46,
                Height = TitleBarHoehe,
                FlatStyle = FlatStyle.Flat,
                BackColor = titleBar.BackColor,
                TabStop = false,
                Cursor = Cursors.Hand,
                ImageAlign = ContentAlignment.MiddleCenter,
                UseVisualStyleBackColor = false
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = titleBar.BackColor;
            button.FlatAppearance.MouseDownBackColor = titleBar.BackColor;

            string pfad = Path.Combine(
                AppContext.BaseDirectory,
                "images",
                dateiname
            );

            if (File.Exists(pfad))
            {
                using Image original = Image.FromFile(pfad);

                button.Image = new Bitmap(
                    original,
                    new Size(24, 24)
                );
            }

            return button;
        }

        private void TitleBar_MouseDown(
            object? sender,
            MouseEventArgs e
        )
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            ReleaseCapture();

            SendMessage(
                Handle,
                WM_NCLBUTTONDOWN,
                (IntPtr)HT_CAPTION,
                IntPtr.Zero
            );
        }

        protected override void OnLoad(EventArgs e)
        {
            // Falls InitializeComponent() den BorderStyle verändert hat,
            // wird er hier nochmals sicher auf None gesetzt.
            FormBorderStyle = FormBorderStyle.None;

            if (!layoutAngepasst)
            {
                layoutAngepasst = true;
                VerschiebeFormInhalt();
            }

            // Die Titelleiste übernimmt die aktuelle Form-Farbe.
            TitelleisteFarbenAktualisieren();

            base.OnLoad(e);

            titleBar.BringToFront();
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            if (titleBar != null)
            {
                TitelleisteFarbenAktualisieren();
            }
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);

            if (titleLabel != null)
            {
                titleLabel.ForeColor = ForeColor;
            }
        }

        private void TitelleisteFarbenAktualisieren()
        {
            titleBar.BackColor = BackColor;
            titleLabel.ForeColor = ForeColor;

            ButtonHintergrundAktualisieren(btnMinimize);
            ButtonHintergrundAktualisieren(btnMaximize);
            ButtonHintergrundAktualisieren(btnClose);
        }

        private void ButtonHintergrundAktualisieren(Button button)
        {
            button.BackColor = titleBar.BackColor;
            button.FlatAppearance.MouseOverBackColor = titleBar.BackColor;
            button.FlatAppearance.MouseDownBackColor = titleBar.BackColor;
        }

        private void VerschiebeFormInhalt()
        {
            SuspendLayout();

            // Die ursprüngliche Formfläche bleibt erhalten.
            Height += TitleBarHoehe;

            foreach (Control control in Controls)
            {
                if (control == titleBar)
                {
                    continue;
                }

                control.Top += TitleBarHoehe;
            }

            titleBar.Top = 0;
            titleBar.Left = 0;

            ResumeLayout(true);
        }

        private static Image BildAusgrauen(Image image)
        {
            Bitmap bitmap = new Bitmap(
                image.Width,
                image.Height
            );

            using Graphics g = Graphics.FromImage(bitmap);
            using ImageAttributes attributes = new ImageAttributes();

            ColorMatrix matrix = new ColorMatrix
            {
                Matrix33 = 0.35f
            };

            attributes.SetColorMatrix(matrix);

            g.DrawImage(
                image,
                new Rectangle(
                    0,
                    0,
                    bitmap.Width,
                    bitmap.Height
                ),
                0,
                0,
                image.Width,
                image.Height,
                GraphicsUnit.Pixel,
                attributes
            );

            return bitmap;
        }
    }
}
