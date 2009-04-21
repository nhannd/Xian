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
    partial class WorklistDetailEditorComponentControl
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
			this._description = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._classDescription = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._category = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._worklistClass = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this.SuspendLayout();
			// 
			// _name
			// 
			this._name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(20, 141);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.PasswordChar = '\0';
			this._name.Size = new System.Drawing.Size(492, 41);
			this._name.TabIndex = 2;
			this._name.ToolTip = null;
			this._name.Value = null;
			// 
			// _description
			// 
			this._description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._description.LabelText = "Description";
			this._description.Location = new System.Drawing.Point(20, 186);
			this._description.Margin = new System.Windows.Forms.Padding(2);
			this._description.Name = "_description";
			this._description.Size = new System.Drawing.Size(492, 66);
			this._description.TabIndex = 3;
			this._description.Value = null;
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(347, 267);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 4;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(428, 267);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 5;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _classDescription
			// 
			this._classDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._classDescription.LabelText = "Class Description";
			this._classDescription.Location = new System.Drawing.Point(20, 96);
			this._classDescription.Margin = new System.Windows.Forms.Padding(2);
			this._classDescription.Mask = "";
			this._classDescription.Name = "_classDescription";
			this._classDescription.PasswordChar = '\0';
			this._classDescription.ReadOnly = true;
			this._classDescription.Size = new System.Drawing.Size(492, 41);
			this._classDescription.TabIndex = 1;
			this._classDescription.ToolTip = null;
			this._classDescription.Value = null;
			// 
			// _category
			// 
			this._category.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._category.DataSource = null;
			this._category.DisplayMember = "";
			this._category.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._category.LabelText = "Category";
			this._category.Location = new System.Drawing.Point(20, 6);
			this._category.Margin = new System.Windows.Forms.Padding(2);
			this._category.Name = "_category";
			this._category.Size = new System.Drawing.Size(492, 41);
			this._category.TabIndex = 6;
			this._category.Value = null;
			// 
			// _worklistClass
			// 
			this._worklistClass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._worklistClass.DataSource = null;
			this._worklistClass.DisplayMember = "";
			this._worklistClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._worklistClass.LabelText = "Class";
			this._worklistClass.Location = new System.Drawing.Point(20, 51);
			this._worklistClass.Margin = new System.Windows.Forms.Padding(2);
			this._worklistClass.Name = "_worklistClass";
			this._worklistClass.Size = new System.Drawing.Size(492, 41);
			this._worklistClass.TabIndex = 7;
			this._worklistClass.Value = null;
			// 
			// WorklistDetailEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._worklistClass);
			this.Controls.Add(this._category);
			this.Controls.Add(this._classDescription);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._description);
			this.Controls.Add(this._name);
			this.Name = "WorklistDetailEditorComponentControl";
			this.Size = new System.Drawing.Size(543, 307);
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TextField _name;
		private ClearCanvas.Desktop.View.WinForms.TextAreaField _description;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private ClearCanvas.Desktop.View.WinForms.TextField _classDescription;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _category;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _worklistClass;

    }
}
