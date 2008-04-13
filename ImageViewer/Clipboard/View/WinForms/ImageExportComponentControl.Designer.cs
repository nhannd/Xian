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

namespace ClearCanvas.ImageViewer.Clipboard.View.WinForms
{
    partial class ImageExportComponentControl
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
			this._imageExporters = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._path = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._buttonBrowse = new System.Windows.Forms.Button();
			this._buttonConfigure = new System.Windows.Forms.Button();
			this._buttonOk = new System.Windows.Forms.Button();
			this._buttonCancel = new System.Windows.Forms.Button();
			this._groupOptions = new System.Windows.Forms.GroupBox();
			this._checkOptionWysiwyg = new System.Windows.Forms.RadioButton();
			this._checkOptionCompleteImage = new System.Windows.Forms.RadioButton();
			this._groupOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// _imageExporters
			// 
			this._imageExporters.DataSource = null;
			this._imageExporters.DisplayMember = "";
			this._imageExporters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._imageExporters.LabelText = "Export As";
			this._imageExporters.Location = new System.Drawing.Point(10, 74);
			this._imageExporters.Margin = new System.Windows.Forms.Padding(2);
			this._imageExporters.Name = "_imageExporters";
			this._imageExporters.Size = new System.Drawing.Size(315, 41);
			this._imageExporters.TabIndex = 2;
			this._imageExporters.Value = null;
			// 
			// _path
			// 
			this._path.LabelText = "Path";
			this._path.Location = new System.Drawing.Point(10, 15);
			this._path.Margin = new System.Windows.Forms.Padding(2);
			this._path.Mask = "";
			this._path.Name = "_path";
			this._path.PasswordChar = '\0';
			this._path.Size = new System.Drawing.Size(315, 41);
			this._path.TabIndex = 0;
			this._path.ToolTip = null;
			this._path.Value = null;
			// 
			// _buttonBrowse
			// 
			this._buttonBrowse.Location = new System.Drawing.Point(330, 31);
			this._buttonBrowse.Name = "_buttonBrowse";
			this._buttonBrowse.Size = new System.Drawing.Size(70, 23);
			this._buttonBrowse.TabIndex = 1;
			this._buttonBrowse.Text = "Browse";
			this._buttonBrowse.UseVisualStyleBackColor = true;
			this._buttonBrowse.Click += new System.EventHandler(this.OnBrowse);
			// 
			// _buttonConfigure
			// 
			this._buttonConfigure.Location = new System.Drawing.Point(330, 91);
			this._buttonConfigure.Name = "_buttonConfigure";
			this._buttonConfigure.Size = new System.Drawing.Size(70, 23);
			this._buttonConfigure.TabIndex = 3;
			this._buttonConfigure.Text = "Configure";
			this._buttonConfigure.UseVisualStyleBackColor = true;
			this._buttonConfigure.Click += new System.EventHandler(this.OnConfigureExporter);
			// 
			// _buttonOk
			// 
			this._buttonOk.Location = new System.Drawing.Point(254, 150);
			this._buttonOk.Name = "_buttonOk";
			this._buttonOk.Size = new System.Drawing.Size(70, 23);
			this._buttonOk.TabIndex = 5;
			this._buttonOk.Text = "Ok";
			this._buttonOk.UseVisualStyleBackColor = true;
			this._buttonOk.Click += new System.EventHandler(this.OnOk);
			// 
			// _buttonCancel
			// 
			this._buttonCancel.Location = new System.Drawing.Point(330, 150);
			this._buttonCancel.Name = "_buttonCancel";
			this._buttonCancel.Size = new System.Drawing.Size(70, 23);
			this._buttonCancel.TabIndex = 6;
			this._buttonCancel.Text = "Cancel";
			this._buttonCancel.UseVisualStyleBackColor = true;
			this._buttonCancel.Click += new System.EventHandler(this.OnCancel);
			// 
			// _groupOptions
			// 
			this._groupOptions.Controls.Add(this._checkOptionCompleteImage);
			this._groupOptions.Controls.Add(this._checkOptionWysiwyg);
			this._groupOptions.Location = new System.Drawing.Point(10, 133);
			this._groupOptions.Name = "_groupOptions";
			this._groupOptions.Size = new System.Drawing.Size(210, 52);
			this._groupOptions.TabIndex = 4;
			this._groupOptions.TabStop = false;
			this._groupOptions.Text = "Options";
			// 
			// _checkOptionWysiwyg
			// 
			this._checkOptionWysiwyg.AutoSize = true;
			this._checkOptionWysiwyg.Location = new System.Drawing.Point(10, 20);
			this._checkOptionWysiwyg.Name = "_checkOptionWysiwyg";
			this._checkOptionWysiwyg.Size = new System.Drawing.Size(67, 17);
			this._checkOptionWysiwyg.TabIndex = 0;
			this._checkOptionWysiwyg.Text = "Wysiwyg";
			this._checkOptionWysiwyg.UseVisualStyleBackColor = true;
			// 
			// _checkOptionCompleteImage
			// 
			this._checkOptionCompleteImage.AutoSize = true;
			this._checkOptionCompleteImage.Location = new System.Drawing.Point(95, 19);
			this._checkOptionCompleteImage.Name = "_checkOptionCompleteImage";
			this._checkOptionCompleteImage.Size = new System.Drawing.Size(101, 17);
			this._checkOptionCompleteImage.TabIndex = 1;
			this._checkOptionCompleteImage.Text = "Complete Image";
			this._checkOptionCompleteImage.UseVisualStyleBackColor = true;
			// 
			// ImageExportComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._groupOptions);
			this.Controls.Add(this._buttonCancel);
			this.Controls.Add(this._buttonOk);
			this.Controls.Add(this._buttonConfigure);
			this.Controls.Add(this._buttonBrowse);
			this.Controls.Add(this._path);
			this.Controls.Add(this._imageExporters);
			this.Name = "ImageExportComponentControl";
			this.Size = new System.Drawing.Size(414, 203);
			this._groupOptions.ResumeLayout(false);
			this._groupOptions.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _imageExporters;
		private ClearCanvas.Desktop.View.WinForms.TextField _path;
		private System.Windows.Forms.Button _buttonBrowse;
		private System.Windows.Forms.Button _buttonConfigure;
		private System.Windows.Forms.Button _buttonOk;
		private System.Windows.Forms.Button _buttonCancel;
		private System.Windows.Forms.GroupBox _groupOptions;
		private System.Windows.Forms.RadioButton _checkOptionCompleteImage;
		private System.Windows.Forms.RadioButton _checkOptionWysiwyg;
    }
}
