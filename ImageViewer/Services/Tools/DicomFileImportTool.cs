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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Explorer.Local;
using ClearCanvas.ImageViewer.Services.Auditing;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[MenuAction("Import", "explorerlocal-contextmenu/ImportDicomFiles", "Import")]
	[Tooltip("Import", "TooltipImportDicomFiles")]
	[IconSet("Import", "Icons.DicomFileImportToolSmall.png", "Icons.DicomFileImportToolMedium.png", "Icons.DicomFileImportToolLarge.png")]
	[EnabledStateObserver("Import", "Enabled", "EnabledChanged")]
	[ViewerActionPermission("Import", ImageViewer.Services.AuthorityTokens.Study.Import)]

	[ExtensionOf(typeof(LocalImageExplorerToolExtensionPoint))]
	public class DicomFileImportTool : Tool<ILocalImageExplorerToolContext>
	{
		public event EventHandler EnabledChanged;
		private bool _enabled = true;

		public DicomFileImportTool()
		{
		}

		public bool Enabled
		{
			get { return _enabled; }
			private set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(EnabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			Context.SelectedPathsChanged += OnContextSelectedPathsChanged;
		}

		protected override void Dispose(bool disposing)
		{
			Context.SelectedPathsChanged -= OnContextSelectedPathsChanged;
			base.Dispose(disposing);
		}

		private void OnContextSelectedPathsChanged(object sender, EventArgs e)
		{
			Enabled = Context.SelectedPaths.Count > 0;
		}

		public void Import()
		{
			List<string> filePaths = new List<string>();

			foreach (string path in this.Context.SelectedPaths)
			{
				if (string.IsNullOrEmpty(path))
					continue;

				filePaths.Add(path);
			}

			if (filePaths.Count == 0)
				return;

			FileImportRequest request = new FileImportRequest();
			request.FilePaths = filePaths;
			request.BadFileBehaviour = BadFileBehaviour.Ignore;
			request.Recursive = true;
			request.FileImportBehaviour = FileImportBehaviour.Copy;

			LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();
			try
			{
				client.Open();
				client.Import(request);
				client.Close();

				AuditHelper.LogImportStudies(new AuditedInstances(), EventSource.CurrentUser, EventResult.Success);

				LocalDataStoreActivityMonitorComponentManager.ShowImportComponent(this.Context.DesktopWindow);
			}
			catch (EndpointNotFoundException)
			{
				client.Abort();
				this.Context.DesktopWindow.ShowMessageBox(SR.MessageImportLocalDataStoreServiceNotRunning, MessageBoxActions.Ok);
			}
			catch (Exception e)
			{
				client.Abort();
				ExceptionHandler.Report(e, SR.MessageFailedToImportSelection, this.Context.DesktopWindow);
			}
		}
	}
}
