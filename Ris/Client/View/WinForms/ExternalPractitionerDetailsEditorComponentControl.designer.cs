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
            this.SuspendLayout();
            // 
            // _licenseNumber
            // 
            this._licenseNumber.LabelText = "License Number";
            this._licenseNumber.Location = new System.Drawing.Point(239, 95);
            this._licenseNumber.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._licenseNumber.Mask = "";
            this._licenseNumber.Name = "_licenseNumber";
            this._licenseNumber.Size = new System.Drawing.Size(200, 50);
            this._licenseNumber.TabIndex = 4;
            this._licenseNumber.Value = null;
            // 
            // _middleName
            // 
            this._middleName.LabelText = "Middle Name";
            this._middleName.Location = new System.Drawing.Point(462, 25);
            this._middleName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._middleName.Mask = "";
            this._middleName.Name = "_middleName";
            this._middleName.Size = new System.Drawing.Size(200, 50);
            this._middleName.TabIndex = 2;
            this._middleName.Value = null;
            // 
            // _givenName
            // 
            this._givenName.LabelText = "Given Name";
            this._givenName.Location = new System.Drawing.Point(239, 25);
            this._givenName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._givenName.Mask = "";
            this._givenName.Name = "_givenName";
            this._givenName.Size = new System.Drawing.Size(200, 50);
            this._givenName.TabIndex = 1;
            this._givenName.Value = null;
            // 
            // _familyName
            // 
            this._familyName.LabelText = "Family Name";
            this._familyName.Location = new System.Drawing.Point(17, 25);
            this._familyName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._familyName.Mask = "";
            this._familyName.Name = "_familyName";
            this._familyName.Size = new System.Drawing.Size(200, 50);
            this._familyName.TabIndex = 0;
            this._familyName.Value = null;
            // 
            // ExternalPractitionerDetailsEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._middleName);
            this.Controls.Add(this._givenName);
            this.Controls.Add(this._familyName);
            this.Controls.Add(this._licenseNumber);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "ExternalPractitionerDetailsEditorComponentControl";
            this.Size = new System.Drawing.Size(671, 181);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TextField _licenseNumber;
        private ClearCanvas.Desktop.View.WinForms.TextField _middleName;
        private ClearCanvas.Desktop.View.WinForms.TextField _givenName;
        private ClearCanvas.Desktop.View.WinForms.TextField _familyName;

    }
}
