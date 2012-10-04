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
    partial class ExternalPractitionerContactPointDetailsEditorComponentControl
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
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._description = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._isDefaultContactPoint = new System.Windows.Forms.CheckBox();
			this._resultCommunicationMode = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.SuspendLayout();
			// 
			// _name
			// 
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(32, 20);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.PasswordChar = '\0';
			this._name.Size = new System.Drawing.Size(150, 41);
			this._name.TabIndex = 0;
			this._name.ToolTip = null;
			this._name.Value = null;
			// 
			// _description
			// 
			this._description.LabelText = "Description";
			this._description.Location = new System.Drawing.Point(32, 89);
			this._description.Margin = new System.Windows.Forms.Padding(2);
			this._description.Mask = "";
			this._description.Name = "_description";
			this._description.PasswordChar = '\0';
			this._description.Size = new System.Drawing.Size(357, 41);
			this._description.TabIndex = 2;
			this._description.ToolTip = null;
			this._description.Value = null;
			// 
			// _isDefaultContactPoint
			// 
			this._isDefaultContactPoint.AutoSize = true;
			this._isDefaultContactPoint.Location = new System.Drawing.Point(262, 40);
			this._isDefaultContactPoint.Name = "_isDefaultContactPoint";
			this._isDefaultContactPoint.Size = new System.Drawing.Size(127, 17);
			this._isDefaultContactPoint.TabIndex = 1;
			this._isDefaultContactPoint.Text = "Default Contact Point";
			this._isDefaultContactPoint.UseVisualStyleBackColor = true;
			// 
			// _resultCommunicationMode
			// 
			this._resultCommunicationMode.DataSource = null;
			this._resultCommunicationMode.DisplayMember = "";
			this._resultCommunicationMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._resultCommunicationMode.LabelText = "Preferred means of result communication";
			this._resultCommunicationMode.Location = new System.Drawing.Point(32, 153);
			this._resultCommunicationMode.Margin = new System.Windows.Forms.Padding(2);
			this._resultCommunicationMode.Name = "_resultCommunicationMode";
			this._resultCommunicationMode.Size = new System.Drawing.Size(224, 41);
			this._resultCommunicationMode.TabIndex = 3;
			this._resultCommunicationMode.Value = null;
			// 
			// ExternalPractitionerContactPointDetailsEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._resultCommunicationMode);
			this.Controls.Add(this._isDefaultContactPoint);
			this.Controls.Add(this._description);
			this.Controls.Add(this._name);
			this.Name = "ExternalPractitionerContactPointDetailsEditorComponentControl";
			this.Size = new System.Drawing.Size(425, 225);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TextField _name;
        private ClearCanvas.Desktop.View.WinForms.TextField _description;
        private System.Windows.Forms.CheckBox _isDefaultContactPoint;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _resultCommunicationMode;
    }
}
