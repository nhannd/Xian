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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.Explorer.Local;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	[MenuAction("openMpr", "explorerlocal-contextmenu/MenuOpenMpr", "OpenMpr")]
	[Tooltip("openMpr", "OpenDicomFilesVerbose")]
	[IconSet("openMpr", IconScheme.Colour, "Icons.OpenMprToolLarge.png", "Icons.OpenMprToolMedium.png", "Icons.OpenMprToolSmall.png")]
	[EnabledStateObserver("openMpr", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof (LocalImageExplorerToolExtensionPoint))]
	public class OpenMprLocalImageExplorerTool : Tool<ILocalImageExplorerToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public OpenMprLocalImageExplorerTool()
		{
			_enabled = true;
		}

		/// <summary>
		/// Called to determine whether this tool is enabled/disabled in the UI.
		/// </summary>
		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Notifies that the Enabled state of this tool has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public void OpenMpr()
		{
			string[] files = BuildFileList();
			if (files.Length == 0)
				return;

			try
			{
				//host component as workspace.
				VolumeLoader loader = new VolumeLoader();
				Volume volume = loader.LoadFromFiles(files);
				ImageViewerComponent component = MprLayoutManager.CreateMprLayoutAndComponent(volume);
				if (volume == null)
					return;

				LaunchImageViewerArgs args = new LaunchImageViewerArgs(ViewerLaunchSettings.WindowBehaviour);
				args.Title = "MPR (Local Files)";
				ImageViewerComponent.Launch(component, args);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		private string[] BuildFileList()
		{
			List<string> fileList = new List<string>();

			foreach (string path in this.Context.SelectedPaths)
			{
				if (File.Exists(path))
					fileList.Add(path);
				else if (Directory.Exists(path))
					fileList.AddRange(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories));
			}

			return fileList.ToArray();
		}
	}
}