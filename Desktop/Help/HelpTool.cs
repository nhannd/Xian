#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using System;

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

	[MenuAction("showLogs", "global-menus/MenuHelp/MenuShowLogs", "ShowLogs")]
	[GroupHint("showLogs", "Application.Help.Support")]
	[ActionPermission("showLogs", AuthorityTokens.Desktop.ShowLogs)]

	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class HelpTool : Tool<IDesktopToolContext>
	{
		public HelpTool()
		{
		}

        public void ShowAbout()
		{
			var aboutForm = AboutDialogExtensionPoint.CreateDialog();
			aboutForm.ShowDialog();
		}

		public void ShowWebsite()
		{
			Execute("http://www.clearcanvas.ca", SR.URLNotFound);
		}

		public void ShowUsersGuide()
		{
			Execute(HelpSettings.Default.UserGuidePath, SR.UsersGuideNotFound);
		}

		public void ShowLicense()
		{
			Execute("EULA.rtf", SR.LicenseNotFound);
		}

		public void ShowLogs()
		{
			string logdir = Platform.LogDirectory;
			if (!string.IsNullOrEmpty(logdir) && Directory.Exists(logdir))
				Process.Start(logdir);
		}

		private void Execute(string filename, string errorMessage)
		{
			bool showMessageBox = String.IsNullOrEmpty(filename);
			if (!showMessageBox)
			{
				try
				{
					ProcessStartInfo info = new ProcessStartInfo();
					info.WorkingDirectory = Platform.InstallDirectory;
					info.FileName = filename;

					Process.Start(info);
				}
				catch (Exception e)
				{
					showMessageBox = true;
					Platform.Log(LogLevel.Warn, e, "Failed to launch '{0}'.", filename);
				}
			}

			if (showMessageBox)
				this.Context.DesktopWindow.ShowMessageBox(errorMessage, MessageBoxActions.Ok);
		}
	}
}
