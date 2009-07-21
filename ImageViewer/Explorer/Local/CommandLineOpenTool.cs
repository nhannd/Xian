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
						OpenStudyHelper.OpenFiles(files.ToArray(), ViewerLaunchSettings.WindowBehaviour);
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
