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
using ClearCanvas.ImageViewer.BaseTools;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter
{
	// We decorate FilterTool with the DropDownButtonAction attribute
	// and set the path such that it shows up in the main toolbar. We specify that the
	// contents of the menu are to retrieved using the DropDownMenuModel property.
	[DropDownAction("apply", "global-toolbars/ToolbarStandard/ToolbarFilter", "DropDownMenuModel")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[Tooltip("apply", "Filters")]
	[IconSet("apply", IconScheme.Colour, "Icons.FilterToolSmall.png", "Icons.FilterToolMedium.png", "Icons.FilterToolLarge.png")]

	[ExtensionOf(typeof(ImageViewerToolExtensionPoint))]
	public class FilterTool : ImageViewerTool
	{ 
		public FilterTool()
		{
		}

		// We have to provide the dropdown button with the data to populate the dropdown menu.
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
}
