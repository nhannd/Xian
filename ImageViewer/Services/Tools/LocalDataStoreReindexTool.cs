using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Services.LocalDataStore;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[MenuAction("activate", "global-menus/MenuTools/MenuUtilities/MenuLocalDataStoreReindex")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.LocalDataStoreReindexMedium.png", "Icons.LocalDataStoreReindexLarge.png")]
	[ClickHandler("activate", "Activate")]

	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class LocalDataStoreReindexTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		public LocalDataStoreReindexTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public void Activate()
		{
			LocalDataStoreServiceClient client = new LocalDataStoreServiceClient();
			try
			{
				client.Open();
				client.Reindex();
				client.Close();

				LocalDataStoreActivityMonitorComponentManager.ShowReindexComponent(this.Context.DesktopWindow);
			}
			catch (Exception e)
			{
				client.Abort();
				ExceptionHandler.Report(e, SR.MessageFailedToStartReindex, this.Context.DesktopWindow);
			}
		}
	}
}
