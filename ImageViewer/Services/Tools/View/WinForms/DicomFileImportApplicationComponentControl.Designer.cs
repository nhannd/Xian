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
    partial class DicomFileImportApplicationComponentControl
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
			this._importTable = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._importProgressControl = new ClearCanvas.ImageViewer.Services.Tools.View.WinForms.ImportProgressControl();
			this.SuspendLayout();
			// 
			// _importTable
			// 
			this._importTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._importTable.Location = new System.Drawing.Point(0, 0);
			this._importTable.MultiSelect = false;
			this._importTable.Name = "_importTable";
			this._importTable.ReadOnly = false;
			this._importTable.Size = new System.Drawing.Size(776, 228);
			this._importTable.TabIndex = 0;
			// 
			// _importProgressControl
			// 
			this._importProgressControl.AcceptButton = null;
			this._importProgressControl.AvailableCount = 0;
			this._importProgressControl.CancelButton = null;
			this._importProgressControl.ButtonEnabled = true;
			this._importProgressControl.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._importProgressControl.FailedSteps = 0;
			this._importProgressControl.Location = new System.Drawing.Point(0, 228);
			this._importProgressControl.Name = "_importProgressControl";
			this._importProgressControl.Size = new System.Drawing.Size(776, 165);
			this._importProgressControl.StatusMessage = "Status:";
			this._importProgressControl.TabIndex = 1;
			this._importProgressControl.TotalProcessed = 0;
			this._importProgressControl.TotalToProcess = 100;
			// 
			// DicomFileImportApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._importTable);
			this.Controls.Add(this._importProgressControl);
			this.Name = "DicomFileImportApplicationComponentControl";
			this.Size = new System.Drawing.Size(776, 393);
			this.ResumeLayout(false);

        }

        #endregion

		private ImportProgressControl _importProgressControl;
		private ClearCanvas.Desktop.View.WinForms.TableView _importTable;



	}
}
