namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class PatientEditorControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._middleName = new ClearCanvas.Controls.WinForms.TextField();
            this._givenName = new ClearCanvas.Controls.WinForms.TextField();
            this._familyName = new ClearCanvas.Controls.WinForms.TextField();
            this._sex = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._patientIdentifierList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._identifierAddButton = new System.Windows.Forms.Button();
            this._identifierDeleteButton = new System.Windows.Forms.Button();
            this._identifierUpdateButton = new System.Windows.Forms.Button();
            this._dateOfBirth = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._dateOfDeath = new ClearCanvas.Controls.WinForms.DateTimeField();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _middleName
            // 
            this._middleName.LabelText = "Middle Name";
            this._middleName.Location = new System.Drawing.Point(464, 6);
            this._middleName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._middleName.Name = "_middleName";
            this._middleName.Size = new System.Drawing.Size(185, 47);
            this._middleName.TabIndex = 25;
            this._middleName.Value = null;
            // 
            // _givenName
            // 
            this._givenName.LabelText = "Given Name";
            this._givenName.Location = new System.Drawing.Point(237, 6);
            this._givenName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._givenName.Name = "_givenName";
            this._givenName.Size = new System.Drawing.Size(185, 47);
            this._givenName.TabIndex = 24;
            this._givenName.Value = null;
            // 
            // _familyName
            // 
            this._familyName.LabelText = "Family Name";
            this._familyName.Location = new System.Drawing.Point(7, 6);
            this._familyName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._familyName.Name = "_familyName";
            this._familyName.Size = new System.Drawing.Size(185, 47);
            this._familyName.TabIndex = 23;
            this._familyName.Value = null;
            // 
            // _sex
            // 
            this._sex.DataSource = null;
            this._sex.LabelText = "Sex";
            this._sex.Location = new System.Drawing.Point(7, 77);
            this._sex.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._sex.Name = "_sex";
            this._sex.Size = new System.Drawing.Size(185, 50);
            this._sex.TabIndex = 26;
            this._sex.Value = null;
            // 
            // _patientIdentifierList
            // 
            this._patientIdentifierList.AutoSize = true;
            this._patientIdentifierList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._patientIdentifierList.DataSource = null;
            this._patientIdentifierList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._patientIdentifierList.Location = new System.Drawing.Point(5, 147);
            this._patientIdentifierList.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this._patientIdentifierList.Name = "_patientIdentifierList";
            this._patientIdentifierList.Size = new System.Drawing.Size(950, 328);
            this._patientIdentifierList.TabIndex = 27;
            this._patientIdentifierList.ItemDoubleClicked += new System.EventHandler(this._patientIdentifierList_ItemDoubleClicked);
            // 
            // _identifierAddButton
            // 
            this._identifierAddButton.Location = new System.Drawing.Point(4, 4);
            this._identifierAddButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._identifierAddButton.Name = "_identifierAddButton";
            this._identifierAddButton.Size = new System.Drawing.Size(100, 28);
            this._identifierAddButton.TabIndex = 32;
            this._identifierAddButton.Text = "Add";
            this._identifierAddButton.UseVisualStyleBackColor = true;
            this._identifierAddButton.Click += new System.EventHandler(this._identiferAddButton_Click);
            // 
            // _identifierDeleteButton
            // 
            this._identifierDeleteButton.Location = new System.Drawing.Point(220, 4);
            this._identifierDeleteButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._identifierDeleteButton.Name = "_identifierDeleteButton";
            this._identifierDeleteButton.Size = new System.Drawing.Size(100, 28);
            this._identifierDeleteButton.TabIndex = 33;
            this._identifierDeleteButton.Text = "Delete";
            this._identifierDeleteButton.UseVisualStyleBackColor = true;
            this._identifierDeleteButton.Click += new System.EventHandler(this._identifierDeleteButton_Click);
            // 
            // _identifierUpdateButton
            // 
            this._identifierUpdateButton.Location = new System.Drawing.Point(112, 4);
            this._identifierUpdateButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this._identifierUpdateButton.Name = "_identifierUpdateButton";
            this._identifierUpdateButton.Size = new System.Drawing.Size(100, 28);
            this._identifierUpdateButton.TabIndex = 34;
            this._identifierUpdateButton.Text = "Update";
            this._identifierUpdateButton.UseVisualStyleBackColor = true;
            this._identifierUpdateButton.Click += new System.EventHandler(this._identifierUpdateButton_Click);
            // 
            // _dateOfBirth
            // 
            this._dateOfBirth.LabelText = "Date of Birth";
            this._dateOfBirth.Location = new System.Drawing.Point(237, 77);
            this._dateOfBirth.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._dateOfBirth.Name = "_dateOfBirth";
            this._dateOfBirth.Nullable = false;
            this._dateOfBirth.Size = new System.Drawing.Size(185, 50);
            this._dateOfBirth.TabIndex = 35;
            this._dateOfBirth.Value = null;
            // 
            // _dateOfDeath
            // 
            this._dateOfDeath.LabelText = "Date of Death";
            this._dateOfDeath.Location = new System.Drawing.Point(464, 77);
            this._dateOfDeath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._dateOfDeath.Name = "_dateOfDeath";
            this._dateOfDeath.Nullable = false;
            this._dateOfDeath.Size = new System.Drawing.Size(185, 50);
            this._dateOfDeath.TabIndex = 36;
            this._dateOfDeath.Value = null;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._dateOfDeath);
            this.panel1.Controls.Add(this._familyName);
            this.panel1.Controls.Add(this._dateOfBirth);
            this.panel1.Controls.Add(this._givenName);
            this.panel1.Controls.Add(this._middleName);
            this.panel1.Controls.Add(this._sex);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(954, 136);
            this.panel1.TabIndex = 37;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this._patientIdentifierList, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(960, 522);
            this.tableLayoutPanel1.TabIndex = 38;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this._identifierDeleteButton);
            this.flowLayoutPanel1.Controls.Add(this._identifierUpdateButton);
            this.flowLayoutPanel1.Controls.Add(this._identifierAddButton);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(633, 483);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(324, 36);
            this.flowLayoutPanel1.TabIndex = 39;
            // 
            // PatientEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "PatientEditorControl";
            this.Size = new System.Drawing.Size(960, 522);
            this.Load += new System.EventHandler(this.PatientEditorControl_Load);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Controls.WinForms.TextField _familyName;
        private ClearCanvas.Controls.WinForms.TextField _givenName;
        private ClearCanvas.Controls.WinForms.TextField _middleName;
        private ClearCanvas.Controls.WinForms.ComboBoxField _sex;
        private ClearCanvas.Desktop.View.WinForms.TableView _patientIdentifierList;
        private System.Windows.Forms.Button _identifierAddButton;
        private System.Windows.Forms.Button _identifierDeleteButton;
        private System.Windows.Forms.Button _identifierUpdateButton;
        private ClearCanvas.Controls.WinForms.DateTimeField _dateOfBirth;
        private ClearCanvas.Controls.WinForms.DateTimeField _dateOfDeath;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}
