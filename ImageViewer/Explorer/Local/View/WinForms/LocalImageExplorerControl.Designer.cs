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

namespace ClearCanvas.ImageViewer.Explorer.Local.View.WinForms
{
	partial class LocalImageExplorerControl
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
			this._fileBrowser = new ClearCanvas.Desktop.View.WinForms.FileBrowser.Browser();
			this._shellBrowser = new ClearCanvas.Desktop.View.WinForms.FileBrowser.ShellDll.ShellBrowser();
			this.SuspendLayout();
			// 
			// _fileBrowser
			// 
			this._fileBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fileBrowser.ListViewMode = System.Windows.Forms.View.Details;
			this._fileBrowser.Location = new System.Drawing.Point(0, 0);
			this._fileBrowser.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
			this._fileBrowser.Name = "_fileBrowser";
			this._fileBrowser.ShellBrowser = this._shellBrowser;
			this._fileBrowser.Size = new System.Drawing.Size(959, 597);
			this._fileBrowser.SplitterDistance = 162;
			this._fileBrowser.TabIndex = 0;
			// 
			// LocalImageExplorerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._fileBrowser);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "LocalImageExplorerControl";
			this.Size = new System.Drawing.Size(959, 597);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.FileBrowser.Browser _fileBrowser;
		private ClearCanvas.Desktop.View.WinForms.FileBrowser.ShellDll.ShellBrowser _shellBrowser;
		private System.Windows.Forms.ContextMenuStrip _contextMenu;
	}
}
