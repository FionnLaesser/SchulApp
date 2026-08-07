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
            studentListLabel = new Label();
            label6 = new Label();
            deleteNameBox = new TextBox();
            label7 = new Label();
            OKdeleteBtn = new Button();
            back = new Button();
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
            newStudentName.Location = new Point(12, 27);
            newStudentName.Name = "newStudentName";
            newStudentName.Size = new Size(147, 23);
            newStudentName.TabIndex = 1;
            newStudentName.Tag = "";
            newStudentName.TextChanged += newStudentName_TextChanged;
            // 
            // OKnewStudent
            // 
            OKnewStudent.Location = new Point(165, 23);
            OKnewStudent.Name = "OKnewStudent";
            OKnewStudent.Size = new Size(56, 29);
            OKnewStudent.TabIndex = 2;
            OKnewStudent.Text = "Enter";
            OKnewStudent.UseVisualStyleBackColor = true;
            OKnewStudent.Click += OKnewStudent_Click;
            // 
            // showStudents
            // 
            showStudents.Location = new Point(165, 58);
            showStudents.Name = "showStudents";
            showStudents.Size = new Size(172, 29);
            showStudents.TabIndex = 3;
            showStudents.Text = "Schüler Anzeigen";
            showStudents.UseVisualStyleBackColor = true;
            showStudents.Click += showStudents_Click;
            // 
            // OKchangeName
            // 
            OKchangeName.Location = new Point(262, 177);
            OKchangeName.Name = "OKchangeName";
            OKchangeName.Size = new Size(75, 23);
            OKchangeName.TabIndex = 4;
            OKchangeName.Text = "Ändern";
            OKchangeName.UseVisualStyleBackColor = true;
            OKchangeName.Click += OKchangeName_Click;
            // 
            // oldName
            // 
            oldName.Location = new Point(12, 178);
            oldName.Name = "oldName";
            oldName.Size = new Size(100, 23);
            oldName.TabIndex = 5;
            // 
            // newName
            // 
            newName.Location = new Point(142, 177);
            newName.Name = "newName";
            newName.Size = new Size(100, 23);
            newName.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 160);
            label2.Name = "label2";
            label2.Size = new Size(67, 15);
            label2.TabIndex = 7;
            label2.Text = "Alter Name";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(142, 160);
            label3.Name = "label3";
            label3.Size = new Size(74, 15);
            label3.TabIndex = 8;
            label3.Text = "Neuer Name";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 126);
            label4.Name = "label4";
            label4.Size = new Size(227, 15);
            label4.TabIndex = 9;
            label4.Text = "Hier kannst du die Schüler Namen ändern";
            // 
            // studentListLabel
            // 
            studentListLabel.AutoSize = true;
            studentListLabel.BackColor = SystemColors.ButtonShadow;
            studentListLabel.ForeColor = Color.Cornsilk;
            studentListLabel.Location = new Point(613, 95);
            studentListLabel.Name = "studentListLabel";
            studentListLabel.Size = new Size(38, 15);
            studentListLabel.TabIndex = 10;
            studentListLabel.Text = "label5";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(15, 215);
            label6.Name = "label6";
            label6.Size = new Size(205, 15);
            label6.TabIndex = 11;
            label6.Text = "Hier kannst du einen Schüler Löschen";
            // 
            // deleteNameBox
            // 
            deleteNameBox.Location = new Point(15, 259);
            deleteNameBox.Name = "deleteNameBox";
            deleteNameBox.Size = new Size(100, 23);
            deleteNameBox.TabIndex = 12;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(15, 241);
            label7.Name = "label7";
            label7.Size = new Size(39, 15);
            label7.TabIndex = 13;
            label7.Text = "Name";
            // 
            // OKdeleteBtn
            // 
            OKdeleteBtn.Location = new Point(121, 251);
            OKdeleteBtn.Name = "OKdeleteBtn";
            OKdeleteBtn.Size = new Size(62, 37);
            OKdeleteBtn.TabIndex = 14;
            OKdeleteBtn.Text = "Löschen";
            OKdeleteBtn.UseVisualStyleBackColor = true;
            OKdeleteBtn.Click += OKdeleteBtn_Click;
            // 
            // back
            // 
            back.Location = new Point(897, 12);
            back.Name = "back";
            back.Size = new Size(75, 23);
            back.TabIndex = 15;
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
            Controls.Add(OKdeleteBtn);
            Controls.Add(label7);
            Controls.Add(deleteNameBox);
            Controls.Add(label6);
            Controls.Add(studentListLabel);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(newName);
            Controls.Add(oldName);
            Controls.Add(OKchangeName);
            Controls.Add(showStudents);
            Controls.Add(OKnewStudent);
            Controls.Add(newStudentName);
            Controls.Add(label1);
            Name = "Schueler";
            Text = "SchulApp";
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
        private Label studentListLabel;
        private Label label6;
        private TextBox deleteNameBox;
        private Label label7;
        private Button OKdeleteBtn;
        private Button back;
    }
}