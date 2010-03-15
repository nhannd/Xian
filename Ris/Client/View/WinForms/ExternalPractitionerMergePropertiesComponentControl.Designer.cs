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
    partial class ExternalPractitionerMergePropertiesComponentControl
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
			this._name = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._billingNumber = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._licenseNumber = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.SuspendLayout();
			// 
			// _name
			// 
			this._name.DataSource = null;
			this._name.DisplayMember = "";
			this._name.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(2, 2);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Name = "_name";
			this._name.Size = new System.Drawing.Size(182, 41);
			this._name.TabIndex = 1;
			this._name.Value = null;
			// 
			// _billingNumber
			// 
			this._billingNumber.DataSource = null;
			this._billingNumber.DisplayMember = "";
			this._billingNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._billingNumber.LabelText = "Billing #";
			this._billingNumber.Location = new System.Drawing.Point(2, 92);
			this._billingNumber.Margin = new System.Windows.Forms.Padding(2);
			this._billingNumber.Name = "_billingNumber";
			this._billingNumber.Size = new System.Drawing.Size(182, 41);
			this._billingNumber.TabIndex = 2;
			this._billingNumber.Value = null;
			// 
			// _licenseNumber
			// 
			this._licenseNumber.DataSource = null;
			this._licenseNumber.DisplayMember = "";
			this._licenseNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._licenseNumber.LabelText = "License #";
			this._licenseNumber.Location = new System.Drawing.Point(2, 47);
			this._licenseNumber.Margin = new System.Windows.Forms.Padding(2);
			this._licenseNumber.Name = "_licenseNumber";
			this._licenseNumber.Size = new System.Drawing.Size(182, 41);
			this._licenseNumber.TabIndex = 3;
			this._licenseNumber.Value = null;
			// 
			// ExternalPractitionerMergePropertiesComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._licenseNumber);
			this.Controls.Add(this._billingNumber);
			this.Controls.Add(this._name);
			this.Name = "ExternalPractitionerMergePropertiesComponentControl";
			this.Size = new System.Drawing.Size(274, 324);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _name;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _billingNumber;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _licenseNumber;
    }
}
