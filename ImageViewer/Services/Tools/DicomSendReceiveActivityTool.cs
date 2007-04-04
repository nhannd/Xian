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
	[MenuAction("activate", "global-menus/MenuTools/MenuUtilities/MenuDicomSendReceiveActivity")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.DicomSendReceiveActivityMedium.png", "Icons.DicomSendReceiveActivityLarge.png")]
	[ClickHandler("activate", "Activate")]

	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class DicomSendReceiveActivityTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		public DicomSendReceiveActivityTool()
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
				LocalDataStoreActivityMonitorComponentManager.ShowSendReceiveActivityComponent(this.Context.DesktopWindow);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.MessageFailedToLaunchSendReceiveActivityComponent, this.Context.DesktopWindow);
			}
		}
	}
}
