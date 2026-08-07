namespace SchulApp
{
    partial class Kurse
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
            back = new Button();
            titleLabel = new Label();
            newGroup = new GroupBox();
            createKursBtn = new Button();
            newKursLehrer = new ComboBox();
            newLehrerLabel = new Label();
            newKursKlasse = new ComboBox();
            newKlasseLabel = new Label();
            newKursName = new TextBox();
            newNameLabel = new Label();
            editGroup = new GroupBox();
            updateKursBtn = new Button();
            editKursLehrer = new ComboBox();
            editLehrerLabel = new Label();
            editKursKlasse = new ComboBox();
            editKlasseLabel = new Label();
            editKursName = new TextBox();
            editNameLabel = new Label();
            deleteGroup = new GroupBox();
            deleteKursBtn = new Button();
            deleteKursName = new TextBox();
            deleteLabel = new Label();
            reloadBtn = new Button();
            kurseGrid = new DataGridView();
            newGroup.SuspendLayout();
            editGroup.SuspendLayout();
            deleteGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)kurseGrid).BeginInit();
            SuspendLayout();
            // 
            // back
            // 
            back.Location = new Point(900, 18);
            back.Name = "back";
            back.Size = new Size(72, 27);
            back.TabIndex = 0;
            back.Text = "zurück";
            back.UseVisualStyleBackColor = true;
            back.Click += back_Click;
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            titleLabel.Location = new Point(25, 18);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(197, 32);
            titleLabel.TabIndex = 1;
            titleLabel.Text = "Kurse verwalten";
            // 
            // newGroup
            // 
            newGroup.Controls.Add(createKursBtn);
            newGroup.Controls.Add(newKursLehrer);
            newGroup.Controls.Add(newLehrerLabel);
            newGroup.Controls.Add(newKursKlasse);
            newGroup.Controls.Add(newKlasseLabel);
            newGroup.Controls.Add(newKursName);
            newGroup.Controls.Add(newNameLabel);
            newGroup.Location = new Point(25, 70);
            newGroup.Name = "newGroup";
            newGroup.Size = new Size(315, 190);
            newGroup.TabIndex = 2;
            newGroup.TabStop = false;
            newGroup.Text = "Neuer Kurs";
            // 
            // createKursBtn
            // 
            createKursBtn.Location = new Point(210, 145);
            createKursBtn.Name = "createKursBtn";
            createKursBtn.Size = new Size(90, 30);
            createKursBtn.TabIndex = 6;
            createKursBtn.Text = "Speichern";
            createKursBtn.UseVisualStyleBackColor = true;
            createKursBtn.Click += createKursBtn_Click;
            // 
            // newKursLehrer
            // 
            newKursLehrer.DropDownStyle = ComboBoxStyle.DropDownList;
            newKursLehrer.FormattingEnabled = true;
            newKursLehrer.Location = new Point(15, 147);
            newKursLehrer.Name = "newKursLehrer";
            newKursLehrer.Size = new Size(185, 23);
            newKursLehrer.TabIndex = 5;
            // 
            // newLehrerLabel
            // 
            newLehrerLabel.AutoSize = true;
            newLehrerLabel.Location = new Point(15, 128);
            newLehrerLabel.Name = "newLehrerLabel";
            newLehrerLabel.Size = new Size(40, 15);
            newLehrerLabel.TabIndex = 4;
            newLehrerLabel.Text = "Lehrer";
            // 
            // newKursKlasse
            // 
            newKursKlasse.DropDownStyle = ComboBoxStyle.DropDownList;
            newKursKlasse.FormattingEnabled = true;
            newKursKlasse.Location = new Point(15, 94);
            newKursKlasse.Name = "newKursKlasse";
            newKursKlasse.Size = new Size(285, 23);
            newKursKlasse.TabIndex = 3;
            // 
            // newKlasseLabel
            // 
            newKlasseLabel.AutoSize = true;
            newKlasseLabel.Location = new Point(15, 75);
            newKlasseLabel.Name = "newKlasseLabel";
            newKlasseLabel.Size = new Size(41, 15);
            newKlasseLabel.TabIndex = 2;
            newKlasseLabel.Text = "Klasse";
            // 
            // newKursName
            // 
            newKursName.Location = new Point(15, 45);
            newKursName.Name = "newKursName";
            newKursName.Size = new Size(285, 23);
            newKursName.TabIndex = 1;
            // 
            // newNameLabel
            // 
            newNameLabel.AutoSize = true;
            newNameLabel.Location = new Point(15, 26);
            newNameLabel.Name = "newNameLabel";
            newNameLabel.Size = new Size(60, 15);
            newNameLabel.TabIndex = 0;
            newNameLabel.Text = "Kursname";
            // 
            // editGroup
            // 
            editGroup.Controls.Add(updateKursBtn);
            editGroup.Controls.Add(editKursLehrer);
            editGroup.Controls.Add(editLehrerLabel);
            editGroup.Controls.Add(editKursKlasse);
            editGroup.Controls.Add(editKlasseLabel);
            editGroup.Controls.Add(editKursName);
            editGroup.Controls.Add(editNameLabel);
            editGroup.Location = new Point(25, 270);
            editGroup.Name = "editGroup";
            editGroup.Size = new Size(315, 190);
            editGroup.TabIndex = 3;
            editGroup.TabStop = false;
            editGroup.Text = "Ausgewählten Kurs bearbeiten";
            // 
            // updateKursBtn
            // 
            updateKursBtn.Location = new Point(210, 145);
            updateKursBtn.Name = "updateKursBtn";
            updateKursBtn.Size = new Size(90, 30);
            updateKursBtn.TabIndex = 6;
            updateKursBtn.Text = "Ändern";
            updateKursBtn.UseVisualStyleBackColor = true;
            updateKursBtn.Click += updateKursBtn_Click;
            // 
            // editKursLehrer
            // 
            editKursLehrer.DropDownStyle = ComboBoxStyle.DropDownList;
            editKursLehrer.FormattingEnabled = true;
            editKursLehrer.Location = new Point(15, 147);
            editKursLehrer.Name = "editKursLehrer";
            editKursLehrer.Size = new Size(185, 23);
            editKursLehrer.TabIndex = 5;
            // 
            // editLehrerLabel
            // 
            editLehrerLabel.AutoSize = true;
            editLehrerLabel.Location = new Point(15, 128);
            editLehrerLabel.Name = "editLehrerLabel";
            editLehrerLabel.Size = new Size(40, 15);
            editLehrerLabel.TabIndex = 4;
            editLehrerLabel.Text = "Lehrer";
            // 
            // editKursKlasse
            // 
            editKursKlasse.DropDownStyle = ComboBoxStyle.DropDownList;
            editKursKlasse.FormattingEnabled = true;
            editKursKlasse.Location = new Point(15, 94);
            editKursKlasse.Name = "editKursKlasse";
            editKursKlasse.Size = new Size(285, 23);
            editKursKlasse.TabIndex = 3;
            // 
            // editKlasseLabel
            // 
            editKlasseLabel.AutoSize = true;
            editKlasseLabel.Location = new Point(15, 75);
            editKlasseLabel.Name = "editKlasseLabel";
            editKlasseLabel.Size = new Size(41, 15);
            editKlasseLabel.TabIndex = 2;
            editKlasseLabel.Text = "Klasse";
            // 
            // editKursName
            // 
            editKursName.Location = new Point(15, 45);
            editKursName.Name = "editKursName";
            editKursName.Size = new Size(285, 23);
            editKursName.TabIndex = 1;
            // 
            // editNameLabel
            // 
            editNameLabel.AutoSize = true;
            editNameLabel.Location = new Point(15, 26);
            editNameLabel.Name = "editNameLabel";
            editNameLabel.Size = new Size(60, 15);
            editNameLabel.TabIndex = 0;
            editNameLabel.Text = "Kursname";
            // 
            // deleteGroup
            // 
            deleteGroup.Controls.Add(deleteKursBtn);
            deleteGroup.Controls.Add(deleteKursName);
            deleteGroup.Controls.Add(deleteLabel);
            deleteGroup.Location = new Point(25, 470);
            deleteGroup.Name = "deleteGroup";
            deleteGroup.Size = new Size(315, 72);
            deleteGroup.TabIndex = 4;
            deleteGroup.TabStop = false;
            deleteGroup.Text = "Ausgewählten Kurs löschen";
            // 
            // deleteKursBtn
            // 
            deleteKursBtn.Location = new Point(210, 28);
            deleteKursBtn.Name = "deleteKursBtn";
            deleteKursBtn.Size = new Size(90, 30);
            deleteKursBtn.TabIndex = 2;
            deleteKursBtn.Text = "Löschen";
            deleteKursBtn.UseVisualStyleBackColor = true;
            deleteKursBtn.Click += deleteKursBtn_Click;
            // 
            // deleteKursName
            // 
            deleteKursName.Location = new Point(15, 32);
            deleteKursName.Name = "deleteKursName";
            deleteKursName.ReadOnly = true;
            deleteKursName.Size = new Size(185, 23);
            deleteKursName.TabIndex = 1;
            // 
            // deleteLabel
            // 
            deleteLabel.AutoSize = true;
            deleteLabel.Location = new Point(15, 14);
            deleteLabel.Name = "deleteLabel";
            deleteLabel.Size = new Size(31, 15);
            deleteLabel.TabIndex = 0;
            deleteLabel.Text = "Kurs";
            // 
            // reloadBtn
            // 
            reloadBtn.Location = new Point(365, 505);
            reloadBtn.Name = "reloadBtn";
            reloadBtn.Size = new Size(607, 32);
            reloadBtn.TabIndex = 5;
            reloadBtn.Text = "Kurse, Klassen und Lehrer neu laden";
            reloadBtn.UseVisualStyleBackColor = true;
            reloadBtn.Click += reloadBtn_Click;
            // 
            // kurseGrid
            // 
            kurseGrid.AllowUserToAddRows = false;
            kurseGrid.AllowUserToDeleteRows = false;
            kurseGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            kurseGrid.Location = new Point(365, 70);
            kurseGrid.MultiSelect = false;
            kurseGrid.Name = "kurseGrid";
            kurseGrid.ReadOnly = true;
            kurseGrid.RowHeadersVisible = false;
            kurseGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            kurseGrid.Size = new Size(607, 420);
            kurseGrid.TabIndex = 6;
            kurseGrid.SelectionChanged += kurseGrid_SelectionChanged;
            // 
            // Kurse
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(984, 561);
            Controls.Add(kurseGrid);
            Controls.Add(reloadBtn);
            Controls.Add(deleteGroup);
            Controls.Add(editGroup);
            Controls.Add(newGroup);
            Controls.Add(titleLabel);
            Controls.Add(back);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Kurse";
            Text = "SchulApp - Kurse";
            newGroup.ResumeLayout(false);
            newGroup.PerformLayout();
            editGroup.ResumeLayout(false);
            editGroup.PerformLayout();
            deleteGroup.ResumeLayout(false);
            deleteGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)kurseGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Button back;
        private Label titleLabel;
        private GroupBox newGroup;
        private Button createKursBtn;
        private ComboBox newKursLehrer;
        private Label newLehrerLabel;
        private ComboBox newKursKlasse;
        private Label newKlasseLabel;
        private TextBox newKursName;
        private Label newNameLabel;
        private GroupBox editGroup;
        private Button updateKursBtn;
        private ComboBox editKursLehrer;
        private Label editLehrerLabel;
        private ComboBox editKursKlasse;
        private Label editKlasseLabel;
        private TextBox editKursName;
        private Label editNameLabel;
        private GroupBox deleteGroup;
        private Button deleteKursBtn;
        private TextBox deleteKursName;
        private Label deleteLabel;
        private Button reloadBtn;
        private DataGridView kurseGrid;
    }
}
