namespace SchulApp
{
    partial class AuditLog
    {
        private System.ComponentModel.IContainer components = null;

        private DataGridView auditGrid;
        private TextBox detailsTextBox;
        private Button reloadButton;
        private Label infoLabel;

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
            auditGrid = new DataGridView();
            detailsTextBox = new TextBox();
            reloadButton = new Button();
            infoLabel = new Label();
            backBtn = new Button();
            ((System.ComponentModel.ISupportInitialize)auditGrid).BeginInit();
            SuspendLayout();
            // 
            // auditGrid
            // 
            auditGrid.AllowUserToAddRows = false;
            auditGrid.AllowUserToDeleteRows = false;
            auditGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            auditGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            auditGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            auditGrid.Location = new Point(20, 105);
            auditGrid.MultiSelect = false;
            auditGrid.Name = "auditGrid";
            auditGrid.ReadOnly = true;
            auditGrid.RowHeadersWidth = 51;
            auditGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            auditGrid.Size = new Size(1050, 355);
            auditGrid.TabIndex = 2;
            auditGrid.SelectionChanged += auditGrid_SelectionChanged;
            // 
            // detailsTextBox
            // 
            detailsTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            detailsTextBox.Font = new Font("Consolas", 10F);
            detailsTextBox.Location = new Point(20, 480);
            detailsTextBox.Multiline = true;
            detailsTextBox.Name = "detailsTextBox";
            detailsTextBox.ReadOnly = true;
            detailsTextBox.ScrollBars = ScrollBars.Vertical;
            detailsTextBox.Size = new Size(1050, 145);
            detailsTextBox.TabIndex = 3;
            // 
            // reloadButton
            // 
            reloadButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            reloadButton.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            reloadButton.Location = new Point(950, 52);
            reloadButton.Name = "reloadButton";
            reloadButton.Size = new Size(120, 36);
            reloadButton.TabIndex = 1;
            reloadButton.Text = "Neu laden";
            reloadButton.UseVisualStyleBackColor = true;
            reloadButton.Click += reloadButton_Click;
            // 
            // infoLabel
            // 
            infoLabel.AutoSize = true;
            infoLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            infoLabel.Location = new Point(20, 60);
            infoLabel.Name = "infoLabel";
            infoLabel.Size = new Size(375, 21);
            infoLabel.TabIndex = 0;
            infoLabel.Text = "Audit Log: letzte 500 protokollierte Änderungen";
            // 
            // backBtn
            // 
            backBtn.Location = new Point(824, 52);
            backBtn.Name = "backBtn";
            backBtn.Size = new Size(120, 36);
            backBtn.TabIndex = 4;
            backBtn.Text = "zurück";
            backBtn.UseVisualStyleBackColor = true;
            backBtn.Click += this.backBtn_Click;
            // 
            // AuditLog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 650);
            Controls.Add(backBtn);
            Controls.Add(detailsTextBox);
            Controls.Add(auditGrid);
            Controls.Add(reloadButton);
            Controls.Add(infoLabel);
            Name = "AuditLog";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SchulApp - Audit Log";
            Shown += AuditLog_Shown;
            Controls.SetChildIndex(infoLabel, 0);
            Controls.SetChildIndex(reloadButton, 0);
            Controls.SetChildIndex(auditGrid, 0);
            Controls.SetChildIndex(detailsTextBox, 0);
            Controls.SetChildIndex(backBtn, 0);
            ((System.ComponentModel.ISupportInitialize)auditGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button backBtn;
    }
}