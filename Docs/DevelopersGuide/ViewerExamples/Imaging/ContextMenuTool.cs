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
using ClearCanvas.ImageViewer.BaseTools;

// ... (other using namespace statements here)

namespace MyPlugin.Imaging
{
	[ButtonAction("apply", "global-toolbars/ToolbarStandard/ApplyMyTool", "Apply")]
	[MenuAction("apply", "imageviewer-contextmenu/ApplyMyTool", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class ContextMenuTool : ImageViewerTool
	{
		public void Apply() {}
	}
}