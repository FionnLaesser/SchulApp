namespace SchulApp
{
    partial class Bestenliste
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
            titleLabel = new Label();
            bestenlisteGridView = new DataGridView();
            benutzernameColumn = new DataGridViewTextBoxColumn();
            siegeColumn = new DataGridViewTextBoxColumn();
            punkteColumn = new DataGridViewTextBoxColumn();
            toreErzieltColumn = new DataGridViewTextBoxColumn();
            toreKassiertColumn = new DataGridViewTextBoxColumn();
            torverhaeltnisColumn = new DataGridViewTextBoxColumn();
            statusLabel = new Label();
            zurueckBtn = new Button();
            ((System.ComponentModel.ISupportInitialize)bestenlisteGridView).BeginInit();
            SuspendLayout();

            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            titleLabel.Location = new Point(30, 30);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(206, 45);
            titleLabel.TabIndex = 0;
            titleLabel.Text = "Bestenliste";

            // 
            // bestenlisteGridView
            // 
            bestenlisteGridView.AllowUserToAddRows = false;
            bestenlisteGridView.AllowUserToDeleteRows = false;
            bestenlisteGridView.AllowUserToResizeRows = false;
            bestenlisteGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            bestenlisteGridView.AutoGenerateColumns = false;
            bestenlisteGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            bestenlisteGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            bestenlisteGridView.Columns.AddRange(new DataGridViewColumn[] {
                benutzernameColumn,
                siegeColumn,
                punkteColumn,
                toreErzieltColumn,
                toreKassiertColumn,
                torverhaeltnisColumn
            });
            bestenlisteGridView.Location = new Point(30, 95);
            bestenlisteGridView.MultiSelect = false;
            bestenlisteGridView.Name = "bestenlisteGridView";
            bestenlisteGridView.ReadOnly = true;
            bestenlisteGridView.RowHeadersVisible = false;
            bestenlisteGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            bestenlisteGridView.Size = new Size(924, 380);
            bestenlisteGridView.TabIndex = 1;

            // 
            // benutzernameColumn
            // 
            benutzernameColumn.DataPropertyName = "Benutzername";
            benutzernameColumn.HeaderText = "Benutzername";
            benutzernameColumn.Name = "benutzernameColumn";
            benutzernameColumn.ReadOnly = true;

            // 
            // siegeColumn
            // 
            siegeColumn.DataPropertyName = "Siege";
            siegeColumn.HeaderText = "Siege";
            siegeColumn.Name = "siegeColumn";
            siegeColumn.ReadOnly = true;

            // 
            // punkteColumn
            // 
            punkteColumn.DataPropertyName = "Punkte";
            punkteColumn.HeaderText = "Punkte";
            punkteColumn.Name = "punkteColumn";
            punkteColumn.ReadOnly = true;

            // 
            // toreErzieltColumn
            // 
            toreErzieltColumn.DataPropertyName = "ToreErzielt";
            toreErzieltColumn.HeaderText = "Erzielte Tore";
            toreErzieltColumn.Name = "toreErzieltColumn";
            toreErzieltColumn.ReadOnly = true;

            // 
            // toreKassiertColumn
            // 
            toreKassiertColumn.DataPropertyName = "ToreKassiert";
            toreKassiertColumn.HeaderText = "Kassierte Tore";
            toreKassiertColumn.Name = "toreKassiertColumn";
            toreKassiertColumn.ReadOnly = true;

            // 
            // torverhaeltnisColumn
            // 
            torverhaeltnisColumn.DataPropertyName = "Torverhaeltnis";
            torverhaeltnisColumn.HeaderText = "Torverhältnis";
            torverhaeltnisColumn.Name = "torverhaeltnisColumn";
            torverhaeltnisColumn.ReadOnly = true;

            // 
            // statusLabel
            // 
            statusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            statusLabel.AutoSize = true;
            statusLabel.Location = new Point(30, 500);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(151, 15);
            statusLabel.TabIndex = 2;
            statusLabel.Text = "Bestenliste wird geladen...";

            // 
            // zurueckBtn
            // 
            zurueckBtn.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            zurueckBtn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            zurueckBtn.Location = new Point(824, 492);
            zurueckBtn.Name = "zurueckBtn";
            zurueckBtn.Size = new Size(130, 38);
            zurueckBtn.TabIndex = 3;
            zurueckBtn.Text = "Zurück";
            zurueckBtn.UseVisualStyleBackColor = true;
            zurueckBtn.Click += zurueckBtn_Click;

            // 
            // Bestenliste
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 561);
            Controls.Add(zurueckBtn);
            Controls.Add(statusLabel);
            Controls.Add(bestenlisteGridView);
            Controls.Add(titleLabel);
            MinimumSize = new Size(800, 500);
            Name = "Bestenliste";
            StartPosition = FormStartPosition.Manual;
            Text = "SchulApp - Bestenliste";
            Shown += Bestenliste_Shown;
            Controls.SetChildIndex(titleLabel, 0);
            Controls.SetChildIndex(bestenlisteGridView, 0);
            Controls.SetChildIndex(statusLabel, 0);
            Controls.SetChildIndex(zurueckBtn, 0);
            ((System.ComponentModel.ISupportInitialize)bestenlisteGridView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label titleLabel;
        private DataGridView bestenlisteGridView;
        private DataGridViewTextBoxColumn benutzernameColumn;
        private DataGridViewTextBoxColumn siegeColumn;
        private DataGridViewTextBoxColumn punkteColumn;
        private DataGridViewTextBoxColumn toreErzieltColumn;
        private DataGridViewTextBoxColumn toreKassiertColumn;
        private DataGridViewTextBoxColumn torverhaeltnisColumn;
        private Label statusLabel;
        private Button zurueckBtn;
    }
}
