#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common.Utilities;
using System.IO;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.Explorer.Local
{
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class CommandLineOpenTool : Tool<IDesktopToolContext>
	{
		private static bool _alreadyProcessed = false;

		public CommandLineOpenTool()
		{
		}

		public override void Initialize()
		{
			if (_alreadyProcessed)
				return;

			_alreadyProcessed = true;

			base.Initialize();

			try
			{
				List<string> args = new List<string>(Environment.GetCommandLineArgs());
				if (args.Count > 0)
					args.RemoveAt(0); //remove process name.

				if (args.Count > 0)
				{
					if (!PermissionsHelper.IsInRole(ImageViewer.AuthorityTokens.ViewerVisible))
					{
						this.Context.DesktopWindow.ShowMessageBox(SR.MessagePermissionToOpenFilesDenied, MessageBoxActions.Ok);
						return;
					}

					CommandLine commandLine = new CommandLine(args.ToArray());
					List<string> files = BuildFileList(commandLine.Positional);
					if (files.Count > 0)
						new OpenFilesHelper(files) {WindowBehaviour = ViewerLaunchSettings.WindowBehaviour}.OpenFiles();
					else
						this.Context.DesktopWindow.ShowMessageBox(SR.MessageFileNotFound, MessageBoxActions.Ok);
				}
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		private static List<string> BuildFileList(IEnumerable<string> files)
		{
			List<string> fileList = new List<string>();

			foreach (string path in files)
			{
				if (File.Exists(path))
					fileList.Add(path);
				else if (Directory.Exists(path))
					fileList.AddRange(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories));
			}

			return fileList;
		}
	}
}
