#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Clipboard;

// ... (other using namespace statements here)

namespace MyPlugin.Miscellaneous
{
	[MenuAction("export", "clipboard-contextmenu/MenuExportToImage", "Export")]
	[ButtonAction("export", "clipboard-toolbar/ToolbarExportToImage", "Export")]
	[Tooltip("export", "TooltipExportToImage")]
	[IconSet("export", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	[EnabledStateObserver("export", "Enabled", "EnabledChanged")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ClipboardToolExtensionPoint))]
	public class ExportToImageTool : ClipboardTool
	{
		public ExportToImageTool() {}

		public override void Initialize()
		{
			this.Enabled = this.Context.SelectedClipboardItems.Count > 0;
			base.Initialize();
		}

		public void Export()
		{
			foreach (IClipboardItem clipboardItem in this.Context.SelectedClipboardItems)
			{
				if (clipboardItem.Item is IPresentationImage)
				{
					// Export IPresentationImage
				}
				else if (clipboardItem.Item is IDisplaySet)
				{
					// Export IDisplaySet
				}
			}
		}
	}
}