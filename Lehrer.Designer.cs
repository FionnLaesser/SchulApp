namespace SchulApp
{
    partial class Lehrer
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
            back = new Button();
            label1 = new Label();
            label2 = new Label();
            showLehrer = new Label();
            OKnewTeacherBtn = new Button();
            newTeacherTextBox = new TextBox();
            oldName = new TextBox();
            newName = new TextBox();
            OKchangeNameBtn = new Button();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            OKdeleteBtn = new Button();
            nameToDelete = new TextBox();
            label6 = new Label();
            nameTeacherForInfo = new TextBox();
            info = new TextBox();
            infoTitel = new TextBox();
            label8 = new Label();
            label7 = new Label();
            label9 = new Label();
            OKchangingInfo = new Button();
            SuspendLayout();
            // 
            // back
            // 
            back.Location = new Point(897, 12);
            back.Name = "back";
            back.Size = new Size(75, 23);
            back.TabIndex = 16;
            back.Text = "zurück";
            back.UseVisualStyleBackColor = true;
            back.Click += back_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(22, 55);
            label1.Name = "label1";
            label1.Size = new Size(125, 15);
            label1.TabIndex = 17;
            label1.Text = "Neuen Lehrer erstellen";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(22, 122);
            label2.Name = "label2";
            label2.Size = new Size(198, 15);
            label2.TabIndex = 18;
            label2.Text = "Hier kannst du die Lehrer Bearbeiten";
            // 
            // showLehrer
            // 
            showLehrer.AutoSize = true;
            showLehrer.Location = new Point(471, 37);
            showLehrer.Name = "showLehrer";
            showLehrer.Size = new Size(38, 15);
            showLehrer.TabIndex = 19;
            showLehrer.Text = "label3";
            // 
            // OKnewTeacherBtn
            // 
            OKnewTeacherBtn.Location = new Point(145, 74);
            OKnewTeacherBtn.Name = "OKnewTeacherBtn";
            OKnewTeacherBtn.Size = new Size(75, 23);
            OKnewTeacherBtn.TabIndex = 20;
            OKnewTeacherBtn.Text = "Bestätigen";
            OKnewTeacherBtn.UseVisualStyleBackColor = true;
            // 
            // newTeacherTextBox
            // 
            newTeacherTextBox.Location = new Point(22, 73);
            newTeacherTextBox.Name = "newTeacherTextBox";
            newTeacherTextBox.Size = new Size(100, 23);
            newTeacherTextBox.TabIndex = 21;
            // 
            // oldName
            // 
            oldName.Location = new Point(23, 155);
            oldName.Name = "oldName";
            oldName.Size = new Size(100, 23);
            oldName.TabIndex = 22;
            // 
            // newName
            // 
            newName.Location = new Point(146, 155);
            newName.Name = "newName";
            newName.Size = new Size(100, 23);
            newName.TabIndex = 23;
            // 
            // OKchangeNameBtn
            // 
            OKchangeNameBtn.Location = new Point(266, 154);
            OKchangeNameBtn.Name = "OKchangeNameBtn";
            OKchangeNameBtn.Size = new Size(75, 23);
            OKchangeNameBtn.TabIndex = 24;
            OKchangeNameBtn.Text = "Bestätigen";
            OKchangeNameBtn.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(23, 137);
            label3.Name = "label3";
            label3.Size = new Size(67, 15);
            label3.TabIndex = 25;
            label3.Text = "Alter Name";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(146, 137);
            label4.Name = "label4";
            label4.Size = new Size(74, 15);
            label4.TabIndex = 26;
            label4.Text = "Neuer Name";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(23, 201);
            label5.Name = "label5";
            label5.Size = new Size(167, 15);
            label5.TabIndex = 27;
            label5.Text = "Name zum Löschen eingeben:";
            // 
            // OKdeleteBtn
            // 
            OKdeleteBtn.Location = new Point(219, 218);
            OKdeleteBtn.Name = "OKdeleteBtn";
            OKdeleteBtn.Size = new Size(75, 23);
            OKdeleteBtn.TabIndex = 28;
            OKdeleteBtn.Text = "Bestätigen";
            OKdeleteBtn.UseVisualStyleBackColor = true;
            // 
            // nameToDelete
            // 
            nameToDelete.Location = new Point(23, 218);
            nameToDelete.Name = "nameToDelete";
            nameToDelete.Size = new Size(167, 23);
            nameToDelete.TabIndex = 29;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(22, 256);
            label6.Name = "label6";
            label6.Size = new Size(148, 30);
            label6.TabIndex = 30;
            label6.Text = "Hier kannst du eine Info zu\r\n einem Lehrer hinzufügen";
            // 
            // nameTeacherForInfo
            // 
            nameTeacherForInfo.Location = new Point(38, 315);
            nameTeacherForInfo.Name = "nameTeacherForInfo";
            nameTeacherForInfo.Size = new Size(100, 23);
            nameTeacherForInfo.TabIndex = 31;
            // 
            // info
            // 
            info.Location = new Point(176, 332);
            info.Name = "info";
            info.Size = new Size(161, 23);
            info.TabIndex = 32;
            // 
            // infoTitel
            // 
            infoTitel.Location = new Point(176, 288);
            infoTitel.Name = "infoTitel";
            infoTitel.Size = new Size(161, 23);
            infoTitel.TabIndex = 33;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(38, 297);
            label8.Name = "label8";
            label8.Size = new Size(101, 15);
            label8.TabIndex = 35;
            label8.Text = "Name des Lehrers";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(176, 270);
            label7.Name = "label7";
            label7.Size = new Size(164, 15);
            label7.TabIndex = 36;
            label7.Text = "Titel der Info (z.B: GreenFlags)";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(176, 314);
            label9.Name = "label9";
            label9.Size = new Size(161, 15);
            label9.TabIndex = 37;
            label9.Text = "Die Eigentliche Info (z.B Nett)";
            // 
            // OKchangingInfo
            // 
            OKchangingInfo.Location = new Point(358, 306);
            OKchangingInfo.Name = "OKchangingInfo";
            OKchangingInfo.Size = new Size(75, 23);
            OKchangingInfo.TabIndex = 38;
            OKchangingInfo.Text = "Bestätigen";
            OKchangingInfo.UseVisualStyleBackColor = true;
            // 
            // Lehrer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(984, 561);
            Controls.Add(OKchangingInfo);
            Controls.Add(label9);
            Controls.Add(label7);
            Controls.Add(label8);
            Controls.Add(infoTitel);
            Controls.Add(info);
            Controls.Add(nameTeacherForInfo);
            Controls.Add(label6);
            Controls.Add(nameToDelete);
            Controls.Add(OKdeleteBtn);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(OKchangeNameBtn);
            Controls.Add(newName);
            Controls.Add(oldName);
            Controls.Add(newTeacherTextBox);
            Controls.Add(OKnewTeacherBtn);
            Controls.Add(showLehrer);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(back);
            Name = "Lehrer";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button back;
        private Label label1;
        private Label label2;
        private Label showLehrer;
        private Button OKnewTeacherBtn;
        private TextBox newTeacherTextBox;
        private TextBox oldName;
        private TextBox newName;
        private Button OKchangeNameBtn;
        private Label label3;
        private Label label4;
        private Label label5;
        private Button OKdeleteBtn;
        private TextBox nameToDelete;
        private Label label6;
        private TextBox nameTeacherForInfo;
        private TextBox info;
        private TextBox infoTitel;
        private Label label8;
        private Label label7;
        private Label label9;
        private Button OKchangingInfo;
    }
}