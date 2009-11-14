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

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    partial class DicomServerConfigurationComponentControl
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
			this._aeTitle = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._port = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._refreshButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _aeTitle
			// 
			this._aeTitle.LabelText = "AE Title";
			this._aeTitle.Location = new System.Drawing.Point(2, 2);
			this._aeTitle.Margin = new System.Windows.Forms.Padding(2);
			this._aeTitle.Mask = "";
			this._aeTitle.Name = "_aeTitle";
			this._aeTitle.Size = new System.Drawing.Size(150, 41);
			this._aeTitle.TabIndex = 1;
			this._aeTitle.Value = null;
			// 
			// _port
			// 
			this._port.LabelText = "Port";
			this._port.Location = new System.Drawing.Point(2, 49);
			this._port.Margin = new System.Windows.Forms.Padding(2);
			this._port.Mask = "";
			this._port.Name = "_port";
			this._port.Size = new System.Drawing.Size(150, 41);
			this._port.TabIndex = 2;
			this._port.Value = null;
			// 
			// _refreshButton
			// 
			this._refreshButton.Location = new System.Drawing.Point(4, 95);
			this._refreshButton.Name = "_refreshButton";
			this._refreshButton.Size = new System.Drawing.Size(75, 23);
			this._refreshButton.TabIndex = 5;
			this._refreshButton.Text = "Refresh";
			this._refreshButton.UseVisualStyleBackColor = true;
			this._refreshButton.Click += new System.EventHandler(this._refreshButton_Click);
			// 
			// DicomServerConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._refreshButton);
			this.Controls.Add(this._port);
			this.Controls.Add(this._aeTitle);
			this.Name = "DicomServerConfigurationComponentControl";
			this.Size = new System.Drawing.Size(158, 137);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TextField _aeTitle;
		private ClearCanvas.Desktop.View.WinForms.TextField _port;
        private System.Windows.Forms.Button _refreshButton;
    }
}
