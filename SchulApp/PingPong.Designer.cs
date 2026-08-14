namespace SchulApp
{
    partial class PingPong
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PingPong));
            ball = new PictureBox();
            panelLinks = new Panel();
            panelRechts = new Panel();
            gameTimer = new System.Windows.Forms.Timer(components);
            label1 = new Label();
            panelTop = new Panel();
            punkteZahlLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)ball).BeginInit();
            SuspendLayout();
            // 
            // ball
            // 
            ball.Image = (Image)resources.GetObject("ball.Image");
            ball.Location = new Point(468, 354);
            ball.Name = "ball";
            ball.Size = new Size(59, 62);
            ball.SizeMode = PictureBoxSizeMode.StretchImage;
            ball.TabIndex = 1;
            ball.TabStop = false;
            // 
            // panelLinks
            // 
            panelLinks.BackColor = SystemColors.ActiveCaptionText;
            panelLinks.Location = new Point(0, 253);
            panelLinks.Name = "panelLinks";
            panelLinks.Size = new Size(34, 196);
            panelLinks.TabIndex = 3;
            // 
            // panelRechts
            // 
            panelRechts.BackColor = SystemColors.ActiveCaptionText;
            panelRechts.Location = new Point(1166, 253);
            panelRechts.Name = "panelRechts";
            panelRechts.Size = new Size(34, 196);
            panelRechts.TabIndex = 4;
            // 
            // gameTimer
            // 
            gameTimer.Tick += gameTimer_Tick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(152, 27);
            label1.Name = "label1";
            label1.Size = new Size(181, 15);
            label1.TabIndex = 5;
            label1.Text = "Zum Verlassen einfach schliessen";
            // 
            // panelTop
            // 
            panelTop.Location = new Point(0, 17);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(1200, 10);
            panelTop.TabIndex = 6;
            // 
            // punkteZahlLabel
            // 
            punkteZahlLabel.AutoSize = true;
            punkteZahlLabel.Location = new Point(489, 45);
            punkteZahlLabel.Name = "punkteZahlLabel";
            punkteZahlLabel.Size = new Size(22, 15);
            punkteZahlLabel.TabIndex = 7;
            punkteZahlLabel.Text = "1:3";
            // 
            // PingPong
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 750);
            Controls.Add(punkteZahlLabel);
            Controls.Add(label1);
            Controls.Add(panelRechts);
            Controls.Add(panelLinks);
            Controls.Add(ball);
            Controls.Add(panelTop);
            MinimumSize = new Size(900, 600);
            Name = "PingPong";
            StartPosition = FormStartPosition.Manual;
            Text = "SchulApp";
            Controls.SetChildIndex(panelTop, 0);
            Controls.SetChildIndex(ball, 0);
            Controls.SetChildIndex(panelLinks, 0);
            Controls.SetChildIndex(panelRechts, 0);
            Controls.SetChildIndex(label1, 0);
            Controls.SetChildIndex(punkteZahlLabel, 0);
            ((System.ComponentModel.ISupportInitialize)ball).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox ball;
        private Panel panelLinks;
        private Panel panelRechts;
        private System.Windows.Forms.Timer gameTimer;
        private Label label1;
        private Panel panelTop;
        private Label punkteZahlLabel;
    }
}
