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

using System;
using System.Windows.Forms;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Help
{
	[MenuAction("showAbout", "global-menus/MenuHelp/MenuAbout", "ShowAbout")]
	[GroupHint("showAbout", "Application.Help.About")]

	[MenuAction("showWebsite", "global-menus/MenuHelp/MenuWebsite", "ShowWebsite")]
	[GroupHint("showWebsite", "Application.Help.Website")]

	[MenuAction("showUsersGuide", "global-menus/MenuHelp/MenuUsersGuide", "ShowUsersGuide")]
	[GroupHint("showUsersGuide", "Application.Help.UsersGuide")]

	[MenuAction("showLicense", "global-menus/MenuHelp/MenuLicense", "ShowLicense")]
	[GroupHint("showLicense", "Application.Help.License")]

	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class HelpTool : Tool<IDesktopToolContext>
	{
		public HelpTool()
		{
		}

        public void ShowAbout()
		{
			AboutForm aboutForm = new AboutForm();
			aboutForm.ShowDialog();
		}

		public void ShowWebsite()
		{
			Execute("http://www.clearcanvas.ca", SR.URLNotFound);
		}

		public void ShowUsersGuide()
		{
			string guideFile = HelpSettings.Default.UserGuidePath;
			if (!string.IsNullOrEmpty(guideFile))
				Execute(guideFile, SR.HelpNotFound);
		}

		public void ShowLicense()
		{
			Execute("EULA.rtf", SR.HelpNotFound);
		}

		private void Execute(string filename, string errorMesage) {
			ProcessStartInfo nfo = new ProcessStartInfo();
			nfo.WorkingDirectory = Platform.InstallDirectory;
			nfo.FileName = filename;

			try 
			{
				Process.Start(nfo);
			}
			catch
			{
				this.Context.DesktopWindow.ShowMessageBox(errorMesage, MessageBoxActions.Ok);
			}
		}
	}
}
