namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class PatientProfileDetailsEditorControl
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
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this._healthcard = new ClearCanvas.Controls.WinForms.TextField();
            this._mrnSite = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._healthcardProvince = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._healthcardVersionCode = new ClearCanvas.Controls.WinForms.TextField();
            this._healthcardExpiry = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._mrn = new ClearCanvas.Controls.WinForms.TextField();
            this._errorProvider = new ClearCanvas.Desktop.View.WinForms.ValidationErrorProvider();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // _middleName
            // 
            this._middleName.LabelText = "Middle Name";
            this._middleName.Location = new System.Drawing.Point(280, 2);
            this._middleName.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._middleName.Mask = "";
            this._middleName.Name = "_middleName";
            this._middleName.Size = new System.Drawing.Size(122, 38);
            this._middleName.TabIndex = 2;
            this._middleName.Value = null;
            // 
            // _givenName
            // 
            this._givenName.LabelText = "Given Name";
            this._givenName.Location = new System.Drawing.Point(141, 2);
            this._givenName.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._givenName.Mask = "";
            this._givenName.Name = "_givenName";
            this._givenName.Size = new System.Drawing.Size(118, 38);
            this._givenName.TabIndex = 1;
            this._givenName.Value = null;
            // 
            // _familyName
            // 
            this._familyName.LabelText = "Family Name";
            this._familyName.Location = new System.Drawing.Point(2, 2);
            this._familyName.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._familyName.Mask = "";
            this._familyName.Name = "_familyName";
            this._familyName.Size = new System.Drawing.Size(116, 38);
            this._familyName.TabIndex = 0;
            this._familyName.Value = null;
            // 
            // _sex
            // 
            this._sex.DataSource = null;
            this._sex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._sex.LabelText = "Sex";
            this._sex.Location = new System.Drawing.Point(2, 44);
            this._sex.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._sex.Name = "_sex";
            this._sex.Size = new System.Drawing.Size(116, 44);
            this._sex.TabIndex = 3;
            this._sex.Value = null;
            // 
            // _dateOfBirth
            // 
            this._dateOfBirth.LabelText = "Date of Birth";
            this._dateOfBirth.Location = new System.Drawing.Point(141, 44);
            this._dateOfBirth.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._dateOfBirth.Name = "_dateOfBirth";
            this._dateOfBirth.Nullable = false;
            this._dateOfBirth.ShowTime = false;
            this._dateOfBirth.Size = new System.Drawing.Size(118, 44);
            this._dateOfBirth.TabIndex = 4;
            this._dateOfBirth.Value = null;
            // 
            // _dateOfDeath
            // 
            this._dateOfDeath.LabelText = "Date of Death";
            this._dateOfDeath.Location = new System.Drawing.Point(280, 44);
            this._dateOfDeath.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._dateOfDeath.Name = "_dateOfDeath";
            this._dateOfDeath.Nullable = true;
            this._dateOfDeath.ShowTime = false;
            this._dateOfDeath.Size = new System.Drawing.Size(122, 44);
            this._dateOfDeath.TabIndex = 5;
            this._dateOfDeath.Value = null;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(424, 250);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
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
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(417, 90);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(418, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Patient Identifiers";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this._healthcard, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this._mrnSite, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this._healthcardProvince, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this._healthcardVersionCode, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this._healthcardExpiry, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this._mrn, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 112);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(418, 135);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // _healthcard
            // 
            this._healthcard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._healthcard.AutoSize = true;
            this._healthcard.LabelText = "Healthcard";
            this._healthcard.Location = new System.Drawing.Point(2, 47);
            this._healthcard.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
            this._healthcard.Mask = "";
            this._healthcard.Name = "_healthcard";
            this._healthcard.Size = new System.Drawing.Size(190, 40);
            this._healthcard.TabIndex = 2;
            this._healthcard.Value = null;
            // 
            // _mrnSite
            // 
            this._mrnSite.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._mrnSite.AutoSize = true;
            this._mrnSite.DataSource = null;
            this._mrnSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._mrnSite.LabelText = "Site";
            this._mrnSite.Location = new System.Drawing.Point(211, 2);
            this._mrnSite.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
            this._mrnSite.Name = "_mrnSite";
            this._mrnSite.Size = new System.Drawing.Size(190, 41);
            this._mrnSite.TabIndex = 1;
            this._mrnSite.Value = null;
            // 
            // _healthcardProvince
            // 
            this._healthcardProvince.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._healthcardProvince.AutoSize = true;
            this._healthcardProvince.DataSource = null;
            this._healthcardProvince.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._healthcardProvince.LabelText = "Province";
            this._healthcardProvince.Location = new System.Drawing.Point(211, 47);
            this._healthcardProvince.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
            this._healthcardProvince.Name = "_healthcardProvince";
            this._healthcardProvince.Size = new System.Drawing.Size(190, 41);
            this._healthcardProvince.TabIndex = 3;
            this._healthcardProvince.Value = null;
            // 
            // _healthcardVersionCode
            // 
            this._healthcardVersionCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._healthcardVersionCode.AutoSize = true;
            this._healthcardVersionCode.LabelText = "Version Code";
            this._healthcardVersionCode.Location = new System.Drawing.Point(2, 92);
            this._healthcardVersionCode.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
            this._healthcardVersionCode.Mask = "";
            this._healthcardVersionCode.Name = "_healthcardVersionCode";
            this._healthcardVersionCode.Size = new System.Drawing.Size(190, 40);
            this._healthcardVersionCode.TabIndex = 4;
            this._healthcardVersionCode.Value = null;
            // 
            // _healthcardExpiry
            // 
            this._healthcardExpiry.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._healthcardExpiry.AutoSize = true;
            this._healthcardExpiry.LabelText = "Healthcard Expiry Date";
            this._healthcardExpiry.Location = new System.Drawing.Point(211, 92);
            this._healthcardExpiry.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
            this._healthcardExpiry.Name = "_healthcardExpiry";
            this._healthcardExpiry.Nullable = true;
            this._healthcardExpiry.ShowTime = false;
            this._healthcardExpiry.Size = new System.Drawing.Size(190, 42);
            this._healthcardExpiry.TabIndex = 5;
            this._healthcardExpiry.Value = null;
            // 
            // _mrn
            // 
            this._mrn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._mrn.AutoSize = true;
            this._mrn.LabelText = "MRN";
            this._mrn.Location = new System.Drawing.Point(2, 2);
            this._mrn.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._mrn.Mask = "";
            this._mrn.Name = "_mrn";
            this._mrn.Size = new System.Drawing.Size(192, 40);
            this._mrn.TabIndex = 0;
            this._mrn.Value = null;
            // 
            // _errorProvider
            // 
            this._errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this._errorProvider.ContainerControl = this;
            // 
            // PatientProfileDetailsEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PatientProfileDetailsEditorControl";
            this.Size = new System.Drawing.Size(429, 258);
            this.Load += new System.EventHandler(this.PatientEditorControl_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Controls.WinForms.TextField _familyName;
        private ClearCanvas.Controls.WinForms.TextField _givenName;
        private ClearCanvas.Controls.WinForms.TextField _middleName;
        private ClearCanvas.Controls.WinForms.ComboBoxField _sex;
        private ClearCanvas.Controls.WinForms.DateTimeField _dateOfBirth;
        private ClearCanvas.Controls.WinForms.DateTimeField _dateOfDeath;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private ClearCanvas.Controls.WinForms.TextField _mrn;
        private ClearCanvas.Controls.WinForms.TextField _healthcard;
        private ClearCanvas.Controls.WinForms.ComboBoxField _mrnSite;
        private ClearCanvas.Controls.WinForms.ComboBoxField _healthcardProvince;
        private ClearCanvas.Controls.WinForms.DateTimeField _healthcardExpiry;
        private ClearCanvas.Controls.WinForms.TextField _healthcardVersionCode;
        private ClearCanvas.Desktop.View.WinForms.ValidationErrorProvider _errorProvider;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
    }
}
