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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.Help
{
	public partial class AboutForm : Form
	{
		public AboutForm()
		{
			this.SuspendLayout();

			InitializeComponent();

			_version.Text = String.Format(AboutSettings.Default.VersionTextFormat, ProductInformation.GetVersion(true, true));
			_copyright.Text = ProductInformation.Copyright;
			_license.Text = ProductInformation.License;

			if (AboutSettings.Default.UseSettings)
			{
				Assembly assembly = Assembly.Load(AboutSettings.Default.BackgroundImageAssemblyName);
				if (assembly != null)
				{
					string streamName = AboutSettings.Default.BackgroundImageAssemblyName + "." + AboutSettings.Default.BackgroundImageResourceName;
					Stream stream = assembly.GetManifestResourceStream(streamName);
					if (stream != null)
					{
						this.BackgroundImage = new Bitmap(stream);
						this.ClientSize = this.BackgroundImage.Size;
					}
				}

				this._copyright.Location = AboutSettings.Default.CopyrightLocation;
				this._copyright.Size = AboutSettings.Default.CopyrightSize;
				this._copyright.AutoSize = AboutSettings.Default.CopyrightAutoSize;
				this._copyright.ForeColor = AboutSettings.Default.CopyrightForeColor;
				this._copyright.Font = AboutSettings.Default.CopyrightFontBold ? new Font(this._copyright.Font, FontStyle.Bold) : this._copyright.Font;
				this._copyright.TextAlign = AboutSettings.Default.CopyrightTextAlign;

				this._version.Location = AboutSettings.Default.VersionLocation;
				this._version.Size = AboutSettings.Default.VersionSize;
				this._version.AutoSize = AboutSettings.Default.VersionAutoSize;
				this._version.ForeColor = AboutSettings.Default.VersionForeColor;
				this._version.Font = AboutSettings.Default.VersionFontBold ? new Font(this._version.Font, FontStyle.Bold) : this._version.Font;
				this._version.TextAlign = AboutSettings.Default.VersionTextAlign;

				this._license.Visible = AboutSettings.Default.LicenseVisible;
				this._license.Location = AboutSettings.Default.LicenseLocation;
				this._license.Size = AboutSettings.Default.LicenseSize;
				this._license.AutoSize = AboutSettings.Default.LicenseAutoSize;
				this._license.ForeColor = AboutSettings.Default.LicenseForeColor;
				this._license.Font = AboutSettings.Default.LicenseFontBold ? new Font(this._license.Font, FontStyle.Bold) : this._license.Font;
				this._license.TextAlign = AboutSettings.Default.LicenseTextAlign;

				this._closeButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
				this._closeButton.LinkColor = AboutSettings.Default.CloseButtonLinkColor;
			}

			this.ResumeLayout();

			this._closeButton.Click += new EventHandler(OnCloseClicked);
		}

		private void OnCloseClicked(object sender, EventArgs e)
		{
			Close();
		}
	}
}