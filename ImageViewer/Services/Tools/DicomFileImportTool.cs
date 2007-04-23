using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Explorer.Local;
using ClearCanvas.ImageViewer.Services.LocalDataStore;
using System.ServiceModel;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[MenuAction("Import", "explorerlocal-contextmenu/ImportDicomFiles")]
	[Tooltip("Import", "TooltipImportDicomFiles")]
	[IconSet("Import", IconScheme.Colour, "Icons.DicomFileImportSmall.png", "Icons.DicomFileImportSmall.png", "Icons.DicomFileImportSmall.png")]
	[ClickHandler("Import", "Import")]

	[ExtensionOf(typeof(LocalImageExplorerToolExtensionPoint))]
	public class DicomFileImportTool : Tool<ILocalImageExplorerToolContext>
	{
		public DicomFileImportTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public void Import()
		{
			List<string> filePaths = new List<string>();

			foreach (string path in this.Context.SelectedPaths)
			{
				filePaths.Add(path);
			}

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

				LocalDataStoreActivityMonitorComponentManager.ShowImportComponent(this.Context.DesktopWindow);
			}
			catch (EndpointNotFoundException)
			{
				client.Abort();
				Platform.ShowMessageBox(SR.MessageImportLocalDataStoreServiceNotRunning);
			}
			catch (Exception e)
			{
				client.Abort();
				ExceptionHandler.Report(e, SR.MessageFailedToImportSelection, this.Context.DesktopWindow);
			}
		}
	}
}
