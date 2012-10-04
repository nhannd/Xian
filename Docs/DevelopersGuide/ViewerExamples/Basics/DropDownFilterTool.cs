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

namespace MyPlugin.Basics
{
	[DropDownAction("filter", "global-toolbars/ToolbarStandard/ToolbarDropDownFilterTool", "DropDownMenuModel")]
	[Tooltip("filter", "TooltipDropDownFilterTool")]
	[IconSet("filter", IconScheme.Colour, "Icons.MyToolSmall.png", "Icons.MyToolMedium.png", "Icons.MyToolLarge.png")]
	[EnabledStateObserver("filter", "Enabled", "EnabledChanged")]
	// ... (other action attributes here)
	[ExtensionOf(typeof (ImageViewerToolExtensionPoint))]
	public class DropDownFilterTool : ImageViewerTool
	{
		public ActionModelNode DropDownMenuModel
		{
			get
			{
				// The filter tools are ImageViewerToolExtensions, so we have to get the
				// actions from the ImageViewerComponent. Note that while 
				// ImageViewerComponent.ExportedActions gets *all* the actions associated with
				// the ImageViewerComponent, the fact that we specify the site (i.e.
				// imageviewer-filterdropdownmenu) when we call CreateModel will cause 
				// the model to only contain those actions which have that site specified
				// in its path.

				return ActionModelRoot.CreateModel(
					this.GetType().FullName,
					"imageviewer-filterdropdownmenu",
					this.ImageViewer.ExportedActions);
			}
		}
	}

	[MenuAction("apply", "imageviewer-filterdropdownmenu/MenuSharpening", "Apply")]
	// ... (other action attributes here)
	//
	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class SharpeningFilterTool : ImageViewerTool
	{
		public void Apply()
		{
			// code to apply the sharpening filter
		}
	}
}