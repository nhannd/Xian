using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.BaseTools;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter
{
	// We decorate FilterTool with this new DropDownButtonAction attribute that we've defined
	// and set the path such that it shows up in the main toolbar. We specify that the
	// contents of the menu are to retrieved using the GeDropDownMenuModel method.
	[DropDownButtonAction("apply", "global-toolbars/ToolbarStandard/ToolbarFilter", "GetDropDownMenuModel")]
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
		public ActionModelNode GetDropDownMenuModel()
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
