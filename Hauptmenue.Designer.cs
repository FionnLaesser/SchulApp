namespace SchulApp
{
    partial class Hauptmenue
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Hauptmenue));
            lehrerPage = new Button();
            schuelerPage = new Button();
            startBild = new PictureBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)startBild).BeginInit();
            SuspendLayout();
            // 
            // lehrerPage
            // 
            lehrerPage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lehrerPage.Location = new Point(398, 214);
            lehrerPage.Name = "lehrerPage";
            lehrerPage.Size = new Size(75, 23);
            lehrerPage.TabIndex = 0;
            lehrerPage.Text = "Lehrer";
            lehrerPage.UseVisualStyleBackColor = true;
            lehrerPage.Click += lehrerPage_Click;
            // 
            // schuelerPage
            // 
            schuelerPage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            schuelerPage.Location = new Point(398, 243);
            schuelerPage.Name = "schuelerPage";
            schuelerPage.Size = new Size(75, 23);
            schuelerPage.TabIndex = 1;
            schuelerPage.Text = "Schüler";
            schuelerPage.UseVisualStyleBackColor = true;
            schuelerPage.Click += schuelerPage_Click;
            // 
            // startBild
            // 
            startBild.Image = (Image)resources.GetObject("startBild.Image");
            startBild.InitialImage = (Image)resources.GetObject("startBild.InitialImage");
            startBild.Location = new Point(0, 0);
            startBild.Name = "startBild";
            startBild.Size = new Size(1000, 600);
            startBild.SizeMode = PictureBoxSizeMode.StretchImage;
            startBild.TabIndex = 2;
            startBild.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 50F);
            label1.Location = new Point(88, 33);
            label1.Name = "label1";
            label1.Size = new Size(792, 178);
            label1.TabIndex = 3;
            label1.Text = "Herzlich Willkommen zur \r\nSchülerverwaltungs App";
            // 
            // Hauptmenue
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(984, 561);
            Controls.Add(startBild);
            Controls.Add(schuelerPage);
            Controls.Add(lehrerPage);
            Controls.Add(label1);
            Name = "Hauptmenue";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)startBild).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button lehrerPage;
        private Button schuelerPage;
        private PictureBox startBild;
        private Label label1;
    }
}