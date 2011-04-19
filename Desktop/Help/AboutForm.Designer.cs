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

namespace ClearCanvas.Desktop.Help
{
	partial class AboutForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer _components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (_components != null))
			{
				_components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
			this._closeButton = new System.Windows.Forms.LinkLabel();
			this._version = new System.Windows.Forms.Label();
			this._copyright = new System.Windows.Forms.Label();
			this._license = new System.Windows.Forms.Label();
			this._manifest = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _closeButton
			// 
			resources.ApplyResources(this._closeButton, "_closeButton");
			this._closeButton.BackColor = System.Drawing.Color.Transparent;
			this._closeButton.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(150)))), ((int)(((byte)(208)))));
			this._closeButton.Name = "_closeButton";
			this._closeButton.TabStop = true;
			// 
			// _version
			// 
			this._version.BackColor = System.Drawing.Color.Transparent;
			this._version.ForeColor = System.Drawing.Color.White;
			resources.ApplyResources(this._version, "_version");
			this._version.Name = "_version";
			// 
			// _copyright
			// 
			this._copyright.BackColor = System.Drawing.Color.Transparent;
			this._copyright.ForeColor = System.Drawing.Color.Black;
			resources.ApplyResources(this._copyright, "_copyright");
			this._copyright.Name = "_copyright";
			// 
			// _license
			// 
			resources.ApplyResources(this._license, "_license");
			this._license.BackColor = System.Drawing.Color.Transparent;
			this._license.ForeColor = System.Drawing.Color.Black;
			this._license.Name = "_license";
			// 
			// _manifest
			// 
			resources.ApplyResources(this._manifest, "_manifest");
			this._manifest.BackColor = System.Drawing.Color.Transparent;
			this._manifest.ForeColor = System.Drawing.Color.Firebrick;
			this._manifest.Name = "_manifest";
			// 
			// AboutForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this._closeButton;
			this.ControlBox = false;
			this.Controls.Add(this._manifest);
			this.Controls.Add(this._license);
			this.Controls.Add(this._copyright);
			this.Controls.Add(this._version);
			this.Controls.Add(this._closeButton);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.LinkLabel _closeButton;
		private System.Windows.Forms.Label _version;
		private System.Windows.Forms.Label _copyright;
		private System.Windows.Forms.Label _license;
        private System.Windows.Forms.Label _manifest;
	}
}