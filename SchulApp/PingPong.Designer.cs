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
            spielerLinksNameLabel = new Label();
            spielerRechtsNameLabel = new Label();
            gegnerLabel = new Label();
            gegnerComboBox = new ComboBox();
            spielStartBtn = new Button();
            statusLabel = new Label();
            spielEndePanel = new Panel();
            speicherStatusLabel = new Label();
            zurueckBtn = new Button();
            erneutSpielenBtn = new Button();
            ergebnisLabel = new Label();
            gewinnerLabel = new Label();
            spielEndeTitelLabel = new Label();
            pausePanel = new Panel();
            pauseTextLabel = new Label();
            pauseHauptmenueBtn = new Button();
            fortsetzenBtn = new Button();
            pauseTitelLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)ball).BeginInit();
            spielEndePanel.SuspendLayout();
            pausePanel.SuspendLayout();
            SuspendLayout();

            // 
            // ball
            // 
            ball.Image = (Image)resources.GetObject("ball.Image");
            ball.Location = new Point(571, 350);
            ball.Name = "ball";
            ball.Size = new Size(59, 62);
            ball.SizeMode = PictureBoxSizeMode.StretchImage;
            ball.TabIndex = 1;
            ball.TabStop = false;

            // 
            // panelLinks
            // 
            panelLinks.BackColor = Color.Black;
            panelLinks.Location = new Point(0, 300);
            panelLinks.Name = "panelLinks";
            panelLinks.Size = new Size(34, 196);
            panelLinks.TabIndex = 3;

            // 
            // panelRechts
            // 
            panelRechts.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panelRechts.BackColor = Color.Black;
            panelRechts.Location = new Point(1166, 300);
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
            label1.Location = new Point(20, 18);
            label1.Name = "label1";
            label1.Size = new Size(257, 15);
            label1.TabIndex = 5;
            label1.Text = "Links: W/S | Rechts: Pfeiltasten | Q: Pause";

            // 
            // panelTop
            // 
            panelTop.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelTop.BackColor = Color.Black;
            panelTop.Location = new Point(0, 105);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(1200, 6);
            panelTop.TabIndex = 6;

            // 
            // punkteZahlLabel
            // 
            punkteZahlLabel.Anchor = AnchorStyles.Top;
            punkteZahlLabel.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            punkteZahlLabel.Location = new Point(520, 32);
            punkteZahlLabel.Name = "punkteZahlLabel";
            punkteZahlLabel.Size = new Size(160, 50);
            punkteZahlLabel.TabIndex = 7;
            punkteZahlLabel.Text = "0 : 0";
            punkteZahlLabel.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // spielerLinksNameLabel
            // 
            spielerLinksNameLabel.AutoSize = true;
            spielerLinksNameLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            spielerLinksNameLabel.Location = new Point(20, 55);
            spielerLinksNameLabel.Name = "spielerLinksNameLabel";
            spielerLinksNameLabel.Size = new Size(62, 20);
            spielerLinksNameLabel.TabIndex = 8;
            spielerLinksNameLabel.Text = "Links: -";

            // 
            // spielerRechtsNameLabel
            // 
            spielerRechtsNameLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            spielerRechtsNameLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            spielerRechtsNameLabel.Location = new Point(850, 62);
            spielerRechtsNameLabel.Name = "spielerRechtsNameLabel";
            spielerRechtsNameLabel.Size = new Size(330, 24);
            spielerRechtsNameLabel.TabIndex = 9;
            spielerRechtsNameLabel.Text = "Rechts: -";
            spielerRechtsNameLabel.TextAlign = ContentAlignment.MiddleRight;

            // 
            // gegnerLabel
            // 
            gegnerLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            gegnerLabel.AutoSize = true;
            gegnerLabel.Location = new Point(800, 20);
            gegnerLabel.Name = "gegnerLabel";
            gegnerLabel.Size = new Size(49, 15);
            gegnerLabel.TabIndex = 10;
            gegnerLabel.Text = "Gegner:";

            // 
            // gegnerComboBox
            // 
            gegnerComboBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            gegnerComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            gegnerComboBox.Enabled = false;
            gegnerComboBox.FormattingEnabled = true;
            gegnerComboBox.Location = new Point(855, 16);
            gegnerComboBox.Name = "gegnerComboBox";
            gegnerComboBox.Size = new Size(205, 23);
            gegnerComboBox.TabIndex = 11;
            gegnerComboBox.SelectedIndexChanged += gegnerComboBox_SelectedIndexChanged;

            // 
            // spielStartBtn
            // 
            spielStartBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            spielStartBtn.Enabled = false;
            spielStartBtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            spielStartBtn.Location = new Point(1070, 14);
            spielStartBtn.Name = "spielStartBtn";
            spielStartBtn.Size = new Size(110, 28);
            spielStartBtn.TabIndex = 12;
            spielStartBtn.Text = "Spiel starten";
            spielStartBtn.UseVisualStyleBackColor = true;
            spielStartBtn.Click += spielStartBtn_Click;

            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(20, 82);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(253, 15);
            statusLabel.TabIndex = 13;
            statusLabel.Text = "Benutzer werden über die REST API geladen...";

            // 
            // spielEndePanel
            // 
            spielEndePanel.Anchor = AnchorStyles.None;
            spielEndePanel.BorderStyle = BorderStyle.FixedSingle;
            spielEndePanel.Controls.Add(speicherStatusLabel);
            spielEndePanel.Controls.Add(zurueckBtn);
            spielEndePanel.Controls.Add(erneutSpielenBtn);
            spielEndePanel.Controls.Add(ergebnisLabel);
            spielEndePanel.Controls.Add(gewinnerLabel);
            spielEndePanel.Controls.Add(spielEndeTitelLabel);
            spielEndePanel.Location = new Point(400, 260);
            spielEndePanel.Name = "spielEndePanel";
            spielEndePanel.Size = new Size(400, 250);
            spielEndePanel.TabIndex = 14;
            spielEndePanel.Visible = false;

            // 
            // speicherStatusLabel
            // 
            speicherStatusLabel.Location = new Point(25, 130);
            speicherStatusLabel.Name = "speicherStatusLabel";
            speicherStatusLabel.Size = new Size(350, 40);
            speicherStatusLabel.TabIndex = 5;
            speicherStatusLabel.Text = "Ergebnis wird gespeichert...";
            speicherStatusLabel.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // zurueckBtn
            // 
            zurueckBtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            zurueckBtn.Location = new Point(205, 190);
            zurueckBtn.Name = "zurueckBtn";
            zurueckBtn.Size = new Size(170, 40);
            zurueckBtn.TabIndex = 4;
            zurueckBtn.Text = "Zurück zum Hauptmenü";
            zurueckBtn.UseVisualStyleBackColor = true;
            zurueckBtn.Click += zurueckBtn_Click;

            // 
            // erneutSpielenBtn
            // 
            erneutSpielenBtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            erneutSpielenBtn.Location = new Point(25, 190);
            erneutSpielenBtn.Name = "erneutSpielenBtn";
            erneutSpielenBtn.Size = new Size(160, 40);
            erneutSpielenBtn.TabIndex = 3;
            erneutSpielenBtn.Text = "Erneut spielen";
            erneutSpielenBtn.UseVisualStyleBackColor = true;
            erneutSpielenBtn.Click += erneutSpielenBtn_Click;

            // 
            // ergebnisLabel
            // 
            ergebnisLabel.Font = new Font("Segoe UI", 12F);
            ergebnisLabel.Location = new Point(25, 95);
            ergebnisLabel.Name = "ergebnisLabel";
            ergebnisLabel.Size = new Size(350, 30);
            ergebnisLabel.TabIndex = 2;
            ergebnisLabel.Text = "Ergebnis: 5 : 0";
            ergebnisLabel.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // gewinnerLabel
            // 
            gewinnerLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            gewinnerLabel.Location = new Point(25, 60);
            gewinnerLabel.Name = "gewinnerLabel";
            gewinnerLabel.Size = new Size(350, 30);
            gewinnerLabel.TabIndex = 1;
            gewinnerLabel.Text = "Gewinner: Spieler";
            gewinnerLabel.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // spielEndeTitelLabel
            // 
            spielEndeTitelLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            spielEndeTitelLabel.Location = new Point(25, 15);
            spielEndeTitelLabel.Name = "spielEndeTitelLabel";
            spielEndeTitelLabel.Size = new Size(350, 40);
            spielEndeTitelLabel.TabIndex = 0;
            spielEndeTitelLabel.Text = "Spiel beendet!";
            spielEndeTitelLabel.TextAlign = ContentAlignment.MiddleCenter;


            // 
            // pausePanel
            // 
            pausePanel.Anchor = AnchorStyles.None;
            pausePanel.BorderStyle = BorderStyle.FixedSingle;
            pausePanel.Controls.Add(pauseTextLabel);
            pausePanel.Controls.Add(pauseHauptmenueBtn);
            pausePanel.Controls.Add(fortsetzenBtn);
            pausePanel.Controls.Add(pauseTitelLabel);
            pausePanel.Location = new Point(400, 265);
            pausePanel.Name = "pausePanel";
            pausePanel.Size = new Size(400, 220);
            pausePanel.TabIndex = 15;
            pausePanel.Visible = false;

            // 
            // pauseTextLabel
            // 
            pauseTextLabel.Location = new Point(25, 65);
            pauseTextLabel.Name = "pauseTextLabel";
            pauseTextLabel.Size = new Size(350, 50);
            pauseTextLabel.TabIndex = 1;
            pauseTextLabel.Text = "Das Spiel ist pausiert.";
            pauseTextLabel.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // pauseHauptmenueBtn
            // 
            pauseHauptmenueBtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            pauseHauptmenueBtn.Location = new Point(205, 145);
            pauseHauptmenueBtn.Name = "pauseHauptmenueBtn";
            pauseHauptmenueBtn.Size = new Size(170, 40);
            pauseHauptmenueBtn.TabIndex = 3;
            pauseHauptmenueBtn.Text = "Zurück zum Hauptmenü";
            pauseHauptmenueBtn.UseVisualStyleBackColor = true;
            pauseHauptmenueBtn.Click += pauseHauptmenueBtn_Click;

            // 
            // fortsetzenBtn
            // 
            fortsetzenBtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            fortsetzenBtn.Location = new Point(25, 145);
            fortsetzenBtn.Name = "fortsetzenBtn";
            fortsetzenBtn.Size = new Size(160, 40);
            fortsetzenBtn.TabIndex = 2;
            fortsetzenBtn.Text = "Fortsetzen";
            fortsetzenBtn.UseVisualStyleBackColor = true;
            fortsetzenBtn.Click += fortsetzenBtn_Click;

            // 
            // pauseTitelLabel
            // 
            pauseTitelLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            pauseTitelLabel.Location = new Point(25, 15);
            pauseTitelLabel.Name = "pauseTitelLabel";
            pauseTitelLabel.Size = new Size(350, 40);
            pauseTitelLabel.TabIndex = 0;
            pauseTitelLabel.Text = "Spiel pausiert";
            pauseTitelLabel.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // PingPong
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1200, 750);
            Controls.Add(pausePanel);
            Controls.Add(spielEndePanel);
            Controls.Add(statusLabel);
            Controls.Add(spielStartBtn);
            Controls.Add(gegnerComboBox);
            Controls.Add(gegnerLabel);
            Controls.Add(spielerRechtsNameLabel);
            Controls.Add(spielerLinksNameLabel);
            Controls.Add(punkteZahlLabel);
            Controls.Add(label1);
            Controls.Add(panelRechts);
            Controls.Add(panelLinks);
            Controls.Add(ball);
            Controls.Add(panelTop);
            MinimumSize = new Size(900, 600);
            Name = "PingPong";
            StartPosition = FormStartPosition.Manual;
            Text = "SchulApp - Ping Pong";
            Shown += PingPong_Shown;
            Controls.SetChildIndex(panelTop, 0);
            Controls.SetChildIndex(ball, 0);
            Controls.SetChildIndex(panelLinks, 0);
            Controls.SetChildIndex(panelRechts, 0);
            Controls.SetChildIndex(label1, 0);
            Controls.SetChildIndex(punkteZahlLabel, 0);
            Controls.SetChildIndex(spielerLinksNameLabel, 0);
            Controls.SetChildIndex(spielerRechtsNameLabel, 0);
            Controls.SetChildIndex(gegnerLabel, 0);
            Controls.SetChildIndex(gegnerComboBox, 0);
            Controls.SetChildIndex(spielStartBtn, 0);
            Controls.SetChildIndex(statusLabel, 0);
            Controls.SetChildIndex(spielEndePanel, 0);
            Controls.SetChildIndex(pausePanel, 0);
            ((System.ComponentModel.ISupportInitialize)ball).EndInit();
            spielEndePanel.ResumeLayout(false);
            pausePanel.ResumeLayout(false);
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
        private Label spielerLinksNameLabel;
        private Label spielerRechtsNameLabel;
        private Label gegnerLabel;
        private ComboBox gegnerComboBox;
        private Button spielStartBtn;
        private Label statusLabel;
        private Panel spielEndePanel;
        private Label speicherStatusLabel;
        private Button zurueckBtn;
        private Button erneutSpielenBtn;
        private Label ergebnisLabel;
        private Label gewinnerLabel;
        private Label spielEndeTitelLabel;
        private Panel pausePanel;
        private Label pauseTextLabel;
        private Button pauseHauptmenueBtn;
        private Button fortsetzenBtn;
        private Label pauseTitelLabel;
    }
}
