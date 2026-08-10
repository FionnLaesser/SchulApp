namespace SchulApp
{
    partial class Stundenplan
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
            createEintragBtn = new Button();
            newRaum = new TextBox();
            newRaumLabel = new Label();
            newEndzeit = new DateTimePicker();
            newEndLabel = new Label();
            newStartzeit = new DateTimePicker();
            newStartLabel = new Label();
            newWochentag = new ComboBox();
            newTagLabel = new Label();
            newKurs = new ComboBox();
            newKursLabel = new Label();
            editGroup = new GroupBox();
            updateEintragBtn = new Button();
            editRaum = new TextBox();
            editRaumLabel = new Label();
            editEndzeit = new DateTimePicker();
            editEndLabel = new Label();
            editStartzeit = new DateTimePicker();
            editStartLabel = new Label();
            editWochentag = new ComboBox();
            editTagLabel = new Label();
            editKurs = new ComboBox();
            editKursLabel = new Label();
            deleteGroup = new GroupBox();
            deleteEintragBtn = new Button();
            deleteInfo = new TextBox();
            reloadBtn = new Button();
            stundenplanGrid = new DataGridView();
            datum = new Label();
            uhrzeit = new Label();
            newGroup.SuspendLayout();
            editGroup.SuspendLayout();
            deleteGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)stundenplanGrid).BeginInit();
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
            titleLabel.Size = new Size(278, 32);
            titleLabel.TabIndex = 1;
            titleLabel.Text = "Stundenplan verwalten";
            // 
            // newGroup
            // 
            newGroup.Controls.Add(createEintragBtn);
            newGroup.Controls.Add(newRaum);
            newGroup.Controls.Add(newRaumLabel);
            newGroup.Controls.Add(newEndzeit);
            newGroup.Controls.Add(newEndLabel);
            newGroup.Controls.Add(newStartzeit);
            newGroup.Controls.Add(newStartLabel);
            newGroup.Controls.Add(newWochentag);
            newGroup.Controls.Add(newTagLabel);
            newGroup.Controls.Add(newKurs);
            newGroup.Controls.Add(newKursLabel);
            newGroup.Location = new Point(25, 70);
            newGroup.Name = "newGroup";
            newGroup.Size = new Size(330, 210);
            newGroup.TabIndex = 2;
            newGroup.TabStop = false;
            newGroup.Text = "Neuer Stundenplaneintrag";
            // 
            // createEintragBtn
            // 
            createEintragBtn.Location = new Point(225, 163);
            createEintragBtn.Name = "createEintragBtn";
            createEintragBtn.Size = new Size(90, 31);
            createEintragBtn.TabIndex = 10;
            createEintragBtn.Text = "Speichern";
            createEintragBtn.UseVisualStyleBackColor = true;
            createEintragBtn.Click += createEintragBtn_Click;
            // 
            // newRaum
            // 
            newRaum.Location = new Point(15, 166);
            newRaum.Name = "newRaum";
            newRaum.Size = new Size(195, 23);
            newRaum.TabIndex = 9;
            // 
            // newRaumLabel
            // 
            newRaumLabel.AutoSize = true;
            newRaumLabel.Location = new Point(15, 147);
            newRaumLabel.Name = "newRaumLabel";
            newRaumLabel.Size = new Size(38, 15);
            newRaumLabel.TabIndex = 8;
            newRaumLabel.Text = "Raum";
            // 
            // newEndzeit
            // 
            newEndzeit.CustomFormat = "HH:mm";
            newEndzeit.Format = DateTimePickerFormat.Custom;
            newEndzeit.Location = new Point(170, 113);
            newEndzeit.Name = "newEndzeit";
            newEndzeit.ShowUpDown = true;
            newEndzeit.Size = new Size(145, 23);
            newEndzeit.TabIndex = 7;
            newEndzeit.Value = new DateTime(2026, 1, 1, 8, 45, 0, 0);
            // 
            // newEndLabel
            // 
            newEndLabel.AutoSize = true;
            newEndLabel.Location = new Point(170, 94);
            newEndLabel.Name = "newEndLabel";
            newEndLabel.Size = new Size(45, 15);
            newEndLabel.TabIndex = 6;
            newEndLabel.Text = "Endzeit";
            // 
            // newStartzeit
            // 
            newStartzeit.CustomFormat = "HH:mm";
            newStartzeit.Format = DateTimePickerFormat.Custom;
            newStartzeit.Location = new Point(15, 113);
            newStartzeit.Name = "newStartzeit";
            newStartzeit.ShowUpDown = true;
            newStartzeit.Size = new Size(145, 23);
            newStartzeit.TabIndex = 5;
            newStartzeit.Value = new DateTime(2026, 1, 1, 8, 0, 0, 0);
            // 
            // newStartLabel
            // 
            newStartLabel.AutoSize = true;
            newStartLabel.Location = new Point(15, 94);
            newStartLabel.Name = "newStartLabel";
            newStartLabel.Size = new Size(49, 15);
            newStartLabel.TabIndex = 4;
            newStartLabel.Text = "Startzeit";
            // 
            // newWochentag
            // 
            newWochentag.DropDownStyle = ComboBoxStyle.DropDownList;
            newWochentag.FormattingEnabled = true;
            newWochentag.Location = new Point(170, 59);
            newWochentag.Name = "newWochentag";
            newWochentag.Size = new Size(145, 23);
            newWochentag.TabIndex = 3;
            // 
            // newTagLabel
            // 
            newTagLabel.AutoSize = true;
            newTagLabel.Location = new Point(170, 40);
            newTagLabel.Name = "newTagLabel";
            newTagLabel.Size = new Size(68, 15);
            newTagLabel.TabIndex = 2;
            newTagLabel.Text = "Wochentag";
            // 
            // newKurs
            // 
            newKurs.DropDownStyle = ComboBoxStyle.DropDownList;
            newKurs.FormattingEnabled = true;
            newKurs.Location = new Point(15, 59);
            newKurs.Name = "newKurs";
            newKurs.Size = new Size(145, 23);
            newKurs.TabIndex = 1;
            // 
            // newKursLabel
            // 
            newKursLabel.AutoSize = true;
            newKursLabel.Location = new Point(15, 40);
            newKursLabel.Name = "newKursLabel";
            newKursLabel.Size = new Size(30, 15);
            newKursLabel.TabIndex = 0;
            newKursLabel.Text = "Kurs";
            // 
            // editGroup
            // 
            editGroup.Controls.Add(updateEintragBtn);
            editGroup.Controls.Add(editRaum);
            editGroup.Controls.Add(editRaumLabel);
            editGroup.Controls.Add(editEndzeit);
            editGroup.Controls.Add(editEndLabel);
            editGroup.Controls.Add(editStartzeit);
            editGroup.Controls.Add(editStartLabel);
            editGroup.Controls.Add(editWochentag);
            editGroup.Controls.Add(editTagLabel);
            editGroup.Controls.Add(editKurs);
            editGroup.Controls.Add(editKursLabel);
            editGroup.Location = new Point(25, 290);
            editGroup.Name = "editGroup";
            editGroup.Size = new Size(330, 210);
            editGroup.TabIndex = 3;
            editGroup.TabStop = false;
            editGroup.Text = "Ausgewählten Eintrag bearbeiten";
            // 
            // updateEintragBtn
            // 
            updateEintragBtn.Location = new Point(225, 163);
            updateEintragBtn.Name = "updateEintragBtn";
            updateEintragBtn.Size = new Size(90, 31);
            updateEintragBtn.TabIndex = 10;
            updateEintragBtn.Text = "Ändern";
            updateEintragBtn.UseVisualStyleBackColor = true;
            updateEintragBtn.Click += updateEintragBtn_Click;
            // 
            // editRaum
            // 
            editRaum.Location = new Point(15, 166);
            editRaum.Name = "editRaum";
            editRaum.Size = new Size(195, 23);
            editRaum.TabIndex = 9;
            // 
            // editRaumLabel
            // 
            editRaumLabel.AutoSize = true;
            editRaumLabel.Location = new Point(15, 147);
            editRaumLabel.Name = "editRaumLabel";
            editRaumLabel.Size = new Size(38, 15);
            editRaumLabel.TabIndex = 8;
            editRaumLabel.Text = "Raum";
            // 
            // editEndzeit
            // 
            editEndzeit.CustomFormat = "HH:mm";
            editEndzeit.Format = DateTimePickerFormat.Custom;
            editEndzeit.Location = new Point(170, 113);
            editEndzeit.Name = "editEndzeit";
            editEndzeit.ShowUpDown = true;
            editEndzeit.Size = new Size(145, 23);
            editEndzeit.TabIndex = 7;
            editEndzeit.Value = new DateTime(2026, 1, 1, 8, 45, 0, 0);
            // 
            // editEndLabel
            // 
            editEndLabel.AutoSize = true;
            editEndLabel.Location = new Point(170, 94);
            editEndLabel.Name = "editEndLabel";
            editEndLabel.Size = new Size(45, 15);
            editEndLabel.TabIndex = 6;
            editEndLabel.Text = "Endzeit";
            // 
            // editStartzeit
            // 
            editStartzeit.CustomFormat = "HH:mm";
            editStartzeit.Format = DateTimePickerFormat.Custom;
            editStartzeit.Location = new Point(15, 113);
            editStartzeit.Name = "editStartzeit";
            editStartzeit.ShowUpDown = true;
            editStartzeit.Size = new Size(145, 23);
            editStartzeit.TabIndex = 5;
            editStartzeit.Value = new DateTime(2026, 1, 1, 8, 0, 0, 0);
            // 
            // editStartLabel
            // 
            editStartLabel.AutoSize = true;
            editStartLabel.Location = new Point(15, 94);
            editStartLabel.Name = "editStartLabel";
            editStartLabel.Size = new Size(49, 15);
            editStartLabel.TabIndex = 4;
            editStartLabel.Text = "Startzeit";
            // 
            // editWochentag
            // 
            editWochentag.DropDownStyle = ComboBoxStyle.DropDownList;
            editWochentag.FormattingEnabled = true;
            editWochentag.Location = new Point(170, 59);
            editWochentag.Name = "editWochentag";
            editWochentag.Size = new Size(145, 23);
            editWochentag.TabIndex = 3;
            // 
            // editTagLabel
            // 
            editTagLabel.AutoSize = true;
            editTagLabel.Location = new Point(170, 40);
            editTagLabel.Name = "editTagLabel";
            editTagLabel.Size = new Size(68, 15);
            editTagLabel.TabIndex = 2;
            editTagLabel.Text = "Wochentag";
            // 
            // editKurs
            // 
            editKurs.DropDownStyle = ComboBoxStyle.DropDownList;
            editKurs.FormattingEnabled = true;
            editKurs.Location = new Point(15, 59);
            editKurs.Name = "editKurs";
            editKurs.Size = new Size(145, 23);
            editKurs.TabIndex = 1;
            // 
            // editKursLabel
            // 
            editKursLabel.AutoSize = true;
            editKursLabel.Location = new Point(15, 40);
            editKursLabel.Name = "editKursLabel";
            editKursLabel.Size = new Size(30, 15);
            editKursLabel.TabIndex = 0;
            editKursLabel.Text = "Kurs";
            // 
            // deleteGroup
            // 
            deleteGroup.Controls.Add(deleteEintragBtn);
            deleteGroup.Controls.Add(deleteInfo);
            deleteGroup.Location = new Point(25, 510);
            deleteGroup.Name = "deleteGroup";
            deleteGroup.Size = new Size(330, 42);
            deleteGroup.TabIndex = 4;
            deleteGroup.TabStop = false;
            deleteGroup.Text = "Löschen";
            // 
            // deleteEintragBtn
            // 
            deleteEintragBtn.Location = new Point(225, 11);
            deleteEintragBtn.Name = "deleteEintragBtn";
            deleteEintragBtn.Size = new Size(90, 27);
            deleteEintragBtn.TabIndex = 1;
            deleteEintragBtn.Text = "Löschen";
            deleteEintragBtn.UseVisualStyleBackColor = true;
            deleteEintragBtn.Click += deleteEintragBtn_Click;
            // 
            // deleteInfo
            // 
            deleteInfo.Location = new Point(15, 13);
            deleteInfo.Name = "deleteInfo";
            deleteInfo.ReadOnly = true;
            deleteInfo.Size = new Size(195, 23);
            deleteInfo.TabIndex = 0;
            // 
            // reloadBtn
            // 
            reloadBtn.Location = new Point(375, 505);
            reloadBtn.Name = "reloadBtn";
            reloadBtn.Size = new Size(597, 32);
            reloadBtn.TabIndex = 5;
            reloadBtn.Text = "Stundenplan und Kurse neu laden";
            reloadBtn.UseVisualStyleBackColor = true;
            reloadBtn.Click += reloadBtn_Click;
            // 
            // stundenplanGrid
            // 
            stundenplanGrid.AllowUserToAddRows = false;
            stundenplanGrid.AllowUserToDeleteRows = false;
            stundenplanGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            stundenplanGrid.Location = new Point(375, 70);
            stundenplanGrid.MultiSelect = false;
            stundenplanGrid.Name = "stundenplanGrid";
            stundenplanGrid.ReadOnly = true;
            stundenplanGrid.RowHeadersVisible = false;
            stundenplanGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            stundenplanGrid.Size = new Size(597, 420);
            stundenplanGrid.TabIndex = 6;
            stundenplanGrid.SelectionChanged += stundenplanGrid_SelectionChanged;
            // 
            // datum
            // 
            datum.AutoSize = true;
            datum.Location = new Point(419, 9);
            datum.Name = "datum";
            datum.Size = new Size(38, 15);
            datum.TabIndex = 7;
            datum.Text = "label1";
            // 
            // uhrzeit
            // 
            uhrzeit.AutoSize = true;
            uhrzeit.Location = new Point(419, 32);
            uhrzeit.Name = "uhrzeit";
            uhrzeit.Size = new Size(38, 15);
            uhrzeit.TabIndex = 8;
            uhrzeit.Text = "label1";
            // 
            // Stundenplan
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(984, 561);
            Controls.Add(uhrzeit);
            Controls.Add(datum);
            Controls.Add(stundenplanGrid);
            Controls.Add(reloadBtn);
            Controls.Add(deleteGroup);
            Controls.Add(editGroup);
            Controls.Add(newGroup);
            Controls.Add(titleLabel);
            Controls.Add(back);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Stundenplan";
            Text = "SchulApp - Stundenplan";
            newGroup.ResumeLayout(false);
            newGroup.PerformLayout();
            editGroup.ResumeLayout(false);
            editGroup.PerformLayout();
            deleteGroup.ResumeLayout(false);
            deleteGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)stundenplanGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Button back;
        private Label titleLabel;
        private GroupBox newGroup;
        private Button createEintragBtn;
        private TextBox newRaum;
        private Label newRaumLabel;
        private DateTimePicker newEndzeit;
        private Label newEndLabel;
        private DateTimePicker newStartzeit;
        private Label newStartLabel;
        private ComboBox newWochentag;
        private Label newTagLabel;
        private ComboBox newKurs;
        private Label newKursLabel;
        private GroupBox editGroup;
        private Button updateEintragBtn;
        private TextBox editRaum;
        private Label editRaumLabel;
        private DateTimePicker editEndzeit;
        private Label editEndLabel;
        private DateTimePicker editStartzeit;
        private Label editStartLabel;
        private ComboBox editWochentag;
        private Label editTagLabel;
        private ComboBox editKurs;
        private Label editKursLabel;
        private GroupBox deleteGroup;
        private Button deleteEintragBtn;
        private TextBox deleteInfo;
        private Button reloadBtn;
        private DataGridView stundenplanGrid;
        private Label datum;
        private Label uhrzeit;
    }
}
