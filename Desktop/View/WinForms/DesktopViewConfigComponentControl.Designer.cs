#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

namespace ClearCanvas.Desktop.View.WinForms {
	partial class DesktopViewConfigComponentControl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.Windows.Forms.GroupBox _grpGlobalToolbars;
			System.Windows.Forms.FlowLayoutPanel _flowGlobalToolbars;
			this._chkWrapGlobalToolbars = new System.Windows.Forms.CheckBox();
			_grpGlobalToolbars = new System.Windows.Forms.GroupBox();
			_flowGlobalToolbars = new System.Windows.Forms.FlowLayoutPanel();
			_grpGlobalToolbars.SuspendLayout();
			_flowGlobalToolbars.SuspendLayout();
			this.SuspendLayout();
			// 
			// _grpGlobalToolbars
			// 
			_grpGlobalToolbars.Controls.Add(_flowGlobalToolbars);
			_grpGlobalToolbars.Dock = System.Windows.Forms.DockStyle.Top;
			_grpGlobalToolbars.Location = new System.Drawing.Point(0, 0);
			_grpGlobalToolbars.Name = "_grpGlobalToolbars";
			_grpGlobalToolbars.Padding = new System.Windows.Forms.Padding(8);
			_grpGlobalToolbars.Size = new System.Drawing.Size(554, 120);
			_grpGlobalToolbars.TabIndex = 0;
			_grpGlobalToolbars.TabStop = false;
			_grpGlobalToolbars.Text = "Global Toolbars";
			// 
			// _flowGlobalToolbars
			// 
			_flowGlobalToolbars.Controls.Add(this._chkWrapGlobalToolbars);
			_flowGlobalToolbars.Dock = System.Windows.Forms.DockStyle.Fill;
			_flowGlobalToolbars.Location = new System.Drawing.Point(8, 21);
			_flowGlobalToolbars.Name = "_flowGlobalToolbars";
			_flowGlobalToolbars.Size = new System.Drawing.Size(538, 91);
			_flowGlobalToolbars.TabIndex = 0;
			// 
			// _chkWrapGlobalToolbars
			// 
			this._chkWrapGlobalToolbars.AutoSize = true;
			this._chkWrapGlobalToolbars.Location = new System.Drawing.Point(3, 3);
			this._chkWrapGlobalToolbars.Name = "_chkWrapGlobalToolbars";
			this._chkWrapGlobalToolbars.Size = new System.Drawing.Size(147, 17);
			this._chkWrapGlobalToolbars.TabIndex = 1;
			this._chkWrapGlobalToolbars.Text = "Wrap toolbars horizontally";
			this._chkWrapGlobalToolbars.UseVisualStyleBackColor = true;
			// 
			// DesktopViewConfigComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(_grpGlobalToolbars);
			this.Name = "DesktopViewConfigComponentControl";
			this.Size = new System.Drawing.Size(554, 258);
			_grpGlobalToolbars.ResumeLayout(false);
			_flowGlobalToolbars.ResumeLayout(false);
			_flowGlobalToolbars.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.CheckBox _chkWrapGlobalToolbars;
	}
}
