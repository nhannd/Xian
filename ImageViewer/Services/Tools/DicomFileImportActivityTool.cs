using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[MenuAction("activate", "global-menus/MenuTools/MenuUtilities/MenuDicomFileImportActivity")]
	//[IconSet("activate", IconScheme.Colour, "", "Icons.DicomFileImportActivityMedium.png", "Icons.DicomFileImportActivityLarge.png")]
	[ClickHandler("activate", "Activate")]

	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class DicomFileImportActivityTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		public DicomFileImportActivityTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public void Activate()
		{
			try
			{
				LocalDataStoreActivityMonitorComponentManager.ShowImportComponent(this.Context.DesktopWindow);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageFailedToLaunchImportActivityComponent, this.Context.DesktopWindow);
			}
		}
	}
}
