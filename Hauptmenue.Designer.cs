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
            SuspendLayout();
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            titleLabel.Location = new Point(300, 72);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(381, 54);
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
            schuelerPage.Location = new Point(340, 196);
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
            lehrerPage.Location = new Point(340, 255);
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
            klassenPage.Location = new Point(340, 314);
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
            kursePage.Location = new Point(340, 373);
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
            stundenplanPage.Location = new Point(340, 432);
            stundenplanPage.Name = "stundenplanPage";
            stundenplanPage.Size = new Size(300, 50);
            stundenplanPage.TabIndex = 6;
            stundenplanPage.Text = "Stundenplan";
            stundenplanPage.UseVisualStyleBackColor = true;
            stundenplanPage.Click += stundenplanPage_Click;
            // 
            // Hauptmenue
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(984, 561);
            Controls.Add(stundenplanPage);
            Controls.Add(kursePage);
            Controls.Add(klassenPage);
            Controls.Add(lehrerPage);
            Controls.Add(schuelerPage);
            Controls.Add(subtitleLabel);
            Controls.Add(titleLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Hauptmenue";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SchulApp - Hauptmenü";
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
    }
}
