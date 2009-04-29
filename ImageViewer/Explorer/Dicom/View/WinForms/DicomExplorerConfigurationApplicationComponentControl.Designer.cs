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

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    partial class DicomExplorerConfigurationApplicationComponentControl
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
			this._showPhoneticIdeographicNames = new System.Windows.Forms.CheckBox();
			this._showNumberOfImages = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _showPhoneticIdeographicNames
			// 
			this._showPhoneticIdeographicNames.AutoSize = true;
			this._showPhoneticIdeographicNames.Location = new System.Drawing.Point(12, 46);
			this._showPhoneticIdeographicNames.Name = "_showPhoneticIdeographicNames";
			this._showPhoneticIdeographicNames.Size = new System.Drawing.Size(210, 17);
			this._showPhoneticIdeographicNames.TabIndex = 1;
			this._showPhoneticIdeographicNames.Text = "Show phonetic and ideographic names";
			this._showPhoneticIdeographicNames.UseVisualStyleBackColor = true;
			// 
			// _showNumberOfImages
			// 
			this._showNumberOfImages.AutoSize = true;
			this._showNumberOfImages.Location = new System.Drawing.Point(12, 13);
			this._showNumberOfImages.Name = "_showNumberOfImages";
			this._showNumberOfImages.Size = new System.Drawing.Size(178, 17);
			this._showNumberOfImages.TabIndex = 0;
			this._showNumberOfImages.Text = "Show number of images in study";
			this._showNumberOfImages.UseVisualStyleBackColor = true;
			// 
			// DicomExplorerConfigurationApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._showNumberOfImages);
			this.Controls.Add(this._showPhoneticIdeographicNames);
			this.Name = "DicomExplorerConfigurationApplicationComponentControl";
			this.Size = new System.Drawing.Size(329, 210);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.CheckBox _showPhoneticIdeographicNames;
		private System.Windows.Forms.CheckBox _showNumberOfImages;
    }
}
