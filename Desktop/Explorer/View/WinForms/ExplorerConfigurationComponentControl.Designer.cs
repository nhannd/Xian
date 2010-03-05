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

namespace ClearCanvas.Desktop.Explorer.View.WinForms
{
    partial class ExplorerConfigurationComponentControl
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
			this._launchAsGroupBox = new System.Windows.Forms.GroupBox();
			this._launchAsShelf = new System.Windows.Forms.RadioButton();
			this._launchAsWorkspace = new System.Windows.Forms.RadioButton();
			this._launchAtStartup = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this._launchAsGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// _launchAsGroupBox
			// 
			this._launchAsGroupBox.Controls.Add(this._launchAsShelf);
			this._launchAsGroupBox.Controls.Add(this._launchAsWorkspace);
			this._launchAsGroupBox.Location = new System.Drawing.Point(12, 35);
			this._launchAsGroupBox.Name = "_launchAsGroupBox";
			this._launchAsGroupBox.Size = new System.Drawing.Size(133, 74);
			this._launchAsGroupBox.TabIndex = 1;
			this._launchAsGroupBox.TabStop = false;
			this._launchAsGroupBox.Text = "Launch as:";
			// 
			// _launchAsShelf
			// 
			this._launchAsShelf.AutoSize = true;
			this._launchAsShelf.Location = new System.Drawing.Point(13, 45);
			this._launchAsShelf.Name = "_launchAsShelf";
			this._launchAsShelf.Size = new System.Drawing.Size(107, 17);
			this._launchAsShelf.TabIndex = 1;
			this._launchAsShelf.Text = "Docking Window";
			this._launchAsShelf.UseVisualStyleBackColor = true;
			// 
			// _launchAsWorkspace
			// 
			this._launchAsWorkspace.AutoSize = true;
			this._launchAsWorkspace.Location = new System.Drawing.Point(13, 22);
			this._launchAsWorkspace.Name = "_launchAsWorkspace";
			this._launchAsWorkspace.Size = new System.Drawing.Size(80, 17);
			this._launchAsWorkspace.TabIndex = 0;
			this._launchAsWorkspace.Text = "Workspace";
			this._launchAsWorkspace.UseVisualStyleBackColor = true;
			// 
			// _launchAtStartup
			// 
			this._launchAtStartup.AutoSize = true;
			this._launchAtStartup.Location = new System.Drawing.Point(12, 12);
			this._launchAtStartup.Name = "_launchAtStartup";
			this._launchAtStartup.Size = new System.Drawing.Size(111, 17);
			this._launchAtStartup.TabIndex = 0;
			this._launchAtStartup.Text = "Launch at Startup";
			this._launchAtStartup.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 121);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(306, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "Note: you must close the Explorer for the change to take effect.";
			// 
			// ExplorerConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label1);
			this.Controls.Add(this._launchAtStartup);
			this.Controls.Add(this._launchAsGroupBox);
			this.Name = "ExplorerConfigurationComponentControl";
			this.Size = new System.Drawing.Size(338, 157);
			this._launchAsGroupBox.ResumeLayout(false);
			this._launchAsGroupBox.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.GroupBox _launchAsGroupBox;
		private System.Windows.Forms.RadioButton _launchAsShelf;
		private System.Windows.Forms.RadioButton _launchAsWorkspace;
		private System.Windows.Forms.CheckBox _launchAtStartup;
		private System.Windows.Forms.Label label1;
    }
}
