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
            this._dateOfBirth = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._dateOfDeath = new ClearCanvas.Controls.WinForms.DateTimeField();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._patientIdentifierList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // _middleName
            // 
            this._middleName.Dock = System.Windows.Forms.DockStyle.Fill;
            this._middleName.LabelText = "Middle Name";
            this._middleName.Location = new System.Drawing.Point(194, 2);
            this._middleName.Margin = new System.Windows.Forms.Padding(2);
            this._middleName.Name = "_middleName";
            this._middleName.Size = new System.Drawing.Size(93, 38);
            this._middleName.TabIndex = 2;
            this._middleName.Value = null;
            // 
            // _givenName
            // 
            this._givenName.Dock = System.Windows.Forms.DockStyle.Fill;
            this._givenName.LabelText = "Given Name";
            this._givenName.Location = new System.Drawing.Point(98, 2);
            this._givenName.Margin = new System.Windows.Forms.Padding(2);
            this._givenName.Name = "_givenName";
            this._givenName.Size = new System.Drawing.Size(92, 38);
            this._givenName.TabIndex = 1;
            this._givenName.Value = null;
            // 
            // _familyName
            // 
            this._familyName.Dock = System.Windows.Forms.DockStyle.Fill;
            this._familyName.LabelText = "Family Name";
            this._familyName.Location = new System.Drawing.Point(2, 2);
            this._familyName.Margin = new System.Windows.Forms.Padding(2);
            this._familyName.Name = "_familyName";
            this._familyName.Size = new System.Drawing.Size(92, 38);
            this._familyName.TabIndex = 0;
            this._familyName.Value = null;
            // 
            // _sex
            // 
            this._sex.DataSource = null;
            this._sex.Dock = System.Windows.Forms.DockStyle.Fill;
            this._sex.LabelText = "Sex";
            this._sex.Location = new System.Drawing.Point(2, 44);
            this._sex.Margin = new System.Windows.Forms.Padding(2);
            this._sex.Name = "_sex";
            this._sex.Size = new System.Drawing.Size(92, 54);
            this._sex.TabIndex = 3;
            this._sex.Value = null;
            // 
            // _dateOfBirth
            // 
            this._dateOfBirth.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dateOfBirth.LabelText = "Date of Birth";
            this._dateOfBirth.Location = new System.Drawing.Point(98, 44);
            this._dateOfBirth.Margin = new System.Windows.Forms.Padding(2);
            this._dateOfBirth.Name = "_dateOfBirth";
            this._dateOfBirth.Nullable = false;
            this._dateOfBirth.Size = new System.Drawing.Size(92, 54);
            this._dateOfBirth.TabIndex = 4;
            this._dateOfBirth.Value = null;
            // 
            // _dateOfDeath
            // 
            this._dateOfDeath.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dateOfDeath.LabelText = "Date of Death";
            this._dateOfDeath.Location = new System.Drawing.Point(194, 44);
            this._dateOfDeath.Margin = new System.Windows.Forms.Padding(2);
            this._dateOfDeath.Name = "_dateOfDeath";
            this._dateOfDeath.Nullable = true;
            this._dateOfDeath.Size = new System.Drawing.Size(93, 54);
            this._dateOfDeath.TabIndex = 5;
            this._dateOfDeath.Value = null;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this._patientIdentifierList, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(295, 424);
            this.tableLayoutPanel1.TabIndex = 38;
            // 
            // _patientIdentifierList
            // 
            this._patientIdentifierList.AutoSize = true;
            this._patientIdentifierList.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._patientIdentifierList.DataSource = null;
            this._patientIdentifierList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._patientIdentifierList.Location = new System.Drawing.Point(4, 123);
            this._patientIdentifierList.Margin = new System.Windows.Forms.Padding(4);
            this._patientIdentifierList.Name = "_patientIdentifierList";
            this._patientIdentifierList.Size = new System.Drawing.Size(287, 297);
            this._patientIdentifierList.TabIndex = 1;
            this._patientIdentifierList.ToolbarModel = null;
            this._patientIdentifierList.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this._patientIdentifierList.ItemDoubleClicked += new System.EventHandler(this._patientIdentifierList_ItemDoubleClicked);
            this._patientIdentifierList.SelectionChanged += new System.EventHandler(this._patientIdentifierList_SelectionChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Controls.Add(this._dateOfDeath, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this._familyName, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this._dateOfBirth, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this._givenName, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this._sex, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this._middleName, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(289, 100);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 106);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(289, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Patient Identifiers";
            // 
            // PatientEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PatientEditorControl";
            this.Size = new System.Drawing.Size(295, 424);
            this.Load += new System.EventHandler(this.PatientEditorControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Controls.WinForms.TextField _familyName;
        private ClearCanvas.Controls.WinForms.TextField _givenName;
        private ClearCanvas.Controls.WinForms.TextField _middleName;
        private ClearCanvas.Controls.WinForms.ComboBoxField _sex;
        private ClearCanvas.Desktop.View.WinForms.TableView _patientIdentifierList;
        private ClearCanvas.Controls.WinForms.DateTimeField _dateOfBirth;
        private ClearCanvas.Controls.WinForms.DateTimeField _dateOfDeath;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
    }
}
