namespace SchulApp
{
    partial class Schueler
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
            label1 = new Label();
            newStudentName = new TextBox();
            OKnewStudent = new Button();
            showStudents = new Button();
            OKchangeName = new Button();
            oldName = new TextBox();
            newName = new TextBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label6 = new Label();
            deleteNameBox = new TextBox();
            label7 = new Label();
            OKdeleteBtn = new Button();
            back = new Button();
            newStudentKlasse = new ComboBox();
            labelKlasseNeu = new Label();
            editKlasse = new ComboBox();
            labelKlasseBearbeiten = new Label();
            studentGrid = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)studentGrid).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(147, 15);
            label1.TabIndex = 0;
            label1.Text = "Neuen Schüler hinzufügen";
            label1.Click += label1_Click;
            // 
            // newStudentName
            // 
            newStudentName.Location = new Point(12, 45);
            newStudentName.Name = "newStudentName";
            newStudentName.PlaceholderText = "Name";
            newStudentName.Size = new Size(200, 23);
            newStudentName.TabIndex = 1;
            newStudentName.TextChanged += newStudentName_TextChanged;
            // 
            // newStudentKlasse
            // 
            newStudentKlasse.DropDownStyle = ComboBoxStyle.DropDownList;
            newStudentKlasse.FormattingEnabled = true;
            newStudentKlasse.Location = new Point(12, 94);
            newStudentKlasse.Name = "newStudentKlasse";
            newStudentKlasse.Size = new Size(200, 23);
            newStudentKlasse.TabIndex = 2;
            // 
            // labelKlasseNeu
            // 
            labelKlasseNeu.AutoSize = true;
            labelKlasseNeu.Location = new Point(12, 76);
            labelKlasseNeu.Name = "labelKlasseNeu";
            labelKlasseNeu.Size = new Size(40, 15);
            labelKlasseNeu.TabIndex = 3;
            labelKlasseNeu.Text = "Klasse";
            // 
            // OKnewStudent
            // 
            OKnewStudent.Location = new Point(218, 45);
            OKnewStudent.Name = "OKnewStudent";
            OKnewStudent.Size = new Size(100, 72);
            OKnewStudent.TabIndex = 4;
            OKnewStudent.Text = "Speichern";
            OKnewStudent.UseVisualStyleBackColor = true;
            OKnewStudent.Click += OKnewStudent_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 151);
            label4.Name = "label4";
            label4.Size = new Size(157, 15);
            label4.TabIndex = 5;
            label4.Text = "Ausgewählten Schüler ändern";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 179);
            label2.Name = "label2";
            label2.Size = new Size(91, 15);
            label2.TabIndex = 6;
            label2.Text = "Aktueller Name";
            // 
            // oldName
            // 
            oldName.Location = new Point(12, 197);
            oldName.Name = "oldName";
            oldName.ReadOnly = true;
            oldName.Size = new Size(200, 23);
            oldName.TabIndex = 7;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 228);
            label3.Name = "label3";
            label3.Size = new Size(74, 15);
            label3.TabIndex = 8;
            label3.Text = "Neuer Name";
            // 
            // newName
            // 
            newName.Location = new Point(12, 246);
            newName.Name = "newName";
            newName.Size = new Size(200, 23);
            newName.TabIndex = 9;
            // 
            // labelKlasseBearbeiten
            // 
            labelKlasseBearbeiten.AutoSize = true;
            labelKlasseBearbeiten.Location = new Point(12, 277);
            labelKlasseBearbeiten.Name = "labelKlasseBearbeiten";
            labelKlasseBearbeiten.Size = new Size(71, 15);
            labelKlasseBearbeiten.TabIndex = 10;
            labelKlasseBearbeiten.Text = "Neue Klasse";
            // 
            // editKlasse
            // 
            editKlasse.DropDownStyle = ComboBoxStyle.DropDownList;
            editKlasse.FormattingEnabled = true;
            editKlasse.Location = new Point(12, 295);
            editKlasse.Name = "editKlasse";
            editKlasse.Size = new Size(200, 23);
            editKlasse.TabIndex = 11;
            // 
            // OKchangeName
            // 
            OKchangeName.Location = new Point(218, 246);
            OKchangeName.Name = "OKchangeName";
            OKchangeName.Size = new Size(100, 72);
            OKchangeName.TabIndex = 12;
            OKchangeName.Text = "Ändern";
            OKchangeName.UseVisualStyleBackColor = true;
            OKchangeName.Click += OKchangeName_Click;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 352);
            label6.Name = "label6";
            label6.Size = new Size(185, 15);
            label6.TabIndex = 13;
            label6.Text = "Ausgewählten Schüler löschen";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(12, 377);
            label7.Name = "label7";
            label7.Size = new Size(39, 15);
            label7.TabIndex = 14;
            label7.Text = "Name";
            // 
            // deleteNameBox
            // 
            deleteNameBox.Location = new Point(12, 395);
            deleteNameBox.Name = "deleteNameBox";
            deleteNameBox.ReadOnly = true;
            deleteNameBox.Size = new Size(200, 23);
            deleteNameBox.TabIndex = 15;
            // 
            // OKdeleteBtn
            // 
            OKdeleteBtn.Location = new Point(218, 395);
            OKdeleteBtn.Name = "OKdeleteBtn";
            OKdeleteBtn.Size = new Size(100, 23);
            OKdeleteBtn.TabIndex = 16;
            OKdeleteBtn.Text = "Löschen";
            OKdeleteBtn.UseVisualStyleBackColor = true;
            OKdeleteBtn.Click += OKdeleteBtn_Click;
            // 
            // showStudents
            // 
            showStudents.Location = new Point(12, 444);
            showStudents.Name = "showStudents";
            showStudents.Size = new Size(306, 29);
            showStudents.TabIndex = 17;
            showStudents.Text = "Schüler neu laden";
            showStudents.UseVisualStyleBackColor = true;
            showStudents.Click += showStudents_Click;
            // 
            // studentGrid
            // 
            studentGrid.AllowUserToAddRows = false;
            studentGrid.AllowUserToDeleteRows = false;
            studentGrid.AllowUserToResizeRows = false;
            studentGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            studentGrid.BackgroundColor = SystemColors.Window;
            studentGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            studentGrid.Location = new Point(350, 45);
            studentGrid.MultiSelect = false;
            studentGrid.Name = "studentGrid";
            studentGrid.ReadOnly = true;
            studentGrid.RowHeadersVisible = false;
            studentGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            studentGrid.Size = new Size(610, 428);
            studentGrid.TabIndex = 18;
            studentGrid.SelectionChanged += studentGrid_SelectionChanged;
            // 
            // back
            // 
            back.Location = new Point(885, 12);
            back.Name = "back";
            back.Size = new Size(75, 23);
            back.TabIndex = 19;
            back.Text = "zurück";
            back.UseVisualStyleBackColor = true;
            back.Click += back_Click;
            // 
            // Schueler
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(984, 561);
            Controls.Add(back);
            Controls.Add(studentGrid);
            Controls.Add(showStudents);
            Controls.Add(OKdeleteBtn);
            Controls.Add(deleteNameBox);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(OKchangeName);
            Controls.Add(editKlasse);
            Controls.Add(labelKlasseBearbeiten);
            Controls.Add(newName);
            Controls.Add(label3);
            Controls.Add(oldName);
            Controls.Add(label2);
            Controls.Add(label4);
            Controls.Add(OKnewStudent);
            Controls.Add(labelKlasseNeu);
            Controls.Add(newStudentKlasse);
            Controls.Add(newStudentName);
            Controls.Add(label1);
            Name = "Schueler";
            StartPosition = FormStartPosition.Manual;
            Text = "SchulApp - Schüler";
            ((System.ComponentModel.ISupportInitialize)studentGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label label1;
        private TextBox newStudentName;
        private Button OKnewStudent;
        private Button showStudents;
        private Button OKchangeName;
        private TextBox oldName;
        private TextBox newName;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label6;
        private TextBox deleteNameBox;
        private Label label7;
        private Button OKdeleteBtn;
        private Button back;
        private ComboBox newStudentKlasse;
        private Label labelKlasseNeu;
        private ComboBox editKlasse;
        private Label labelKlasseBearbeiten;
        private DataGridView studentGrid;
    }
}
