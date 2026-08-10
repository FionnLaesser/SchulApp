namespace SchulApp
{
    partial class Lehrer
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            back = new Button();
            labelNeu = new Label();
            newTeacherName = new TextBox();
            newTeacherEmail = new TextBox();
            newTeacherTelefon = new TextBox();
            labelNeuName = new Label();
            labelNeuEmail = new Label();
            labelNeuTelefon = new Label();
            OKnewTeacherBtn = new Button();
            labelBearbeiten = new Label();
            currentTeacherName = new TextBox();
            editTeacherName = new TextBox();
            editTeacherEmail = new TextBox();
            editTeacherTelefon = new TextBox();
            labelAktuell = new Label();
            labelEditName = new Label();
            labelEditEmail = new Label();
            labelEditTelefon = new Label();
            OKchangeTeacherBtn = new Button();
            labelLoeschen = new Label();
            deleteTeacherName = new TextBox();
            OKdeleteTeacherBtn = new Button();
            reloadTeacherBtn = new Button();
            lehrerGrid = new DataGridView();
            labelInfo = new Label();
            infoTitel = new TextBox();
            info = new TextBox();
            labelInfoTitel = new Label();
            labelInformation = new Label();
            OKchangingInfo = new Button();
            lehrerInfoGrid = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)lehrerGrid).BeginInit();
            ((System.ComponentModel.ISupportInitialize)lehrerInfoGrid).BeginInit();
            SuspendLayout();
            // 
            // back
            // 
            back.Location = new Point(885, 12);
            back.Name = "back";
            back.Size = new Size(75, 23);
            back.TabIndex = 0;
            back.Text = "zurück";
            back.UseVisualStyleBackColor = true;
            back.Click += back_Click;
            // 
            // labelNeu
            // 
            labelNeu.AutoSize = true;
            labelNeu.Location = new Point(12, 12);
            labelNeu.Name = "labelNeu";
            labelNeu.Size = new Size(141, 15);
            labelNeu.TabIndex = 1;
            labelNeu.Text = "Neuen Lehrer hinzufügen";
            // 
            // newTeacherName
            // 
            newTeacherName.Location = new Point(12, 55);
            newTeacherName.Name = "newTeacherName";
            newTeacherName.Size = new Size(205, 23);
            newTeacherName.TabIndex = 3;
            // 
            // newTeacherEmail
            // 
            newTeacherEmail.Location = new Point(12, 101);
            newTeacherEmail.Name = "newTeacherEmail";
            newTeacherEmail.Size = new Size(205, 23);
            newTeacherEmail.TabIndex = 5;
            // 
            // newTeacherTelefon
            // 
            newTeacherTelefon.Location = new Point(12, 147);
            newTeacherTelefon.Name = "newTeacherTelefon";
            newTeacherTelefon.Size = new Size(205, 23);
            newTeacherTelefon.TabIndex = 7;
            // 
            // labelNeuName
            // 
            labelNeuName.AutoSize = true;
            labelNeuName.Location = new Point(12, 37);
            labelNeuName.Name = "labelNeuName";
            labelNeuName.Size = new Size(39, 15);
            labelNeuName.TabIndex = 2;
            labelNeuName.Text = "Name";
            // 
            // labelNeuEmail
            // 
            labelNeuEmail.AutoSize = true;
            labelNeuEmail.Location = new Point(12, 83);
            labelNeuEmail.Name = "labelNeuEmail";
            labelNeuEmail.Size = new Size(41, 15);
            labelNeuEmail.TabIndex = 4;
            labelNeuEmail.Text = "E-Mail";
            // 
            // labelNeuTelefon
            // 
            labelNeuTelefon.AutoSize = true;
            labelNeuTelefon.Location = new Point(12, 129);
            labelNeuTelefon.Name = "labelNeuTelefon";
            labelNeuTelefon.Size = new Size(46, 15);
            labelNeuTelefon.TabIndex = 6;
            labelNeuTelefon.Text = "Telefon";
            // 
            // OKnewTeacherBtn
            // 
            OKnewTeacherBtn.Location = new Point(223, 55);
            OKnewTeacherBtn.Name = "OKnewTeacherBtn";
            OKnewTeacherBtn.Size = new Size(100, 115);
            OKnewTeacherBtn.TabIndex = 8;
            OKnewTeacherBtn.Text = "Speichern";
            OKnewTeacherBtn.UseVisualStyleBackColor = true;
            OKnewTeacherBtn.Click += OKnewTeacherBtn_Click;
            // 
            // labelBearbeiten
            // 
            labelBearbeiten.AutoSize = true;
            labelBearbeiten.Location = new Point(12, 191);
            labelBearbeiten.Name = "labelBearbeiten";
            labelBearbeiten.Size = new Size(158, 15);
            labelBearbeiten.TabIndex = 9;
            labelBearbeiten.Text = "Ausgewählten Lehrer ändern";
            // 
            // currentTeacherName
            // 
            currentTeacherName.Location = new Point(12, 232);
            currentTeacherName.Name = "currentTeacherName";
            currentTeacherName.ReadOnly = true;
            currentTeacherName.Size = new Size(205, 23);
            currentTeacherName.TabIndex = 11;
            // 
            // editTeacherName
            // 
            editTeacherName.Location = new Point(12, 278);
            editTeacherName.Name = "editTeacherName";
            editTeacherName.Size = new Size(205, 23);
            editTeacherName.TabIndex = 13;
            // 
            // editTeacherEmail
            // 
            editTeacherEmail.Location = new Point(12, 324);
            editTeacherEmail.Name = "editTeacherEmail";
            editTeacherEmail.Size = new Size(205, 23);
            editTeacherEmail.TabIndex = 15;
            // 
            // editTeacherTelefon
            // 
            editTeacherTelefon.Location = new Point(12, 370);
            editTeacherTelefon.Name = "editTeacherTelefon";
            editTeacherTelefon.Size = new Size(205, 23);
            editTeacherTelefon.TabIndex = 17;
            // 
            // labelAktuell
            // 
            labelAktuell.AutoSize = true;
            labelAktuell.Location = new Point(12, 214);
            labelAktuell.Name = "labelAktuell";
            labelAktuell.Size = new Size(89, 15);
            labelAktuell.TabIndex = 10;
            labelAktuell.Text = "Aktueller Name";
            // 
            // labelEditName
            // 
            labelEditName.AutoSize = true;
            labelEditName.Location = new Point(12, 260);
            labelEditName.Name = "labelEditName";
            labelEditName.Size = new Size(74, 15);
            labelEditName.TabIndex = 12;
            labelEditName.Text = "Neuer Name";
            // 
            // labelEditEmail
            // 
            labelEditEmail.AutoSize = true;
            labelEditEmail.Location = new Point(12, 306);
            labelEditEmail.Name = "labelEditEmail";
            labelEditEmail.Size = new Size(41, 15);
            labelEditEmail.TabIndex = 14;
            labelEditEmail.Text = "E-Mail";
            // 
            // labelEditTelefon
            // 
            labelEditTelefon.AutoSize = true;
            labelEditTelefon.Location = new Point(12, 352);
            labelEditTelefon.Name = "labelEditTelefon";
            labelEditTelefon.Size = new Size(46, 15);
            labelEditTelefon.TabIndex = 16;
            labelEditTelefon.Text = "Telefon";
            // 
            // OKchangeTeacherBtn
            // 
            OKchangeTeacherBtn.Location = new Point(223, 278);
            OKchangeTeacherBtn.Name = "OKchangeTeacherBtn";
            OKchangeTeacherBtn.Size = new Size(100, 115);
            OKchangeTeacherBtn.TabIndex = 18;
            OKchangeTeacherBtn.Text = "Ändern";
            OKchangeTeacherBtn.UseVisualStyleBackColor = true;
            OKchangeTeacherBtn.Click += OKchangeTeacherBtn_Click;
            // 
            // labelLoeschen
            // 
            labelLoeschen.AutoSize = true;
            labelLoeschen.Location = new Point(12, 414);
            labelLoeschen.Name = "labelLoeschen";
            labelLoeschen.Size = new Size(162, 15);
            labelLoeschen.TabIndex = 19;
            labelLoeschen.Text = "Ausgewählten Lehrer löschen";
            // 
            // deleteTeacherName
            // 
            deleteTeacherName.Location = new Point(12, 437);
            deleteTeacherName.Name = "deleteTeacherName";
            deleteTeacherName.ReadOnly = true;
            deleteTeacherName.Size = new Size(205, 23);
            deleteTeacherName.TabIndex = 20;
            // 
            // OKdeleteTeacherBtn
            // 
            OKdeleteTeacherBtn.Location = new Point(223, 437);
            OKdeleteTeacherBtn.Name = "OKdeleteTeacherBtn";
            OKdeleteTeacherBtn.Size = new Size(100, 23);
            OKdeleteTeacherBtn.TabIndex = 21;
            OKdeleteTeacherBtn.Text = "Löschen";
            OKdeleteTeacherBtn.UseVisualStyleBackColor = true;
            OKdeleteTeacherBtn.Click += OKdeleteTeacherBtn_Click;
            // 
            // reloadTeacherBtn
            // 
            reloadTeacherBtn.Location = new Point(12, 485);
            reloadTeacherBtn.Name = "reloadTeacherBtn";
            reloadTeacherBtn.Size = new Size(311, 29);
            reloadTeacherBtn.TabIndex = 22;
            reloadTeacherBtn.Text = "Lehrer neu laden";
            reloadTeacherBtn.UseVisualStyleBackColor = true;
            reloadTeacherBtn.Click += reloadTeacherBtn_Click;
            // 
            // lehrerGrid
            // 
            lehrerGrid.AllowUserToAddRows = false;
            lehrerGrid.AllowUserToDeleteRows = false;
            lehrerGrid.AllowUserToResizeRows = false;
            lehrerGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            lehrerGrid.Location = new Point(350, 42);
            lehrerGrid.MultiSelect = false;
            lehrerGrid.Name = "lehrerGrid";
            lehrerGrid.ReadOnly = true;
            lehrerGrid.RowHeadersVisible = false;
            lehrerGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            lehrerGrid.Size = new Size(610, 245);
            lehrerGrid.TabIndex = 23;
            lehrerGrid.SelectionChanged += lehrerGrid_SelectionChanged;
            // 
            // labelInfo
            // 
            labelInfo.AutoSize = true;
            labelInfo.Location = new Point(350, 301);
            labelInfo.Name = "labelInfo";
            labelInfo.Size = new Size(208, 15);
            labelInfo.TabIndex = 24;
            labelInfo.Text = "Information zum ausgewählten Lehrer";
            // 
            // infoTitel
            // 
            infoTitel.Location = new Point(350, 342);
            infoTitel.Name = "infoTitel";
            infoTitel.PlaceholderText = "z.B. Lieblingsfach";
            infoTitel.Size = new Size(165, 23);
            infoTitel.TabIndex = 26;
            // 
            // info
            // 
            info.Location = new Point(521, 342);
            info.Name = "info";
            info.PlaceholderText = "z.B. Mathematik";
            info.Size = new Size(270, 23);
            info.TabIndex = 28;
            // 
            // labelInfoTitel
            // 
            labelInfoTitel.AutoSize = true;
            labelInfoTitel.Location = new Point(350, 324);
            labelInfoTitel.Name = "labelInfoTitel";
            labelInfoTitel.Size = new Size(30, 15);
            labelInfoTitel.TabIndex = 25;
            labelInfoTitel.Text = "Titel";
            // 
            // labelInformation
            // 
            labelInformation.AutoSize = true;
            labelInformation.Location = new Point(521, 324);
            labelInformation.Name = "labelInformation";
            labelInformation.Size = new Size(70, 15);
            labelInformation.TabIndex = 27;
            labelInformation.Text = "Information";
            // 
            // OKchangingInfo
            // 
            OKchangingInfo.Location = new Point(797, 342);
            OKchangingInfo.Name = "OKchangingInfo";
            OKchangingInfo.Size = new Size(163, 23);
            OKchangingInfo.TabIndex = 29;
            OKchangingInfo.Text = "Information speichern";
            OKchangingInfo.UseVisualStyleBackColor = true;
            OKchangingInfo.Click += OKchangingInfo_Click;
            // 
            // lehrerInfoGrid
            // 
            lehrerInfoGrid.AllowUserToAddRows = false;
            lehrerInfoGrid.AllowUserToDeleteRows = false;
            lehrerInfoGrid.AllowUserToResizeRows = false;
            lehrerInfoGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            lehrerInfoGrid.Location = new Point(350, 376);
            lehrerInfoGrid.MultiSelect = false;
            lehrerInfoGrid.Name = "lehrerInfoGrid";
            lehrerInfoGrid.ReadOnly = true;
            lehrerInfoGrid.RowHeadersVisible = false;
            lehrerInfoGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            lehrerInfoGrid.Size = new Size(610, 138);
            lehrerInfoGrid.TabIndex = 30;
            lehrerInfoGrid.SelectionChanged += lehrerInfoGrid_SelectionChanged;
            // 
            // Lehrer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(984, 561);
            Controls.Add(lehrerInfoGrid);
            Controls.Add(OKchangingInfo);
            Controls.Add(info);
            Controls.Add(labelInformation);
            Controls.Add(infoTitel);
            Controls.Add(labelInfoTitel);
            Controls.Add(labelInfo);
            Controls.Add(lehrerGrid);
            Controls.Add(reloadTeacherBtn);
            Controls.Add(OKdeleteTeacherBtn);
            Controls.Add(deleteTeacherName);
            Controls.Add(labelLoeschen);
            Controls.Add(OKchangeTeacherBtn);
            Controls.Add(editTeacherTelefon);
            Controls.Add(labelEditTelefon);
            Controls.Add(editTeacherEmail);
            Controls.Add(labelEditEmail);
            Controls.Add(editTeacherName);
            Controls.Add(labelEditName);
            Controls.Add(currentTeacherName);
            Controls.Add(labelAktuell);
            Controls.Add(labelBearbeiten);
            Controls.Add(OKnewTeacherBtn);
            Controls.Add(newTeacherTelefon);
            Controls.Add(labelNeuTelefon);
            Controls.Add(newTeacherEmail);
            Controls.Add(labelNeuEmail);
            Controls.Add(newTeacherName);
            Controls.Add(labelNeuName);
            Controls.Add(labelNeu);
            Controls.Add(back);
            Name = "Lehrer";
            StartPosition = FormStartPosition.Manual;
            Text = "SchulApp - Lehrer";
            ((System.ComponentModel.ISupportInitialize)lehrerGrid).EndInit();
            ((System.ComponentModel.ISupportInitialize)lehrerInfoGrid).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button back;
        private Label labelNeu;
        private TextBox newTeacherName;
        private TextBox newTeacherEmail;
        private TextBox newTeacherTelefon;
        private Label labelNeuName;
        private Label labelNeuEmail;
        private Label labelNeuTelefon;
        private Button OKnewTeacherBtn;
        private Label labelBearbeiten;
        private TextBox currentTeacherName;
        private TextBox editTeacherName;
        private TextBox editTeacherEmail;
        private TextBox editTeacherTelefon;
        private Label labelAktuell;
        private Label labelEditName;
        private Label labelEditEmail;
        private Label labelEditTelefon;
        private Button OKchangeTeacherBtn;
        private Label labelLoeschen;
        private TextBox deleteTeacherName;
        private Button OKdeleteTeacherBtn;
        private Button reloadTeacherBtn;
        private DataGridView lehrerGrid;
        private Label labelInfo;
        private TextBox infoTitel;
        private TextBox info;
        private Label labelInfoTitel;
        private Label labelInformation;
        private Button OKchangingInfo;
        private DataGridView lehrerInfoGrid;
    }
}
