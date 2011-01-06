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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer.Common;

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