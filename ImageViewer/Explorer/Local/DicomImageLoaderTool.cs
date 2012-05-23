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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Local
{
	[MenuAction("Open", "explorerlocal-contextmenu/MenuOpenFiles", "Open")]
	[Tooltip("Open", "OpenDicomFilesVerbose")]
	[IconSet("Open", "Icons.OpenToolSmall.png", "Icons.OpenToolMedium.png", "Icons.OpenToolLarge.png")]
	[EnabledStateObserver("Open", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(LocalImageExplorerToolExtensionPoint))]
	public class DicomImageLoaderTool : Tool<ILocalImageExplorerToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public DicomImageLoaderTool()
		{
			_enabled = true;
		}

		/// <summary>
		/// Called by the framework to initialize this tool.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			this.Context.DefaultActionHandler = Open;
			Context.SelectedPathsChanged += OnContextSelectedPathsChanged;
		}

		protected override void Dispose(bool disposing)
		{
			Context.SelectedPathsChanged -= OnContextSelectedPathsChanged;
			base.Dispose(disposing);
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

		public void Open()
		{
			string[] files = BuildFileList();
			if (files.Length == 0)
			{
				Context.DesktopWindow.ShowMessageBox(SR.MessageNoFilesSelected, MessageBoxActions.Ok);
				return;
			}

			try
			{
				new OpenFilesHelper(files) {WindowBehaviour = ViewerLaunchSettings.WindowBehaviour}.OpenFiles();
			}
			catch(Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageUnableToOpenImages, Context.DesktopWindow);
			}
		}

		private string[] BuildFileList()
		{
			List<string> fileList = new List<string>();

			foreach (string path in this.Context.SelectedPaths)
			{
				if (string.IsNullOrEmpty(path))
					continue;

				if (File.Exists(path))
					fileList.Add(path);
				else if (Directory.Exists(path))
					fileList.AddRange(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories));
			}

			return fileList.ToArray();
		}

		private void OnContextSelectedPathsChanged(object sender, EventArgs e)
		{
			Enabled = Context.SelectedPaths.Count > 0;
		}
	}
}