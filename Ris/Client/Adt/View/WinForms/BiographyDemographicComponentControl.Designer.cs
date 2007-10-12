#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            ClearCanvas.Desktop.Selection selection2 = new ClearCanvas.Desktop.Selection();
            ClearCanvas.Desktop.Selection selection3 = new ClearCanvas.Desktop.Selection();
            ClearCanvas.Desktop.Selection selection4 = new ClearCanvas.Desktop.Selection();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this._addressPage = new System.Windows.Forms.TabPage();
            this._addressList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._phonePage = new System.Windows.Forms.TabPage();
            this._phoneList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._emailPage = new System.Windows.Forms.TabPage();
            this._emailList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._contactPage = new System.Windows.Forms.TabPage();
            this._contactList = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._selectedProfile = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._patientIdentifierGroup = new System.Windows.Forms.GroupBox();
            this._healthcardVersionCode = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._mrnSite = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._healthcardProvince = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._mrn = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._healthcard = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._healthcardExpiry = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._personalInfoGroup = new System.Windows.Forms.GroupBox();
            this._primaryLanguage = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._religion = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._degree = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._prefix = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._suffix = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._middleName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._timeOfDeath = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._dateOfBirth = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._sex = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._familyName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._givenName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this._addressPage.SuspendLayout();
            this._phonePage.SuspendLayout();
            this._emailPage.SuspendLayout();
            this._contactPage.SuspendLayout();
            this._patientIdentifierGroup.SuspendLayout();
            this._personalInfoGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._selectedProfile);
            this.splitContainer1.Panel1.Controls.Add(this._patientIdentifierGroup);
            this.splitContainer1.Panel1.Controls.Add(this._personalInfoGroup);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(706, 567);
            this.splitContainer1.SplitterDistance = 239;
            this.splitContainer1.TabIndex = 3;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this._addressPage);
            this.tabControl1.Controls.Add(this._phonePage);
            this.tabControl1.Controls.Add(this._emailPage);
            this.tabControl1.Controls.Add(this._contactPage);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(706, 324);
            this.tabControl1.TabIndex = 4;
            // 
            // _addressPage
            // 
            this._addressPage.Controls.Add(this._addressList);
            this._addressPage.Location = new System.Drawing.Point(4, 22);
            this._addressPage.Name = "_addressPage";
            this._addressPage.Padding = new System.Windows.Forms.Padding(3);
            this._addressPage.Size = new System.Drawing.Size(698, 298);
            this._addressPage.TabIndex = 0;
            this._addressPage.Text = "Addresses";
            this._addressPage.UseVisualStyleBackColor = true;
            // 
            // _addressList
            // 
            this._addressList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._addressList.Location = new System.Drawing.Point(3, 3);
            this._addressList.MenuModel = null;
            this._addressList.Name = "_addressList";
            this._addressList.ReadOnly = false;
            this._addressList.Selection = selection1;
            this._addressList.ShowToolbar = false;
            this._addressList.Size = new System.Drawing.Size(692, 292);
            this._addressList.TabIndex = 0;
            this._addressList.Table = null;
            this._addressList.ToolbarModel = null;
            this._addressList.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._addressList.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._addressList.ItemDoubleClicked += new System.EventHandler(this._addressList_ItemDoubleClicked);
            // 
            // _phonePage
            // 
            this._phonePage.Controls.Add(this._phoneList);
            this._phonePage.Location = new System.Drawing.Point(4, 22);
            this._phonePage.Name = "_phonePage";
            this._phonePage.Padding = new System.Windows.Forms.Padding(3);
            this._phonePage.Size = new System.Drawing.Size(698, 298);
            this._phonePage.TabIndex = 1;
            this._phonePage.Text = "Phone Numbers";
            this._phonePage.UseVisualStyleBackColor = true;
            // 
            // _phoneList
            // 
            this._phoneList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._phoneList.Location = new System.Drawing.Point(3, 3);
            this._phoneList.MenuModel = null;
            this._phoneList.Name = "_phoneList";
            this._phoneList.ReadOnly = false;
            this._phoneList.Selection = selection2;
            this._phoneList.ShowToolbar = false;
            this._phoneList.Size = new System.Drawing.Size(692, 292);
            this._phoneList.TabIndex = 0;
            this._phoneList.Table = null;
            this._phoneList.ToolbarModel = null;
            this._phoneList.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._phoneList.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._phoneList.ItemDoubleClicked += new System.EventHandler(this._phoneList_ItemDoubleClicked);
            // 
            // _emailPage
            // 
            this._emailPage.Controls.Add(this._emailList);
            this._emailPage.Location = new System.Drawing.Point(4, 22);
            this._emailPage.Name = "_emailPage";
            this._emailPage.Padding = new System.Windows.Forms.Padding(3);
            this._emailPage.Size = new System.Drawing.Size(698, 298);
            this._emailPage.TabIndex = 2;
            this._emailPage.Text = "Email Addresses";
            this._emailPage.UseVisualStyleBackColor = true;
            // 
            // _emailList
            // 
            this._emailList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._emailList.Location = new System.Drawing.Point(3, 3);
            this._emailList.MenuModel = null;
            this._emailList.Name = "_emailList";
            this._emailList.ReadOnly = false;
            this._emailList.Selection = selection3;
            this._emailList.ShowToolbar = false;
            this._emailList.Size = new System.Drawing.Size(692, 292);
            this._emailList.TabIndex = 0;
            this._emailList.Table = null;
            this._emailList.ToolbarModel = null;
            this._emailList.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._emailList.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._emailList.ItemDoubleClicked += new System.EventHandler(this._emailList_ItemDoubleClicked);
            // 
            // _contactPage
            // 
            this._contactPage.Controls.Add(this._contactList);
            this._contactPage.Location = new System.Drawing.Point(4, 22);
            this._contactPage.Name = "_contactPage";
            this._contactPage.Padding = new System.Windows.Forms.Padding(3);
            this._contactPage.Size = new System.Drawing.Size(698, 298);
            this._contactPage.TabIndex = 3;
            this._contactPage.Text = "Contact Person";
            this._contactPage.UseVisualStyleBackColor = true;
            // 
            // _contactList
            // 
            this._contactList.Dock = System.Windows.Forms.DockStyle.Fill;
            this._contactList.Location = new System.Drawing.Point(3, 3);
            this._contactList.MenuModel = null;
            this._contactList.Name = "_contactList";
            this._contactList.ReadOnly = false;
            this._contactList.Selection = selection4;
            this._contactList.ShowToolbar = false;
            this._contactList.Size = new System.Drawing.Size(692, 292);
            this._contactList.TabIndex = 0;
            this._contactList.Table = null;
            this._contactList.ToolbarModel = null;
            this._contactList.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._contactList.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            this._contactList.ItemDoubleClicked += new System.EventHandler(this._contactList_ItemDoubleClicked);
            // 
            // _selectedProfile
            // 
            this._selectedProfile.DataSource = null;
            this._selectedProfile.DisplayMember = "";
            this._selectedProfile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._selectedProfile.LabelText = "Selected Profile";
            this._selectedProfile.Location = new System.Drawing.Point(13, 8);
            this._selectedProfile.Margin = new System.Windows.Forms.Padding(2);
            this._selectedProfile.Name = "_selectedProfile";
            this._selectedProfile.Size = new System.Drawing.Size(262, 41);
            this._selectedProfile.TabIndex = 3;
            this._selectedProfile.Value = null;
            // 
            // _patientIdentifierGroup
            // 
            this._patientIdentifierGroup.Controls.Add(this._healthcardVersionCode);
            this._patientIdentifierGroup.Controls.Add(this._mrnSite);
            this._patientIdentifierGroup.Controls.Add(this._healthcardProvince);
            this._patientIdentifierGroup.Controls.Add(this._mrn);
            this._patientIdentifierGroup.Controls.Add(this._healthcard);
            this._patientIdentifierGroup.Controls.Add(this._healthcardExpiry);
            this._patientIdentifierGroup.Location = new System.Drawing.Point(8, 66);
            this._patientIdentifierGroup.Name = "_patientIdentifierGroup";
            this._patientIdentifierGroup.Size = new System.Drawing.Size(275, 160);
            this._patientIdentifierGroup.TabIndex = 4;
            this._patientIdentifierGroup.TabStop = false;
            this._patientIdentifierGroup.Text = "Patient Identifier";
            // 
            // _healthcardVersionCode
            // 
            this._healthcardVersionCode.AutoSize = true;
            this._healthcardVersionCode.LabelText = "Version Code";
            this._healthcardVersionCode.Location = new System.Drawing.Point(5, 111);
            this._healthcardVersionCode.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
            this._healthcardVersionCode.Mask = "";
            this._healthcardVersionCode.Name = "_healthcardVersionCode";
            this._healthcardVersionCode.ReadOnly = true;
            this._healthcardVersionCode.Size = new System.Drawing.Size(127, 44);
            this._healthcardVersionCode.TabIndex = 4;
            this._healthcardVersionCode.Value = null;
            // 
            // _mrnSite
            // 
            this._mrnSite.LabelText = "Site";
            this._mrnSite.Location = new System.Drawing.Point(140, 17);
            this._mrnSite.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._mrnSite.Mask = "";
            this._mrnSite.Name = "_mrnSite";
            this._mrnSite.ReadOnly = true;
            this._mrnSite.Size = new System.Drawing.Size(127, 44);
            this._mrnSite.TabIndex = 1;
            this._mrnSite.Value = null;
            // 
            // _healthcardProvince
            // 
            this._healthcardProvince.LabelText = "Province";
            this._healthcardProvince.Location = new System.Drawing.Point(140, 65);
            this._healthcardProvince.Margin = new System.Windows.Forms.Padding(2, 2, 15, 2);
            this._healthcardProvince.Mask = "";
            this._healthcardProvince.Name = "_healthcardProvince";
            this._healthcardProvince.ReadOnly = true;
            this._healthcardProvince.Size = new System.Drawing.Size(127, 44);
            this._healthcardProvince.TabIndex = 3;
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
            this._mrn.TabIndex = 0;
            this._mrn.Value = null;
            // 
            // _healthcard
            // 
            this._healthcard.AutoSize = true;
            this._healthcard.LabelText = "Healthcard";
            this._healthcard.Location = new System.Drawing.Point(5, 65);
            this._healthcard.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
            this._healthcard.Mask = "";
            this._healthcard.Name = "_healthcard";
            this._healthcard.ReadOnly = true;
            this._healthcard.Size = new System.Drawing.Size(127, 44);
            this._healthcard.TabIndex = 2;
            this._healthcard.Value = null;
            // 
            // _healthcardExpiry
            // 
            this._healthcardExpiry.AutoSize = true;
            this._healthcardExpiry.LabelText = "Healthcard Expiry Date";
            this._healthcardExpiry.Location = new System.Drawing.Point(140, 111);
            this._healthcardExpiry.Margin = new System.Windows.Forms.Padding(2, 2, 17, 2);
            this._healthcardExpiry.Mask = "";
            this._healthcardExpiry.Name = "_healthcardExpiry";
            this._healthcardExpiry.ReadOnly = true;
            this._healthcardExpiry.Size = new System.Drawing.Size(127, 44);
            this._healthcardExpiry.TabIndex = 5;
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
            this._personalInfoGroup.Location = new System.Drawing.Point(289, 16);
            this._personalInfoGroup.Name = "_personalInfoGroup";
            this._personalInfoGroup.Size = new System.Drawing.Size(407, 210);
            this._personalInfoGroup.TabIndex = 5;
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
            this._primaryLanguage.Size = new System.Drawing.Size(127, 44);
            this._primaryLanguage.TabIndex = 10;
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
            this._religion.Size = new System.Drawing.Size(127, 44);
            this._religion.TabIndex = 9;
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
            this._degree.TabIndex = 5;
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
            this._prefix.TabIndex = 3;
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
            this._suffix.TabIndex = 4;
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
            this._middleName.TabIndex = 2;
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
            this._timeOfDeath.Size = new System.Drawing.Size(127, 44);
            this._timeOfDeath.TabIndex = 8;
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
            this._dateOfBirth.Size = new System.Drawing.Size(127, 44);
            this._dateOfBirth.TabIndex = 7;
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
            this._sex.Size = new System.Drawing.Size(127, 44);
            this._sex.TabIndex = 6;
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
            this._familyName.TabIndex = 0;
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
            this._givenName.TabIndex = 1;
            this._givenName.Value = null;
            // 
            // BiographyDemographicComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "BiographyDemographicComponentControl";
            this.Size = new System.Drawing.Size(706, 567);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this._addressPage.ResumeLayout(false);
            this._phonePage.ResumeLayout(false);
            this._emailPage.ResumeLayout(false);
            this._contactPage.ResumeLayout(false);
            this._patientIdentifierGroup.ResumeLayout(false);
            this._patientIdentifierGroup.PerformLayout();
            this._personalInfoGroup.ResumeLayout(false);
            this._personalInfoGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _selectedProfile;
        private System.Windows.Forms.GroupBox _patientIdentifierGroup;
        private ClearCanvas.Desktop.View.WinForms.TextField _healthcardVersionCode;
        private ClearCanvas.Desktop.View.WinForms.TextField _mrnSite;
        private ClearCanvas.Desktop.View.WinForms.TextField _healthcardProvince;
        private ClearCanvas.Desktop.View.WinForms.TextField _mrn;
        private ClearCanvas.Desktop.View.WinForms.TextField _healthcard;
        private ClearCanvas.Desktop.View.WinForms.TextField _healthcardExpiry;
        private System.Windows.Forms.GroupBox _personalInfoGroup;
        private ClearCanvas.Desktop.View.WinForms.TextField _primaryLanguage;
        private ClearCanvas.Desktop.View.WinForms.TextField _religion;
        private ClearCanvas.Desktop.View.WinForms.TextField _degree;
        private ClearCanvas.Desktop.View.WinForms.TextField _prefix;
        private ClearCanvas.Desktop.View.WinForms.TextField _suffix;
        private ClearCanvas.Desktop.View.WinForms.TextField _middleName;
        private ClearCanvas.Desktop.View.WinForms.TextField _timeOfDeath;
        private ClearCanvas.Desktop.View.WinForms.TextField _dateOfBirth;
        private ClearCanvas.Desktop.View.WinForms.TextField _sex;
        private ClearCanvas.Desktop.View.WinForms.TextField _familyName;
        private ClearCanvas.Desktop.View.WinForms.TextField _givenName;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage _addressPage;
        private ClearCanvas.Desktop.View.WinForms.TableView _addressList;
        private System.Windows.Forms.TabPage _phonePage;
        private ClearCanvas.Desktop.View.WinForms.TableView _phoneList;
        private System.Windows.Forms.TabPage _emailPage;
        private ClearCanvas.Desktop.View.WinForms.TableView _emailList;
        private System.Windows.Forms.TabPage _contactPage;
        private ClearCanvas.Desktop.View.WinForms.TableView _contactList;


    }
}
