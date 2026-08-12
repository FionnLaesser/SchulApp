namespace SchulApp
{
    partial class Profil
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
            profilPicture = new PictureBox();
            profilNameLabel = new Label();
            userLabel = new Label();
            userText = new TextBox();
            rolleLabel = new Label();
            rolleText = new TextBox();
            vornameLabel = new Label();
            vornameText = new TextBox();
            nachnameLabel = new Label();
            nachnameText = new TextBox();
            emailLabel = new Label();
            emailText = new TextBox();
            erstelltAmLabel = new Label();
            erstelltAmValueLabel = new Label();
            speichernBtn = new Button();
            sicherheitLabel = new Label();
            aktuellesPasswortLabel = new Label();
            aktuellesPasswortText = new TextBox();
            neuesPasswortLabel = new Label();
            neuesPasswortText = new TextBox();
            passwortWiederholenLabel = new Label();
            passwortWiederholenText = new TextBox();
            passwortAendernBtn = new Button();
            zurueckBtn = new Button();
            ((System.ComponentModel.ISupportInitialize)profilPicture).BeginInit();
            SuspendLayout();
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            titleLabel.Location = new Point(60, 72);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(230, 54);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Mein Profil";
            // 
            // subtitleLabel
            // 
            subtitleLabel.AutoSize = true;
            subtitleLabel.Font = new Font("Segoe UI", 12F);
            subtitleLabel.Location = new Point(64, 132);
            subtitleLabel.Name = "subtitleLabel";
            subtitleLabel.Size = new Size(334, 21);
            subtitleLabel.TabIndex = 1;
            subtitleLabel.Text = "Persönliche Angaben und Sicherheit verwalten.";
            // 
            // profilPicture
            // 
            profilPicture.Location = new Point(72, 188);
            profilPicture.Name = "profilPicture";
            profilPicture.Size = new Size(140, 140);
            profilPicture.SizeMode = PictureBoxSizeMode.Zoom;
            profilPicture.TabIndex = 2;
            profilPicture.TabStop = false;
            // 
            // profilNameLabel
            // 
            profilNameLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            profilNameLabel.Location = new Point(54, 340);
            profilNameLabel.Name = "profilNameLabel";
            profilNameLabel.Size = new Size(176, 28);
            profilNameLabel.TabIndex = 3;
            profilNameLabel.Text = "Benutzer";
            profilNameLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // userLabel
            // 
            userLabel.AutoSize = true;
            userLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            userLabel.Location = new Point(280, 170);
            userLabel.Name = "userLabel";
            userLabel.Size = new Size(105, 19);
            userLabel.TabIndex = 4;
            userLabel.Text = "Benutzername";
            // 
            // userText
            // 
            userText.Font = new Font("Segoe UI", 11F);
            userText.Location = new Point(280, 194);
            userText.MaxLength = 50;
            userText.Name = "userText";
            userText.Size = new Size(280, 27);
            userText.TabIndex = 0;
            // 
            // rolleLabel
            // 
            rolleLabel.AutoSize = true;
            rolleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            rolleLabel.Location = new Point(590, 170);
            rolleLabel.Name = "rolleLabel";
            rolleLabel.Size = new Size(43, 19);
            rolleLabel.TabIndex = 6;
            rolleLabel.Text = "Rolle";
            // 
            // rolleText
            // 
            rolleText.Font = new Font("Segoe UI", 11F);
            rolleText.Location = new Point(590, 194);
            rolleText.Name = "rolleText";
            rolleText.ReadOnly = true;
            rolleText.Size = new Size(280, 27);
            rolleText.TabIndex = 7;
            rolleText.TabStop = false;
            // 
            // vornameLabel
            // 
            vornameLabel.AutoSize = true;
            vornameLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            vornameLabel.Location = new Point(280, 235);
            vornameLabel.Name = "vornameLabel";
            vornameLabel.Size = new Size(69, 19);
            vornameLabel.TabIndex = 8;
            vornameLabel.Text = "Vorname";
            // 
            // vornameText
            // 
            vornameText.Font = new Font("Segoe UI", 11F);
            vornameText.Location = new Point(280, 259);
            vornameText.MaxLength = 100;
            vornameText.Name = "vornameText";
            vornameText.Size = new Size(280, 27);
            vornameText.TabIndex = 1;
            // 
            // nachnameLabel
            // 
            nachnameLabel.AutoSize = true;
            nachnameLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            nachnameLabel.Location = new Point(590, 235);
            nachnameLabel.Name = "nachnameLabel";
            nachnameLabel.Size = new Size(80, 19);
            nachnameLabel.TabIndex = 10;
            nachnameLabel.Text = "Nachname";
            // 
            // nachnameText
            // 
            nachnameText.Font = new Font("Segoe UI", 11F);
            nachnameText.Location = new Point(590, 259);
            nachnameText.MaxLength = 100;
            nachnameText.Name = "nachnameText";
            nachnameText.Size = new Size(280, 27);
            nachnameText.TabIndex = 2;
            // 
            // emailLabel
            // 
            emailLabel.AutoSize = true;
            emailLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            emailLabel.Location = new Point(280, 300);
            emailLabel.Name = "emailLabel";
            emailLabel.Size = new Size(51, 19);
            emailLabel.TabIndex = 12;
            emailLabel.Text = "E-Mail";
            // 
            // emailText
            // 
            emailText.Font = new Font("Segoe UI", 11F);
            emailText.Location = new Point(280, 324);
            emailText.MaxLength = 255;
            emailText.Name = "emailText";
            emailText.Size = new Size(590, 27);
            emailText.TabIndex = 3;
            // 
            // erstelltAmLabel
            // 
            erstelltAmLabel.AutoSize = true;
            erstelltAmLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            erstelltAmLabel.Location = new Point(280, 372);
            erstelltAmLabel.Name = "erstelltAmLabel";
            erstelltAmLabel.Size = new Size(79, 19);
            erstelltAmLabel.TabIndex = 14;
            erstelltAmLabel.Text = "Erstellt am";
            // 
            // erstelltAmValueLabel
            // 
            erstelltAmValueLabel.AutoSize = true;
            erstelltAmValueLabel.Font = new Font("Segoe UI", 10F);
            erstelltAmValueLabel.Location = new Point(365, 372);
            erstelltAmValueLabel.Name = "erstelltAmValueLabel";
            erstelltAmValueLabel.Size = new Size(15, 19);
            erstelltAmValueLabel.TabIndex = 15;
            erstelltAmValueLabel.Text = "-";
            // 
            // speichernBtn
            // 
            speichernBtn.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            speichernBtn.Location = new Point(720, 362);
            speichernBtn.Name = "speichernBtn";
            speichernBtn.Size = new Size(150, 40);
            speichernBtn.TabIndex = 4;
            speichernBtn.Text = "Profil speichern";
            speichernBtn.UseVisualStyleBackColor = true;
            speichernBtn.Click += speichernBtn_Click;
            // 
            // sicherheitLabel
            // 
            sicherheitLabel.AutoSize = true;
            sicherheitLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            sicherheitLabel.Location = new Point(280, 418);
            sicherheitLabel.Name = "sicherheitLabel";
            sicherheitLabel.Size = new Size(162, 25);
            sicherheitLabel.TabIndex = 17;
            sicherheitLabel.Text = "Passwort ändern";
            // 
            // aktuellesPasswortLabel
            // 
            aktuellesPasswortLabel.AutoSize = true;
            aktuellesPasswortLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            aktuellesPasswortLabel.Location = new Point(280, 449);
            aktuellesPasswortLabel.Name = "aktuellesPasswortLabel";
            aktuellesPasswortLabel.Size = new Size(112, 15);
            aktuellesPasswortLabel.TabIndex = 18;
            aktuellesPasswortLabel.Text = "Aktuelles Passwort";
            // 
            // aktuellesPasswortText
            // 
            aktuellesPasswortText.Font = new Font("Segoe UI", 10F);
            aktuellesPasswortText.Location = new Point(280, 468);
            aktuellesPasswortText.Name = "aktuellesPasswortText";
            aktuellesPasswortText.Size = new Size(165, 25);
            aktuellesPasswortText.TabIndex = 5;
            aktuellesPasswortText.UseSystemPasswordChar = true;
            // 
            // neuesPasswortLabel
            // 
            neuesPasswortLabel.AutoSize = true;
            neuesPasswortLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            neuesPasswortLabel.Location = new Point(460, 449);
            neuesPasswortLabel.Name = "neuesPasswortLabel";
            neuesPasswortLabel.Size = new Size(95, 15);
            neuesPasswortLabel.TabIndex = 20;
            neuesPasswortLabel.Text = "Neues Passwort";
            // 
            // neuesPasswortText
            // 
            neuesPasswortText.Font = new Font("Segoe UI", 10F);
            neuesPasswortText.Location = new Point(460, 468);
            neuesPasswortText.Name = "neuesPasswortText";
            neuesPasswortText.Size = new Size(165, 25);
            neuesPasswortText.TabIndex = 6;
            neuesPasswortText.UseSystemPasswordChar = true;
            // 
            // passwortWiederholenLabel
            // 
            passwortWiederholenLabel.AutoSize = true;
            passwortWiederholenLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            passwortWiederholenLabel.Location = new Point(640, 449);
            passwortWiederholenLabel.Name = "passwortWiederholenLabel";
            passwortWiederholenLabel.Size = new Size(130, 15);
            passwortWiederholenLabel.TabIndex = 22;
            passwortWiederholenLabel.Text = "Passwort wiederholen";
            // 
            // passwortWiederholenText
            // 
            passwortWiederholenText.Font = new Font("Segoe UI", 10F);
            passwortWiederholenText.Location = new Point(640, 468);
            passwortWiederholenText.Name = "passwortWiederholenText";
            passwortWiederholenText.Size = new Size(165, 25);
            passwortWiederholenText.TabIndex = 7;
            passwortWiederholenText.UseSystemPasswordChar = true;
            // 
            // passwortAendernBtn
            // 
            passwortAendernBtn.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            passwortAendernBtn.Location = new Point(820, 463);
            passwortAendernBtn.Name = "passwortAendernBtn";
            passwortAendernBtn.Size = new Size(120, 34);
            passwortAendernBtn.TabIndex = 8;
            passwortAendernBtn.Text = "Ändern";
            passwortAendernBtn.UseVisualStyleBackColor = true;
            passwortAendernBtn.Click += passwortAendernBtn_Click;
            // 
            // zurueckBtn
            // 
            zurueckBtn.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            zurueckBtn.Location = new Point(806, 86);
            zurueckBtn.Name = "zurueckBtn";
            zurueckBtn.Size = new Size(166, 40);
            zurueckBtn.TabIndex = 9;
            zurueckBtn.Text = "Zurück";
            zurueckBtn.UseVisualStyleBackColor = true;
            zurueckBtn.Click += zurueckBtn_Click;
            // 
            // Profil
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(984, 561);
            Controls.Add(zurueckBtn);
            Controls.Add(passwortAendernBtn);
            Controls.Add(passwortWiederholenText);
            Controls.Add(passwortWiederholenLabel);
            Controls.Add(neuesPasswortText);
            Controls.Add(neuesPasswortLabel);
            Controls.Add(aktuellesPasswortText);
            Controls.Add(aktuellesPasswortLabel);
            Controls.Add(sicherheitLabel);
            Controls.Add(speichernBtn);
            Controls.Add(erstelltAmValueLabel);
            Controls.Add(erstelltAmLabel);
            Controls.Add(emailText);
            Controls.Add(emailLabel);
            Controls.Add(nachnameText);
            Controls.Add(nachnameLabel);
            Controls.Add(vornameText);
            Controls.Add(vornameLabel);
            Controls.Add(rolleText);
            Controls.Add(rolleLabel);
            Controls.Add(userText);
            Controls.Add(userLabel);
            Controls.Add(profilNameLabel);
            Controls.Add(profilPicture);
            Controls.Add(subtitleLabel);
            Controls.Add(titleLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Profil";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SchulApp - Profil";
            Load += Profil_Load;
            Controls.SetChildIndex(titleLabel, 0);
            Controls.SetChildIndex(subtitleLabel, 0);
            Controls.SetChildIndex(profilPicture, 0);
            Controls.SetChildIndex(profilNameLabel, 0);
            Controls.SetChildIndex(userLabel, 0);
            Controls.SetChildIndex(userText, 0);
            Controls.SetChildIndex(rolleLabel, 0);
            Controls.SetChildIndex(rolleText, 0);
            Controls.SetChildIndex(vornameLabel, 0);
            Controls.SetChildIndex(vornameText, 0);
            Controls.SetChildIndex(nachnameLabel, 0);
            Controls.SetChildIndex(nachnameText, 0);
            Controls.SetChildIndex(emailLabel, 0);
            Controls.SetChildIndex(emailText, 0);
            Controls.SetChildIndex(erstelltAmLabel, 0);
            Controls.SetChildIndex(erstelltAmValueLabel, 0);
            Controls.SetChildIndex(speichernBtn, 0);
            Controls.SetChildIndex(sicherheitLabel, 0);
            Controls.SetChildIndex(aktuellesPasswortLabel, 0);
            Controls.SetChildIndex(aktuellesPasswortText, 0);
            Controls.SetChildIndex(neuesPasswortLabel, 0);
            Controls.SetChildIndex(neuesPasswortText, 0);
            Controls.SetChildIndex(passwortWiederholenLabel, 0);
            Controls.SetChildIndex(passwortWiederholenText, 0);
            Controls.SetChildIndex(passwortAendernBtn, 0);
            Controls.SetChildIndex(zurueckBtn, 0);
            ((System.ComponentModel.ISupportInitialize)profilPicture).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label titleLabel;
        private Label subtitleLabel;
        private PictureBox profilPicture;
        private Label profilNameLabel;
        private Label userLabel;
        private TextBox userText;
        private Label rolleLabel;
        private TextBox rolleText;
        private Label vornameLabel;
        private TextBox vornameText;
        private Label nachnameLabel;
        private TextBox nachnameText;
        private Label emailLabel;
        private TextBox emailText;
        private Label erstelltAmLabel;
        private Label erstelltAmValueLabel;
        private Button speichernBtn;
        private Label sicherheitLabel;
        private Label aktuellesPasswortLabel;
        private TextBox aktuellesPasswortText;
        private Label neuesPasswortLabel;
        private TextBox neuesPasswortText;
        private Label passwortWiederholenLabel;
        private TextBox passwortWiederholenText;
        private Button passwortAendernBtn;
        private Button zurueckBtn;
    }
}
