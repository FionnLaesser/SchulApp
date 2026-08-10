namespace SchulApp
{
    partial class Klassen
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
            createKlasseBtn = new Button();
            newKlassenlehrer = new ComboBox();
            newTeacherLabel = new Label();
            newKlasseName = new TextBox();
            newNameLabel = new Label();
            editGroup = new GroupBox();
            updateKlasseBtn = new Button();
            editKlassenlehrer = new ComboBox();
            editTeacherLabel = new Label();
            editKlasseName = new TextBox();
            editNameLabel = new Label();
            deleteGroup = new GroupBox();
            deleteKlasseBtn = new Button();
            deleteKlasseName = new TextBox();
            deleteLabel = new Label();
            reloadBtn = new Button();
            klassenGrid = new DataGridView();
            studentLabel = new Label();
            klassenSchuelerGrid = new DataGridView();
            newGroup.SuspendLayout();
            editGroup.SuspendLayout();
            deleteGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)klassenGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)klassenSchuelerGrid).BeginInit();
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
            titleLabel.Size = new Size(220, 32);
            titleLabel.TabIndex = 1;
            titleLabel.Text = "Klassen verwalten";
            // 
            // newGroup
            // 
            newGroup.Controls.Add(createKlasseBtn);
            newGroup.Controls.Add(newKlassenlehrer);
            newGroup.Controls.Add(newTeacherLabel);
            newGroup.Controls.Add(newKlasseName);
            newGroup.Controls.Add(newNameLabel);
            newGroup.Location = new Point(25, 70);
            newGroup.Name = "newGroup";
            newGroup.Size = new Size(315, 150);
            newGroup.TabIndex = 2;
            newGroup.TabStop = false;
            newGroup.Text = "Neue Klasse";
            // 
            // createKlasseBtn
            // 
            createKlasseBtn.Location = new Point(210, 107);
            createKlasseBtn.Name = "createKlasseBtn";
            createKlasseBtn.Size = new Size(90, 30);
            createKlasseBtn.TabIndex = 4;
            createKlasseBtn.Text = "Speichern";
            createKlasseBtn.UseVisualStyleBackColor = true;
            createKlasseBtn.Click += createKlasseBtn_Click;
            // 
            // newKlassenlehrer
            // 
            newKlassenlehrer.DropDownStyle = ComboBoxStyle.DropDownList;
            newKlassenlehrer.FormattingEnabled = true;
            newKlassenlehrer.Location = new Point(15, 104);
            newKlassenlehrer.Name = "newKlassenlehrer";
            newKlassenlehrer.Size = new Size(185, 23);
            newKlassenlehrer.TabIndex = 3;
            // 
            // newTeacherLabel
            // 
            newTeacherLabel.AutoSize = true;
            newTeacherLabel.Location = new Point(15, 85);
            newTeacherLabel.Name = "newTeacherLabel";
            newTeacherLabel.Size = new Size(76, 15);
            newTeacherLabel.TabIndex = 2;
            newTeacherLabel.Text = "Klassenlehrer";
            // 
            // newKlasseName
            // 
            newKlasseName.Location = new Point(15, 50);
            newKlasseName.Name = "newKlasseName";
            newKlasseName.Size = new Size(285, 23);
            newKlasseName.TabIndex = 1;
            // 
            // newNameLabel
            // 
            newNameLabel.AutoSize = true;
            newNameLabel.Location = new Point(15, 31);
            newNameLabel.Name = "newNameLabel";
            newNameLabel.Size = new Size(75, 15);
            newNameLabel.TabIndex = 0;
            newNameLabel.Text = "Bezeichnung";
            // 
            // editGroup
            // 
            editGroup.Controls.Add(updateKlasseBtn);
            editGroup.Controls.Add(editKlassenlehrer);
            editGroup.Controls.Add(editTeacherLabel);
            editGroup.Controls.Add(editKlasseName);
            editGroup.Controls.Add(editNameLabel);
            editGroup.Location = new Point(25, 230);
            editGroup.Name = "editGroup";
            editGroup.Size = new Size(315, 150);
            editGroup.TabIndex = 3;
            editGroup.TabStop = false;
            editGroup.Text = "Ausgewählte Klasse bearbeiten";
            // 
            // updateKlasseBtn
            // 
            updateKlasseBtn.Location = new Point(210, 107);
            updateKlasseBtn.Name = "updateKlasseBtn";
            updateKlasseBtn.Size = new Size(90, 30);
            updateKlasseBtn.TabIndex = 4;
            updateKlasseBtn.Text = "Ändern";
            updateKlasseBtn.UseVisualStyleBackColor = true;
            updateKlasseBtn.Click += updateKlasseBtn_Click;
            // 
            // editKlassenlehrer
            // 
            editKlassenlehrer.DropDownStyle = ComboBoxStyle.DropDownList;
            editKlassenlehrer.FormattingEnabled = true;
            editKlassenlehrer.Location = new Point(15, 104);
            editKlassenlehrer.Name = "editKlassenlehrer";
            editKlassenlehrer.Size = new Size(185, 23);
            editKlassenlehrer.TabIndex = 3;
            // 
            // editTeacherLabel
            // 
            editTeacherLabel.AutoSize = true;
            editTeacherLabel.Location = new Point(15, 85);
            editTeacherLabel.Name = "editTeacherLabel";
            editTeacherLabel.Size = new Size(76, 15);
            editTeacherLabel.TabIndex = 2;
            editTeacherLabel.Text = "Klassenlehrer";
            // 
            // editKlasseName
            // 
            editKlasseName.Location = new Point(15, 50);
            editKlasseName.Name = "editKlasseName";
            editKlasseName.Size = new Size(285, 23);
            editKlasseName.TabIndex = 1;
            // 
            // editNameLabel
            // 
            editNameLabel.AutoSize = true;
            editNameLabel.Location = new Point(15, 31);
            editNameLabel.Name = "editNameLabel";
            editNameLabel.Size = new Size(75, 15);
            editNameLabel.TabIndex = 0;
            editNameLabel.Text = "Bezeichnung";
            // 
            // deleteGroup
            // 
            deleteGroup.Controls.Add(deleteKlasseBtn);
            deleteGroup.Controls.Add(deleteKlasseName);
            deleteGroup.Controls.Add(deleteLabel);
            deleteGroup.Location = new Point(25, 390);
            deleteGroup.Name = "deleteGroup";
            deleteGroup.Size = new Size(315, 100);
            deleteGroup.TabIndex = 4;
            deleteGroup.TabStop = false;
            deleteGroup.Text = "Ausgewählte Klasse löschen";
            // 
            // deleteKlasseBtn
            // 
            deleteKlasseBtn.Location = new Point(210, 51);
            deleteKlasseBtn.Name = "deleteKlasseBtn";
            deleteKlasseBtn.Size = new Size(90, 30);
            deleteKlasseBtn.TabIndex = 2;
            deleteKlasseBtn.Text = "Löschen";
            deleteKlasseBtn.UseVisualStyleBackColor = true;
            deleteKlasseBtn.Click += deleteKlasseBtn_Click;
            // 
            // deleteKlasseName
            // 
            deleteKlasseName.Location = new Point(15, 55);
            deleteKlasseName.Name = "deleteKlasseName";
            deleteKlasseName.ReadOnly = true;
            deleteKlasseName.Size = new Size(185, 23);
            deleteKlasseName.TabIndex = 1;
            // 
            // deleteLabel
            // 
            deleteLabel.AutoSize = true;
            deleteLabel.Location = new Point(15, 32);
            deleteLabel.Name = "deleteLabel";
            deleteLabel.Size = new Size(39, 15);
            deleteLabel.TabIndex = 0;
            deleteLabel.Text = "Klasse";
            // 
            // reloadBtn
            // 
            reloadBtn.Location = new Point(25, 505);
            reloadBtn.Name = "reloadBtn";
            reloadBtn.Size = new Size(315, 32);
            reloadBtn.TabIndex = 5;
            reloadBtn.Text = "Klassen neu laden";
            reloadBtn.UseVisualStyleBackColor = true;
            reloadBtn.Click += reloadBtn_Click;
            // 
            // klassenGrid
            // 
            klassenGrid.AllowUserToAddRows = false;
            klassenGrid.AllowUserToDeleteRows = false;
            klassenGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            klassenGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            klassenGrid.Location = new Point(365, 70);
            klassenGrid.MultiSelect = false;
            klassenGrid.Name = "klassenGrid";
            klassenGrid.ReadOnly = true;
            klassenGrid.RowHeadersVisible = false;
            klassenGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            klassenGrid.Size = new Size(607, 270);
            klassenGrid.TabIndex = 6;
            klassenGrid.SelectionChanged += klassenGrid_SelectionChanged;
            // 
            // studentLabel
            // 
            studentLabel.AutoSize = true;
            studentLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            studentLabel.Location = new Point(365, 356);
            studentLabel.Name = "studentLabel";
            studentLabel.Size = new Size(225, 19);
            studentLabel.TabIndex = 7;
            studentLabel.Text = "Schüler der ausgewählten Klasse";
            // 
            // klassenSchuelerGrid
            // 
            klassenSchuelerGrid.AllowUserToAddRows = false;
            klassenSchuelerGrid.AllowUserToDeleteRows = false;
            klassenSchuelerGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            klassenSchuelerGrid.Location = new Point(365, 382);
            klassenSchuelerGrid.Name = "klassenSchuelerGrid";
            klassenSchuelerGrid.ReadOnly = true;
            klassenSchuelerGrid.RowHeadersVisible = false;
            klassenSchuelerGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            klassenSchuelerGrid.Size = new Size(607, 155);
            klassenSchuelerGrid.TabIndex = 8;
            // 
            // Klassen
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(984, 561);
            Controls.Add(klassenSchuelerGrid);
            Controls.Add(studentLabel);
            Controls.Add(klassenGrid);
            Controls.Add(reloadBtn);
            Controls.Add(deleteGroup);
            Controls.Add(editGroup);
            Controls.Add(newGroup);
            Controls.Add(titleLabel);
            Controls.Add(back);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Klassen";
            Text = "SchulApp - Klassen";
            newGroup.ResumeLayout(false);
            newGroup.PerformLayout();
            editGroup.ResumeLayout(false);
            editGroup.PerformLayout();
            deleteGroup.ResumeLayout(false);
            deleteGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)klassenGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)klassenSchuelerGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Button back;
        private Label titleLabel;
        private GroupBox newGroup;
        private Button createKlasseBtn;
        private ComboBox newKlassenlehrer;
        private Label newTeacherLabel;
        private TextBox newKlasseName;
        private Label newNameLabel;
        private GroupBox editGroup;
        private Button updateKlasseBtn;
        private ComboBox editKlassenlehrer;
        private Label editTeacherLabel;
        private TextBox editKlasseName;
        private Label editNameLabel;
        private GroupBox deleteGroup;
        private Button deleteKlasseBtn;
        private TextBox deleteKlasseName;
        private Label deleteLabel;
        private Button reloadBtn;
        private DataGridView klassenGrid;
        private Label studentLabel;
        private DataGridView klassenSchuelerGrid;
    }
}
