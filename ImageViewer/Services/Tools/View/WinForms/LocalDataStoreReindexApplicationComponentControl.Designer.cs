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
    partial class LocalDataStoreReindexApplicationComponentControl
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
			this._reindexProgressControl = new ClearCanvas.ImageViewer.Services.Tools.View.WinForms.ImportProgressControl();
			this.SuspendLayout();
			// 
			// _reindexProgressControl
			// 
			this._reindexProgressControl.AcceptButton = null;
			this._reindexProgressControl.AutoSize = true;
			this._reindexProgressControl.AvailableCount = 0;
			this._reindexProgressControl.CancelButton = null;
			this._reindexProgressControl.CancelEnabled = true;
			this._reindexProgressControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._reindexProgressControl.FailedSteps = 0;
			this._reindexProgressControl.Location = new System.Drawing.Point(0, 0);
			this._reindexProgressControl.Name = "_reindexProgressControl";
			this._reindexProgressControl.Size = new System.Drawing.Size(777, 188);
			this._reindexProgressControl.StatusMessage = "Status:";
			this._reindexProgressControl.TabIndex = 10;
			this._reindexProgressControl.TotalProcessed = 0;
			this._reindexProgressControl.TotalToProcess = 100;
			// 
			// LocalDataStoreReindexApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._reindexProgressControl);
			this.Name = "LocalDataStoreReindexApplicationComponentControl";
			this.Size = new System.Drawing.Size(777, 188);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private ImportProgressControl _reindexProgressControl;


	}
}
