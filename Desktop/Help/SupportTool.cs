#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop.Help
{
	[MenuAction("getSupport", "global-menus/MenuHelp/MenuGetSupport", "ShowProductSupport")]
	[GroupHint("getSupport", "Application.Help.Support")]
	[ExtensionOf(typeof (DesktopToolExtensionPoint), Enabled = false)]
	public sealed class SupportTool : Tool<IDesktopToolContext>
	{
		public void ShowProductSupport()
		{
			Execute(@"http://www.clearcanvas.ca/support/", SR.URLNotFound);
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
				Context.DesktopWindow.ShowMessageBox(errorMessage, MessageBoxActions.Ok);
		}
	}
}