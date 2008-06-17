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

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class ProcedureTypeEditorComponentControl
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
			this.components = new System.ComponentModel.Container();
			this.baseTypeListBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this._acceptButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this.procedureTypeEditorComponentBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this._baseType = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._id = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._name = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._planXml = new ClearCanvas.Desktop.View.WinForms.TextField();
			((System.ComponentModel.ISupportInitialize)(this.baseTypeListBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.procedureTypeEditorComponentBindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// baseTypeListBindingSource
			// 
			this.baseTypeListBindingSource.DataMember = "BaseTypeList";
			this.baseTypeListBindingSource.DataSource = this.procedureTypeEditorComponentBindingSource;
			// 
			// _acceptButton
			// 
			this._acceptButton.Location = new System.Drawing.Point(165, 222);
			this._acceptButton.Name = "_acceptButton";
			this._acceptButton.Size = new System.Drawing.Size(75, 23);
			this._acceptButton.TabIndex = 8;
			this._acceptButton.Text = "Accept";
			this._acceptButton.UseVisualStyleBackColor = true;
			this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(246, 222);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 9;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// procedureTypeEditorComponentBindingSource
			// 
			this.procedureTypeEditorComponentBindingSource.DataSource = typeof(ClearCanvas.Ris.Client.Admin.ProcedureTypeEditorComponent);
			// 
			// _baseType
			// 
			this._baseType.DataSource = null;
			this._baseType.DisplayMember = "";
			this._baseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._baseType.LabelText = "Base Type";
			this._baseType.Location = new System.Drawing.Point(11, 108);
			this._baseType.Margin = new System.Windows.Forms.Padding(2);
			this._baseType.Name = "_baseType";
			this._baseType.Size = new System.Drawing.Size(282, 41);
			this._baseType.TabIndex = 10;
			this._baseType.Value = null;
			// 
			// _id
			// 
			this._id.LabelText = "ID";
			this._id.Location = new System.Drawing.Point(11, 18);
			this._id.Margin = new System.Windows.Forms.Padding(2);
			this._id.Mask = "";
			this._id.Name = "_id";
			this._id.PasswordChar = '\0';
			this._id.Size = new System.Drawing.Size(282, 41);
			this._id.TabIndex = 11;
			this._id.ToolTip = null;
			this._id.Value = null;
			// 
			// _name
			// 
			this._name.LabelText = "Name";
			this._name.Location = new System.Drawing.Point(11, 63);
			this._name.Margin = new System.Windows.Forms.Padding(2);
			this._name.Mask = "";
			this._name.Name = "_name";
			this._name.PasswordChar = '\0';
			this._name.Size = new System.Drawing.Size(282, 41);
			this._name.TabIndex = 12;
			this._name.ToolTip = null;
			this._name.Value = null;
			// 
			// _planXml
			// 
			this._planXml.LabelText = "Plan XML Document";
			this._planXml.Location = new System.Drawing.Point(11, 153);
			this._planXml.Margin = new System.Windows.Forms.Padding(2);
			this._planXml.Mask = "";
			this._planXml.Name = "_planXml";
			this._planXml.PasswordChar = '\0';
			this._planXml.Size = new System.Drawing.Size(282, 41);
			this._planXml.TabIndex = 13;
			this._planXml.ToolTip = null;
			this._planXml.Value = null;
			// 
			// ProcedureTypeEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._planXml);
			this.Controls.Add(this._name);
			this.Controls.Add(this._id);
			this.Controls.Add(this._baseType);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._acceptButton);
			this.Name = "ProcedureTypeEditorComponentControl";
			this.Size = new System.Drawing.Size(324, 248);
			((System.ComponentModel.ISupportInitialize)(this.baseTypeListBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.procedureTypeEditorComponentBindingSource)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Button _acceptButton;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.BindingSource procedureTypeEditorComponentBindingSource;
		private System.Windows.Forms.BindingSource baseTypeListBindingSource;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _baseType;
		private ClearCanvas.Desktop.View.WinForms.TextField _id;
		private ClearCanvas.Desktop.View.WinForms.TextField _name;
		private ClearCanvas.Desktop.View.WinForms.TextField _planXml;
    }
}
