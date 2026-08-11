namespace SchulApp
{
    partial class Register
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            leftPanel = new Panel();
            brandSubtitleLabel = new Label();
            brandLabel = new Label();
            cardPanel = new Panel();
            backToLoginBtn = new Button();
            registerBtn = new Button();
            repeatPasswordText = new TextBox();
            repeatPasswordLabel = new Label();
            passwordText = new TextBox();
            passwordLabel = new Label();
            userText = new TextBox();
            userLabel = new Label();
            subtitleLabel = new Label();
            titleLabel = new Label();
            leftPanel.SuspendLayout();
            cardPanel.SuspendLayout();
            SuspendLayout();
            // 
            // leftPanel
            // 
            leftPanel.BackColor = Color.FromArgb(31, 65, 114);
            leftPanel.Controls.Add(brandSubtitleLabel);
            leftPanel.Controls.Add(brandLabel);
            leftPanel.Dock = DockStyle.Left;
            leftPanel.Location = new Point(0, 0);
            leftPanel.Name = "leftPanel";
            leftPanel.Size = new Size(340, 560);
            leftPanel.TabIndex = 0;
            // 
            // brandSubtitleLabel
            // 
            brandSubtitleLabel.Font = new Font("Segoe UI", 11F);
            brandSubtitleLabel.ForeColor = Color.FromArgb(218, 228, 243);
            brandSubtitleLabel.Location = new Point(48, 273);
            brandSubtitleLabel.Name = "brandSubtitleLabel";
            brandSubtitleLabel.Size = new Size(244, 58);
            brandSubtitleLabel.TabIndex = 1;
            brandSubtitleLabel.Text = "Erstelle dein Konto und starte direkt mit der Schulverwaltung.";
            // 
            // brandLabel
            // 
            brandLabel.AutoSize = true;
            brandLabel.Font = new Font("Segoe UI Semibold", 28F, FontStyle.Bold);
            brandLabel.ForeColor = Color.White;
            brandLabel.Location = new Point(46, 207);
            brandLabel.Name = "brandLabel";
            brandLabel.Size = new Size(184, 51);
            brandLabel.TabIndex = 0;
            brandLabel.Text = "SchulApp";
            // 
            // cardPanel
            // 
            cardPanel.BackColor = Color.White;
            cardPanel.Controls.Add(backToLoginBtn);
            cardPanel.Controls.Add(registerBtn);
            cardPanel.Controls.Add(repeatPasswordText);
            cardPanel.Controls.Add(repeatPasswordLabel);
            cardPanel.Controls.Add(passwordText);
            cardPanel.Controls.Add(passwordLabel);
            cardPanel.Controls.Add(userText);
            cardPanel.Controls.Add(userLabel);
            cardPanel.Controls.Add(subtitleLabel);
            cardPanel.Controls.Add(titleLabel);
            cardPanel.Location = new Point(404, 38);
            cardPanel.Name = "cardPanel";
            cardPanel.Padding = new Padding(36);
            cardPanel.Size = new Size(430, 485);
            cardPanel.TabIndex = 1;
            // 
            // backToLoginBtn
            // 
            backToLoginBtn.BackColor = Color.White;
            backToLoginBtn.Cursor = Cursors.Hand;
            backToLoginBtn.FlatAppearance.BorderColor = Color.FromArgb(31, 65, 114);
            backToLoginBtn.FlatAppearance.BorderSize = 1;
            backToLoginBtn.FlatStyle = FlatStyle.Flat;
            backToLoginBtn.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            backToLoginBtn.ForeColor = Color.FromArgb(31, 65, 114);
            backToLoginBtn.Location = new Point(43, 402);
            backToLoginBtn.Name = "backToLoginBtn";
            backToLoginBtn.Size = new Size(344, 42);
            backToLoginBtn.TabIndex = 5;
            backToLoginBtn.Text = "Zurück zum Login";
            backToLoginBtn.UseVisualStyleBackColor = false;
            backToLoginBtn.Click += backToLoginBtn_Click;
            // 
            // registerBtn
            // 
            registerBtn.BackColor = Color.FromArgb(31, 65, 114);
            registerBtn.Cursor = Cursors.Hand;
            registerBtn.FlatAppearance.BorderSize = 0;
            registerBtn.FlatStyle = FlatStyle.Flat;
            registerBtn.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            registerBtn.ForeColor = Color.White;
            registerBtn.Location = new Point(43, 348);
            registerBtn.Name = "registerBtn";
            registerBtn.Size = new Size(344, 42);
            registerBtn.TabIndex = 4;
            registerBtn.Text = "Konto erstellen";
            registerBtn.UseVisualStyleBackColor = false;
            registerBtn.Click += registerBtn_Click;
            // 
            // repeatPasswordText
            // 
            repeatPasswordText.BackColor = Color.FromArgb(247, 249, 252);
            repeatPasswordText.BorderStyle = BorderStyle.FixedSingle;
            repeatPasswordText.Font = new Font("Segoe UI", 11F);
            repeatPasswordText.Location = new Point(43, 300);
            repeatPasswordText.Name = "repeatPasswordText";
            repeatPasswordText.PlaceholderText = "Passwort wiederholen";
            repeatPasswordText.Size = new Size(344, 27);
            repeatPasswordText.TabIndex = 3;
            // 
            // repeatPasswordLabel
            // 
            repeatPasswordLabel.AutoSize = true;
            repeatPasswordLabel.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            repeatPasswordLabel.ForeColor = Color.FromArgb(45, 55, 72);
            repeatPasswordLabel.Location = new Point(43, 276);
            repeatPasswordLabel.Name = "repeatPasswordLabel";
            repeatPasswordLabel.Size = new Size(136, 17);
            repeatPasswordLabel.TabIndex = 7;
            repeatPasswordLabel.Text = "Passwort wiederholen";
            // 
            // passwordText
            // 
            passwordText.BackColor = Color.FromArgb(247, 249, 252);
            passwordText.BorderStyle = BorderStyle.FixedSingle;
            passwordText.Font = new Font("Segoe UI", 11F);
            passwordText.Location = new Point(43, 230);
            passwordText.Name = "passwordText";
            passwordText.PlaceholderText = "Mindestens 8 Zeichen";
            passwordText.Size = new Size(344, 27);
            passwordText.TabIndex = 2;
            // 
            // passwordLabel
            // 
            passwordLabel.AutoSize = true;
            passwordLabel.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            passwordLabel.ForeColor = Color.FromArgb(45, 55, 72);
            passwordLabel.Location = new Point(43, 206);
            passwordLabel.Name = "passwordLabel";
            passwordLabel.Size = new Size(64, 17);
            passwordLabel.TabIndex = 5;
            passwordLabel.Text = "Passwort";
            // 
            // userText
            // 
            userText.BackColor = Color.FromArgb(247, 249, 252);
            userText.BorderStyle = BorderStyle.FixedSingle;
            userText.Font = new Font("Segoe UI", 11F);
            userText.Location = new Point(43, 160);
            userText.MaxLength = 50;
            userText.Name = "userText";
            userText.PlaceholderText = "Benutzername wählen";
            userText.Size = new Size(344, 27);
            userText.TabIndex = 1;
            // 
            // userLabel
            // 
            userLabel.AutoSize = true;
            userLabel.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            userLabel.ForeColor = Color.FromArgb(45, 55, 72);
            userLabel.Location = new Point(43, 136);
            userLabel.Name = "userLabel";
            userLabel.Size = new Size(94, 17);
            userLabel.TabIndex = 4;
            userLabel.Text = "Benutzername";
            // 
            // subtitleLabel
            // 
            subtitleLabel.AutoSize = true;
            subtitleLabel.Font = new Font("Segoe UI", 10F);
            subtitleLabel.ForeColor = Color.FromArgb(100, 116, 139);
            subtitleLabel.Location = new Point(43, 91);
            subtitleLabel.Name = "subtitleLabel";
            subtitleLabel.Size = new Size(237, 19);
            subtitleLabel.TabIndex = 1;
            subtitleLabel.Text = "Erstelle ein neues Benutzerkonto.";
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI Semibold", 24F, FontStyle.Bold);
            titleLabel.ForeColor = Color.FromArgb(20, 33, 61);
            titleLabel.Location = new Point(39, 37);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(206, 45);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Registrieren";
            // 
            // Register
            // 
            AcceptButton = registerBtn;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(238, 243, 250);
            ClientSize = new Size(900, 560);
            Controls.Add(cardPanel);
            Controls.Add(leftPanel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Register";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SchulApp - Registrieren";
            leftPanel.ResumeLayout(false);
            leftPanel.PerformLayout();
            cardPanel.ResumeLayout(false);
            cardPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel leftPanel;
        private Label brandLabel;
        private Label brandSubtitleLabel;
        private Panel cardPanel;
        private Label titleLabel;
        private Label subtitleLabel;
        private Label userLabel;
        private TextBox userText;
        private Label passwordLabel;
        private TextBox passwordText;
        private Label repeatPasswordLabel;
        private TextBox repeatPasswordText;
        private Button registerBtn;
        private Button backToLoginBtn;
    }
}
