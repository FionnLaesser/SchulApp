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
            // Einstellungen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(984, 561);
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
    }
}
