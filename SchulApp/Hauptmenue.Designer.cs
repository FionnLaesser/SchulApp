namespace SchulApp
{
    partial class Hauptmenue
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            titleLabel = new Label();
            subtitleLabel = new Label();
            schuelerPage = new Button();
            lehrerPage = new Button();
            klassenPage = new Button();
            kursePage = new Button();
            stundenplanPage = new Button();
            hauptbild = new PictureBox();
            einstellungPage = new Button();
            abmeldenBtn = new Button();
            profileBtn = new Button();
            pingPong = new Button();
            bestenlistePage = new Button();
            auditLogPage = new Button();
            ((System.ComponentModel.ISupportInitialize)hauptbild).BeginInit();
            SuspendLayout();
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            titleLabel.Location = new Point(300, 72);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(338, 54);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Schulverwaltung";
            // 
            // subtitleLabel
            // 
            subtitleLabel.AutoSize = true;
            subtitleLabel.Font = new Font("Segoe UI", 12F);
            subtitleLabel.Location = new Point(323, 136);
            subtitleLabel.Name = "subtitleLabel";
            subtitleLabel.Size = new Size(336, 21);
            subtitleLabel.TabIndex = 1;
            subtitleLabel.Text = "Wähle den Bereich, den du verwalten möchtest.";
            // 
            // schuelerPage
            // 
            schuelerPage.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            schuelerPage.Location = new Point(338, 172);
            schuelerPage.Name = "schuelerPage";
            schuelerPage.Size = new Size(300, 50);
            schuelerPage.TabIndex = 2;
            schuelerPage.Text = "Schüler";
            schuelerPage.UseVisualStyleBackColor = true;
            schuelerPage.Click += schuelerPage_Click;
            // 
            // lehrerPage
            // 
            lehrerPage.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lehrerPage.Location = new Point(338, 231);
            lehrerPage.Name = "lehrerPage";
            lehrerPage.Size = new Size(300, 50);
            lehrerPage.TabIndex = 3;
            lehrerPage.Text = "Lehrer";
            lehrerPage.UseVisualStyleBackColor = true;
            lehrerPage.Click += lehrerPage_Click;
            // 
            // klassenPage
            // 
            klassenPage.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            klassenPage.Location = new Point(338, 290);
            klassenPage.Name = "klassenPage";
            klassenPage.Size = new Size(300, 50);
            klassenPage.TabIndex = 4;
            klassenPage.Text = "Klassen";
            klassenPage.UseVisualStyleBackColor = true;
            klassenPage.Click += klassenPage_Click;
            // 
            // kursePage
            // 
            kursePage.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            kursePage.Location = new Point(338, 349);
            kursePage.Name = "kursePage";
            kursePage.Size = new Size(300, 50);
            kursePage.TabIndex = 5;
            kursePage.Text = "Kurse";
            kursePage.UseVisualStyleBackColor = true;
            kursePage.Click += kursePage_Click;
            // 
            // stundenplanPage
            // 
            stundenplanPage.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            stundenplanPage.Location = new Point(338, 408);
            stundenplanPage.Name = "stundenplanPage";
            stundenplanPage.Size = new Size(300, 50);
            stundenplanPage.TabIndex = 6;
            stundenplanPage.Text = "Stundenplan";
            stundenplanPage.UseVisualStyleBackColor = true;
            stundenplanPage.Click += stundenplanPage_Click;
            // 
            // hauptbild
            // 
            hauptbild.Location = new Point(654, 222);
            hauptbild.Name = "hauptbild";
            hauptbild.Size = new Size(318, 179);
            hauptbild.SizeMode = PictureBoxSizeMode.StretchImage;
            hauptbild.TabIndex = 7;
            hauptbild.TabStop = false;
            // 
            // einstellungPage
            // 
            einstellungPage.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            einstellungPage.Location = new Point(338, 464);
            einstellungPage.Name = "einstellungPage";
            einstellungPage.Size = new Size(300, 50);
            einstellungPage.TabIndex = 8;
            einstellungPage.Text = "Einstellung";
            einstellungPage.UseVisualStyleBackColor = true;
            einstellungPage.Click += einstellungPage_Click;
            // 
            // abmeldenBtn
            // 
            abmeldenBtn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            abmeldenBtn.Location = new Point(842, 128);
            abmeldenBtn.Name = "abmeldenBtn";
            abmeldenBtn.Size = new Size(130, 38);
            abmeldenBtn.TabIndex = 9;
            abmeldenBtn.Text = "Abmelden";
            abmeldenBtn.UseVisualStyleBackColor = true;
            abmeldenBtn.Click += abmeldenBtn_Click;
            // 
            // profileBtn
            // 
            profileBtn.Location = new Point(912, 72);
            profileBtn.Name = "profileBtn";
            profileBtn.Size = new Size(60, 50);
            profileBtn.TabIndex = 10;
            profileBtn.UseVisualStyleBackColor = true;
            profileBtn.Click += profileBtn_Click;
            // 
            // pingPong
            // 
            pingPong.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            pingPong.Location = new Point(12, 194);
            pingPong.Name = "pingPong";
            pingPong.Size = new Size(163, 50);
            pingPong.TabIndex = 11;
            pingPong.Text = "Ping Pong";
            pingPong.UseVisualStyleBackColor = true;
            pingPong.Click += pingPong_Click;
            // 
            // bestenlistePage
            // 
            bestenlistePage.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            bestenlistePage.Location = new Point(12, 253);
            bestenlistePage.Name = "bestenlistePage";
            bestenlistePage.Size = new Size(163, 50);
            bestenlistePage.TabIndex = 12;
            bestenlistePage.Text = "Bestenliste";
            bestenlistePage.UseVisualStyleBackColor = true;
            bestenlistePage.Click += bestenlistePage_Click;
            // 
            // auditLogPage
            // 
            auditLogPage.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            auditLogPage.Location = new Point(12, 312);
            auditLogPage.Name = "auditLogPage";
            auditLogPage.Size = new Size(163, 50);
            auditLogPage.TabIndex = 13;
            auditLogPage.Text = "Audit Log";
            auditLogPage.UseVisualStyleBackColor = true;
            auditLogPage.Click += auditLogPage_Click;
            // 
            // Hauptmenue
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(984, 561);
            Controls.Add(auditLogPage);
            Controls.Add(bestenlistePage);
            Controls.Add(pingPong);
            Controls.Add(profileBtn);
            Controls.Add(abmeldenBtn);
            Controls.Add(einstellungPage);
            Controls.Add(stundenplanPage);
            Controls.Add(kursePage);
            Controls.Add(klassenPage);
            Controls.Add(lehrerPage);
            Controls.Add(schuelerPage);
            Controls.Add(subtitleLabel);
            Controls.Add(titleLabel);
            Controls.Add(hauptbild);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Hauptmenue";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SchulApp - Hauptmenü";
            Shown += Hauptmenue_Shown;
            Controls.SetChildIndex(hauptbild, 0);
            Controls.SetChildIndex(titleLabel, 0);
            Controls.SetChildIndex(subtitleLabel, 0);
            Controls.SetChildIndex(schuelerPage, 0);
            Controls.SetChildIndex(lehrerPage, 0);
            Controls.SetChildIndex(klassenPage, 0);
            Controls.SetChildIndex(kursePage, 0);
            Controls.SetChildIndex(stundenplanPage, 0);
            Controls.SetChildIndex(einstellungPage, 0);
            Controls.SetChildIndex(abmeldenBtn, 0);
            Controls.SetChildIndex(profileBtn, 0);
            Controls.SetChildIndex(pingPong, 0);
            Controls.SetChildIndex(bestenlistePage, 0);
            Controls.SetChildIndex(auditLogPage, 0);
            ((System.ComponentModel.ISupportInitialize)hauptbild).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label titleLabel;
        private Label subtitleLabel;
        private Button schuelerPage;
        private Button lehrerPage;
        private Button klassenPage;
        private Button kursePage;
        private Button stundenplanPage;
        private PictureBox hauptbild;
        private Button einstellungPage;
        private Button abmeldenBtn;
        private Button profileBtn;
        private Button pingPong;
        private Button bestenlistePage;
        private Button auditLogPage;
    }
}
