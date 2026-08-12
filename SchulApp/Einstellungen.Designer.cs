namespace SchulApp
{
    partial class Einstellungen
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
            colorDialogBgColor = new ColorDialog();
            backgroundColorBtn = new Button();
            textBgChooser = new Label();
            back_Click = new Button();
            textfarbeAendernLabel = new Label();
            textfarbeBtn = new Button();
            colorDialogTextFarbe = new ColorDialog();
            resetSettingsBtn = new Button();
            label1 = new Label();
            userNameLabel = new Label();
            SuspendLayout();
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 30F, FontStyle.Bold);
            titleLabel.Location = new Point(12, 9);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(276, 54);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Einstellungen";
            // 
            // subtitleLabel
            // 
            subtitleLabel.AutoSize = true;
            subtitleLabel.Font = new Font("Segoe UI", 12F);
            subtitleLabel.Location = new Point(12, 63);
            subtitleLabel.Name = "subtitleLabel";
            subtitleLabel.Size = new Size(338, 21);
            subtitleLabel.TabIndex = 1;
            subtitleLabel.Text = "Hier kannst du alle deine Präferenzen einstellen";
            // 
            // backgroundColorBtn
            // 
            backgroundColorBtn.Location = new Point(73, 143);
            backgroundColorBtn.Name = "backgroundColorBtn";
            backgroundColorBtn.Size = new Size(75, 49);
            backgroundColorBtn.TabIndex = 2;
            backgroundColorBtn.Text = "Farbe auswählen";
            backgroundColorBtn.UseVisualStyleBackColor = true;
            backgroundColorBtn.Click += backgroundColorBtn_Click;
            // 
            // textBgChooser
            // 
            textBgChooser.AutoSize = true;
            textBgChooser.Location = new Point(12, 110);
            textBgChooser.Name = "textBgChooser";
            textBgChooser.Size = new Size(309, 30);
            textBgChooser.TabIndex = 3;
            textBgChooser.Text = "Hintergrundfarbe wählen \r\n(Achtung: Dies könnte ihr Benutzererlebnis einschränken)";
            // 
            // back_Click
            // 
            back_Click.Location = new Point(897, 9);
            back_Click.Name = "back_Click";
            back_Click.Size = new Size(75, 23);
            back_Click.TabIndex = 4;
            back_Click.Text = "Zurück";
            back_Click.UseVisualStyleBackColor = true;
            back_Click.Click += back_Click_Click;
            // 
            // textfarbeAendernLabel
            // 
            textfarbeAendernLabel.AutoSize = true;
            textfarbeAendernLabel.Location = new Point(12, 205);
            textfarbeAendernLabel.Name = "textfarbeAendernLabel";
            textfarbeAendernLabel.Size = new Size(309, 30);
            textfarbeAendernLabel.TabIndex = 5;
            textfarbeAendernLabel.Text = "Textfarbe wählen \r\n(Achtung: Dies könnte ihr Benutzererlebnis einschränken)";
            // 
            // textfarbeBtn
            // 
            textfarbeBtn.Location = new Point(73, 238);
            textfarbeBtn.Name = "textfarbeBtn";
            textfarbeBtn.Size = new Size(75, 49);
            textfarbeBtn.TabIndex = 6;
            textfarbeBtn.Text = "Farbe auswählen";
            textfarbeBtn.UseVisualStyleBackColor = true;
            textfarbeBtn.Click += textfarbeBtn_Click;
            // 
            // resetSettingsBtn
            // 
            resetSettingsBtn.Location = new Point(416, 64);
            resetSettingsBtn.Name = "resetSettingsBtn";
            resetSettingsBtn.Size = new Size(93, 23);
            resetSettingsBtn.TabIndex = 7;
            resetSettingsBtn.Text = "Zurücksetzen";
            resetSettingsBtn.UseVisualStyleBackColor = true;
            resetSettingsBtn.Click += resetSettingsBtn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(390, 28);
            label1.Name = "label1";
            label1.Size = new Size(159, 30);
            label1.TabIndex = 8;
            label1.Text = "Einstellungen Zurücksetzen? \r\nDrück hier auf den Knopf";
            // 
            // userNameLabel
            // 
            userNameLabel.AutoSize = true;
            userNameLabel.Location = new Point(646, 9);
            userNameLabel.Name = "userNameLabel";
            userNameLabel.Size = new Size(33, 15);
            userNameLabel.TabIndex = 9;
            userNameLabel.Text = "User:";
            // 
            // Einstellungen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(984, 561);
            Controls.Add(userNameLabel);
            Controls.Add(label1);
            Controls.Add(resetSettingsBtn);
            Controls.Add(textfarbeBtn);
            Controls.Add(textfarbeAendernLabel);
            Controls.Add(back_Click);
            Controls.Add(textBgChooser);
            Controls.Add(backgroundColorBtn);
            Controls.Add(subtitleLabel);
            Controls.Add(titleLabel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Einstellungen";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SchulApp - Hauptmenü";
            ResumeLayout(false);
            PerformLayout();
        }

        private Label titleLabel;
        private Label subtitleLabel;
        private ColorDialog colorDialogBgColor;
        private Button backgroundColorBtn;
        private Label textBgChooser;
        private Button back_Click;
        private Label textfarbeAendernLabel;
        private Button textfarbeBtn;
        private ColorDialog colorDialogTextFarbe;
        private Button resetSettingsBtn;
        private Label label1;
        private Label userNameLabel;
    }
}
