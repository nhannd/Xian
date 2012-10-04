#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class ExternalPractitionerDetailsEditorComponentControl
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
			this._licenseNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._middleName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._givenName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._familyName = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._billingNumber = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._isVerified = new System.Windows.Forms.CheckBox();
			this._lastVerified = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _licenseNumber
			// 
			this._licenseNumber.LabelText = "License Number";
			this._licenseNumber.Location = new System.Drawing.Point(13, 129);
			this._licenseNumber.Margin = new System.Windows.Forms.Padding(2);
			this._licenseNumber.Mask = "";
			this._licenseNumber.Name = "_licenseNumber";
			this._licenseNumber.PasswordChar = '\0';
			this._licenseNumber.Size = new System.Drawing.Size(150, 41);
			this._licenseNumber.TabIndex = 3;
			this._licenseNumber.ToolTip = null;
			this._licenseNumber.Value = null;
			// 
			// _middleName
			// 
			this._middleName.LabelText = "Middle Name";
			this._middleName.Location = new System.Drawing.Point(13, 75);
			this._middleName.Margin = new System.Windows.Forms.Padding(2);
			this._middleName.Mask = "";
			this._middleName.Name = "_middleName";
			this._middleName.PasswordChar = '\0';
			this._middleName.Size = new System.Drawing.Size(150, 41);
			this._middleName.TabIndex = 2;
			this._middleName.ToolTip = null;
			this._middleName.Value = null;
			// 
			// _givenName
			// 
			this._givenName.LabelText = "Given Name";
			this._givenName.Location = new System.Drawing.Point(179, 20);
			this._givenName.Margin = new System.Windows.Forms.Padding(2);
			this._givenName.Mask = "";
			this._givenName.Name = "_givenName";
			this._givenName.PasswordChar = '\0';
			this._givenName.Size = new System.Drawing.Size(150, 41);
			this._givenName.TabIndex = 1;
			this._givenName.ToolTip = null;
			this._givenName.Value = null;
			// 
			// _familyName
			// 
			this._familyName.LabelText = "Family Name";
			this._familyName.Location = new System.Drawing.Point(13, 20);
			this._familyName.Margin = new System.Windows.Forms.Padding(2);
			this._familyName.Mask = "";
			this._familyName.Name = "_familyName";
			this._familyName.PasswordChar = '\0';
			this._familyName.Size = new System.Drawing.Size(150, 41);
			this._familyName.TabIndex = 0;
			this._familyName.ToolTip = null;
			this._familyName.Value = null;
			// 
			// _billingNumber
			// 
			this._billingNumber.LabelText = "Billing Number";
			this._billingNumber.Location = new System.Drawing.Point(179, 129);
			this._billingNumber.Margin = new System.Windows.Forms.Padding(2);
			this._billingNumber.Mask = "";
			this._billingNumber.Name = "_billingNumber";
			this._billingNumber.PasswordChar = '\0';
			this._billingNumber.Size = new System.Drawing.Size(150, 41);
			this._billingNumber.TabIndex = 4;
			this._billingNumber.ToolTip = null;
			this._billingNumber.Value = null;
			// 
			// _isVerified
			// 
			this._isVerified.AutoSize = true;
			this._isVerified.Location = new System.Drawing.Point(17, 201);
			this._isVerified.Name = "_isVerified";
			this._isVerified.Size = new System.Drawing.Size(61, 17);
			this._isVerified.TabIndex = 6;
			this._isVerified.Text = "Verified";
			this._isVerified.UseVisualStyleBackColor = true;
			// 
			// _lastVerified
			// 
			this._lastVerified.AutoSize = true;
			this._lastVerified.Location = new System.Drawing.Point(85, 203);
			this._lastVerified.Name = "_lastVerified";
			this._lastVerified.Size = new System.Drawing.Size(64, 13);
			this._lastVerified.TabIndex = 7;
			this._lastVerified.Text = "Last verified";
			// 
			// ExternalPractitionerDetailsEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._lastVerified);
			this.Controls.Add(this._isVerified);
			this.Controls.Add(this._billingNumber);
			this.Controls.Add(this._middleName);
			this.Controls.Add(this._givenName);
			this.Controls.Add(this._familyName);
			this.Controls.Add(this._licenseNumber);
			this.Name = "ExternalPractitionerDetailsEditorComponentControl";
			this.Size = new System.Drawing.Size(360, 237);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TextField _licenseNumber;
        private ClearCanvas.Desktop.View.WinForms.TextField _middleName;
        private ClearCanvas.Desktop.View.WinForms.TextField _givenName;
        private ClearCanvas.Desktop.View.WinForms.TextField _familyName;
		private ClearCanvas.Desktop.View.WinForms.TextField _billingNumber;
		private System.Windows.Forms.CheckBox _isVerified;
		private System.Windows.Forms.Label _lastVerified;

    }
}
