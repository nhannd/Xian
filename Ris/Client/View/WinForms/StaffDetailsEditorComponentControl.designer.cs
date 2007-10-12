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
    partial class StaffDetailsEditorComponentControl
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
            this._staffType = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._middleName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._givenName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._familyName = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._staffId = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.SuspendLayout();
            // 
            // _staffType
            // 
            this._staffType.DataSource = null;
            this._staffType.DisplayMember = "";
            this._staffType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._staffType.LabelText = "StaffType";
            this._staffType.Location = new System.Drawing.Point(17, 101);
            this._staffType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._staffType.Name = "_staffType";
            this._staffType.Size = new System.Drawing.Size(200, 50);
            this._staffType.TabIndex = 3;
            this._staffType.Value = null;
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
            // _staffId
            // 
            this._staffId.LabelText = "Staff ID";
            this._staffId.Location = new System.Drawing.Point(239, 101);
            this._staffId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._staffId.Mask = "";
            this._staffId.Name = "_staffId";
            this._staffId.Size = new System.Drawing.Size(200, 50);
            this._staffId.TabIndex = 4;
            this._staffId.Value = null;
            // 
            // StaffDetailsEditorComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._staffId);
            this.Controls.Add(this._middleName);
            this.Controls.Add(this._givenName);
            this.Controls.Add(this._familyName);
            this.Controls.Add(this._staffType);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "StaffDetailsEditorComponentControl";
            this.Size = new System.Drawing.Size(671, 181);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _staffType;
        private ClearCanvas.Desktop.View.WinForms.TextField _middleName;
        private ClearCanvas.Desktop.View.WinForms.TextField _givenName;
        private ClearCanvas.Desktop.View.WinForms.TextField _familyName;
        private ClearCanvas.Desktop.View.WinForms.TextField _staffId;

    }
}
