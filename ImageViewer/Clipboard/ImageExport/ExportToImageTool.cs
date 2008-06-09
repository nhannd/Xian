using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

#pragma warning disable 0419,1574,1587,1591

namespace ClearCanvas.ImageViewer.Clipboard.ImageExport
{
	[MenuAction("export", "clipboard-contextmenu/MenuExportToImage", "Export")]
	[ButtonAction("export", "clipboard-toolbar/ToolbarExportToImage", "Export")]
	[Tooltip("export", "TooltipExportToImage")]
	[IconSet("export", IconScheme.Colour, "Icons.ExportToImageToolSmall.png", "Icons.ExportToImageToolSmall.png", "Icons.ExportToImageToolSmall.png")]
	[EnabledStateObserver("export", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ClipboardToolExtensionPoint))]
	public class ExportToImageTool : ClipboardTool
	{
		public ExportToImageTool()
		{
		}

		public void Export()
		{
			try
			{
				List<IClipboardItem> selectedClipboardItems = new List<IClipboardItem>(this.Context.SelectedClipboardItems);
				ImageExportComponent.Launch(this.Context.DesktopWindow, selectedClipboardItems);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}