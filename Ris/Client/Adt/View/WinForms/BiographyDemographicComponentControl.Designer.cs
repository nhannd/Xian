namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class BiographyDemographicComponentControl
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
            this._patientIdentifierGroup = new System.Windows.Forms.GroupBox();
            this._healthcardVersionCode = new ClearCanvas.Controls.WinForms.TextField();
            this._mrnSite = new ClearCanvas.Controls.WinForms.TextField();
            this._healthcardProvince = new ClearCanvas.Controls.WinForms.TextField();
            this._mrn = new ClearCanvas.Controls.WinForms.TextField();
            this._healthcard = new ClearCanvas.Controls.WinForms.TextField();
            this._healthcardExpiry = new ClearCanvas.Controls.WinForms.TextField();
            this._personalInfoGroup = new System.Windows.Forms.GroupBox();
            this._primaryLanguage = new ClearCanvas.Controls.WinForms.TextField();
            this._religion = new ClearCanvas.Controls.WinForms.TextField();
            this._degree = new ClearCanvas.Controls.WinForms.TextField();
            this._prefix = new ClearCanvas.Controls.WinForms.TextField();
            this._suffix = new ClearCanvas.Controls.WinForms.TextField();
            this._middleName = new ClearCanvas.Controls.WinForms.TextField();
            this._timeOfDeath = new ClearCanvas.Controls.WinForms.TextField();
            this._dateOfBirth = new ClearCanvas.Controls.WinForms.TextField();
            this._sex = new ClearCanvas.Controls.WinForms.TextField();
            this._familyName = new ClearCanvas.Controls.WinForms.TextField();
            this._givenName = new ClearCanvas.Controls.WinForms.TextField();
            this._patientIdentifierGroup.SuspendLayout();
            this._personalInfoGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // _patientIdentifierGroup
            // 
            this._patientIdentifierGroup.Controls.Add(this._healthcardVersionCode);
            this._patientIdentifierGroup.Controls.Add(this._mrnSite);
            this._patientIdentifierGroup.Controls.Add(this._healthcardProvince);
            this._patientIdentifierGroup.Controls.Add(this._mrn);
            this._patientIdentifierGroup.Controls.Add(this._healthcard);
            this._patientIdentifierGroup.Controls.Add(this._healthcardExpiry);
            this._patientIdentifierGroup.Location = new System.Drawing.Point(416, 3);
            this._patientIdentifierGroup.Name = "_patientIdentifierGroup";
            this._patientIdentifierGroup.Size = new System.Drawing.Size(275, 178);
            this._patientIdentifierGroup.TabIndex = 42;
            this._patientIdentifierGroup.TabStop = false;
            this._patientIdentifierGroup.Text = "Patient Identifier";
            // 
            // _healthcardVersionCode
            // 
            this._healthcardVersionCode.AutoSize = true;
            this._healthcardVersionCode.LabelText = "Version Code";
            this._healthcardVersionCode.Location = new System.Drawing.Point(5, 114);
            this._healthcardVersionCode.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
            this._healthcardVersionCode.Mask = "";
            this._healthcardVersionCode.Name = "_healthcardVersionCode";
            this._healthcardVersionCode.ReadOnly = true;
            this._healthcardVersionCode.Size = new System.Drawing.Size(127, 40);
            this._healthcardVersionCode.TabIndex = 53;
            this._healthcardVersionCode.Value = null;
            // 
            // _mrnSite
            // 
            this._mrnSite.LabelText = "Site";
            this._mrnSite.Location = new System.Drawing.Point(138, 17);
            this._mrnSite.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._mrnSite.Mask = "";
            this._mrnSite.Name = "_mrnSite";
            this._mrnSite.ReadOnly = true;
            this._mrnSite.Size = new System.Drawing.Size(127, 44);
            this._mrnSite.TabIndex = 52;
            this._mrnSite.Value = null;
            // 
            // _healthcardProvince
            // 
            this._healthcardProvince.LabelText = "Province";
            this._healthcardProvince.Location = new System.Drawing.Point(140, 66);
            this._healthcardProvince.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._healthcardProvince.Mask = "";
            this._healthcardProvince.Name = "_healthcardProvince";
            this._healthcardProvince.ReadOnly = true;
            this._healthcardProvince.Size = new System.Drawing.Size(127, 44);
            this._healthcardProvince.TabIndex = 51;
            this._healthcardProvince.Value = null;
            // 
            // _mrn
            // 
            this._mrn.LabelText = "MRN";
            this._mrn.Location = new System.Drawing.Point(5, 18);
            this._mrn.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._mrn.Mask = "";
            this._mrn.Name = "_mrn";
            this._mrn.ReadOnly = true;
            this._mrn.Size = new System.Drawing.Size(127, 44);
            this._mrn.TabIndex = 48;
            this._mrn.Value = null;
            // 
            // _healthcard
            // 
            this._healthcard.AutoSize = true;
            this._healthcard.LabelText = "Healthcard";
            this._healthcard.Location = new System.Drawing.Point(5, 66);
            this._healthcard.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
            this._healthcard.Mask = "";
            this._healthcard.Name = "_healthcard";
            this._healthcard.ReadOnly = true;
            this._healthcard.Size = new System.Drawing.Size(127, 40);
            this._healthcard.TabIndex = 32;
            this._healthcard.Value = null;
            // 
            // _healthcardExpiry
            // 
            this._healthcardExpiry.AutoSize = true;
            this._healthcardExpiry.LabelText = "Healthcard Expiry Date";
            this._healthcardExpiry.Location = new System.Drawing.Point(140, 114);
            this._healthcardExpiry.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
            this._healthcardExpiry.Mask = "";
            this._healthcardExpiry.Name = "_healthcardExpiry";
            this._healthcardExpiry.ReadOnly = true;
            this._healthcardExpiry.Size = new System.Drawing.Size(127, 40);
            this._healthcardExpiry.TabIndex = 33;
            this._healthcardExpiry.Value = null;
            // 
            // _personalInfoGroup
            // 
            this._personalInfoGroup.Controls.Add(this._primaryLanguage);
            this._personalInfoGroup.Controls.Add(this._religion);
            this._personalInfoGroup.Controls.Add(this._degree);
            this._personalInfoGroup.Controls.Add(this._prefix);
            this._personalInfoGroup.Controls.Add(this._suffix);
            this._personalInfoGroup.Controls.Add(this._middleName);
            this._personalInfoGroup.Controls.Add(this._timeOfDeath);
            this._personalInfoGroup.Controls.Add(this._dateOfBirth);
            this._personalInfoGroup.Controls.Add(this._sex);
            this._personalInfoGroup.Controls.Add(this._familyName);
            this._personalInfoGroup.Controls.Add(this._givenName);
            this._personalInfoGroup.Location = new System.Drawing.Point(3, 3);
            this._personalInfoGroup.Name = "_personalInfoGroup";
            this._personalInfoGroup.Size = new System.Drawing.Size(407, 210);
            this._personalInfoGroup.TabIndex = 41;
            this._personalInfoGroup.TabStop = false;
            this._personalInfoGroup.Text = "Personal Info";
            // 
            // _primaryLanguage
            // 
            this._primaryLanguage.AutoSize = true;
            this._primaryLanguage.LabelText = "Primary Language";
            this._primaryLanguage.Location = new System.Drawing.Point(138, 160);
            this._primaryLanguage.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
            this._primaryLanguage.Mask = "";
            this._primaryLanguage.Name = "_primaryLanguage";
            this._primaryLanguage.ReadOnly = true;
            this._primaryLanguage.Size = new System.Drawing.Size(127, 40);
            this._primaryLanguage.TabIndex = 49;
            this._primaryLanguage.Value = null;
            // 
            // _religion
            // 
            this._religion.AutoSize = true;
            this._religion.LabelText = "Religion";
            this._religion.Location = new System.Drawing.Point(5, 160);
            this._religion.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
            this._religion.Mask = "";
            this._religion.Name = "_religion";
            this._religion.ReadOnly = true;
            this._religion.Size = new System.Drawing.Size(127, 40);
            this._religion.TabIndex = 48;
            this._religion.Value = null;
            // 
            // _degree
            // 
            this._degree.LabelText = "Degree";
            this._degree.Location = new System.Drawing.Point(271, 66);
            this._degree.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._degree.Mask = "";
            this._degree.Name = "_degree";
            this._degree.ReadOnly = true;
            this._degree.Size = new System.Drawing.Size(127, 44);
            this._degree.TabIndex = 47;
            this._degree.Value = null;
            // 
            // _prefix
            // 
            this._prefix.LabelText = "Prefix";
            this._prefix.Location = new System.Drawing.Point(5, 66);
            this._prefix.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._prefix.Mask = "";
            this._prefix.Name = "_prefix";
            this._prefix.ReadOnly = true;
            this._prefix.Size = new System.Drawing.Size(127, 44);
            this._prefix.TabIndex = 45;
            this._prefix.Value = null;
            // 
            // _suffix
            // 
            this._suffix.LabelText = "Suffix";
            this._suffix.Location = new System.Drawing.Point(138, 66);
            this._suffix.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._suffix.Mask = "";
            this._suffix.Name = "_suffix";
            this._suffix.ReadOnly = true;
            this._suffix.Size = new System.Drawing.Size(127, 44);
            this._suffix.TabIndex = 46;
            this._suffix.Value = null;
            // 
            // _middleName
            // 
            this._middleName.LabelText = "Middle Name";
            this._middleName.Location = new System.Drawing.Point(271, 18);
            this._middleName.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._middleName.Mask = "";
            this._middleName.Name = "_middleName";
            this._middleName.ReadOnly = true;
            this._middleName.Size = new System.Drawing.Size(127, 44);
            this._middleName.TabIndex = 44;
            this._middleName.Value = null;
            // 
            // _timeOfDeath
            // 
            this._timeOfDeath.LabelText = "Time of Death";
            this._timeOfDeath.Location = new System.Drawing.Point(271, 114);
            this._timeOfDeath.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._timeOfDeath.Mask = "";
            this._timeOfDeath.Name = "_timeOfDeath";
            this._timeOfDeath.ReadOnly = true;
            this._timeOfDeath.Size = new System.Drawing.Size(127, 42);
            this._timeOfDeath.TabIndex = 43;
            this._timeOfDeath.Value = null;
            // 
            // _dateOfBirth
            // 
            this._dateOfBirth.LabelText = "Date of Birth";
            this._dateOfBirth.Location = new System.Drawing.Point(138, 114);
            this._dateOfBirth.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._dateOfBirth.Mask = "";
            this._dateOfBirth.Name = "_dateOfBirth";
            this._dateOfBirth.ReadOnly = true;
            this._dateOfBirth.Size = new System.Drawing.Size(127, 42);
            this._dateOfBirth.TabIndex = 42;
            this._dateOfBirth.Value = null;
            // 
            // _sex
            // 
            this._sex.LabelText = "Sex";
            this._sex.Location = new System.Drawing.Point(5, 114);
            this._sex.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._sex.Mask = "";
            this._sex.Name = "_sex";
            this._sex.ReadOnly = true;
            this._sex.Size = new System.Drawing.Size(127, 42);
            this._sex.TabIndex = 41;
            this._sex.Value = null;
            // 
            // _familyName
            // 
            this._familyName.LabelText = "Family Name";
            this._familyName.Location = new System.Drawing.Point(5, 18);
            this._familyName.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._familyName.Mask = "";
            this._familyName.Name = "_familyName";
            this._familyName.ReadOnly = true;
            this._familyName.Size = new System.Drawing.Size(127, 44);
            this._familyName.TabIndex = 39;
            this._familyName.Value = null;
            // 
            // _givenName
            // 
            this._givenName.LabelText = "Given Name";
            this._givenName.Location = new System.Drawing.Point(138, 18);
            this._givenName.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._givenName.Mask = "";
            this._givenName.Name = "_givenName";
            this._givenName.ReadOnly = true;
            this._givenName.Size = new System.Drawing.Size(127, 44);
            this._givenName.TabIndex = 40;
            this._givenName.Value = null;
            // 
            // BiographyDemographicComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._patientIdentifierGroup);
            this.Controls.Add(this._personalInfoGroup);
            this.Name = "BiographyDemographicComponentControl";
            this.Size = new System.Drawing.Size(699, 223);
            this._patientIdentifierGroup.ResumeLayout(false);
            this._patientIdentifierGroup.PerformLayout();
            this._personalInfoGroup.ResumeLayout(false);
            this._personalInfoGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox _patientIdentifierGroup;
        private ClearCanvas.Controls.WinForms.TextField _healthcardVersionCode;
        private ClearCanvas.Controls.WinForms.TextField _mrnSite;
        private ClearCanvas.Controls.WinForms.TextField _healthcardProvince;
        private ClearCanvas.Controls.WinForms.TextField _mrn;
        private ClearCanvas.Controls.WinForms.TextField _healthcard;
        private ClearCanvas.Controls.WinForms.TextField _healthcardExpiry;
        private System.Windows.Forms.GroupBox _personalInfoGroup;
        private ClearCanvas.Controls.WinForms.TextField _primaryLanguage;
        private ClearCanvas.Controls.WinForms.TextField _religion;
        private ClearCanvas.Controls.WinForms.TextField _degree;
        private ClearCanvas.Controls.WinForms.TextField _prefix;
        private ClearCanvas.Controls.WinForms.TextField _suffix;
        private ClearCanvas.Controls.WinForms.TextField _middleName;
        private ClearCanvas.Controls.WinForms.TextField _timeOfDeath;
        private ClearCanvas.Controls.WinForms.TextField _dateOfBirth;
        private ClearCanvas.Controls.WinForms.TextField _sex;
        private ClearCanvas.Controls.WinForms.TextField _familyName;
        private ClearCanvas.Controls.WinForms.TextField _givenName;

    }
}
