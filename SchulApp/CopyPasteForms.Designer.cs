namespace SchulApp
{
    partial class CopyPasteForms
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
            SuspendLayout();

            // 
            // CopyPasteForms
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;

            ClientSize = new Size(1200, 750);

            MinimumSize = new Size(900, 600);

            Name = "CopyPasteForms";
            Text = "SchulApp";

            StartPosition = FormStartPosition.Manual;

            Load += CopyPasteForms_Load;

            ResumeLayout(false);
        }

        #endregion
    }
}