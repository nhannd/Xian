using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.BaseTools
{
	[ButtonAction("removeItems", DefaultToolbarActionSite + "/ToolbarRemoveItems", "RemoveItems")]
	[EnabledStateObserver("removeItems", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[IconSet("removeItems", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolMedium.png", "Icons.DeleteToolLarge.png")]
	//
	[ButtonAction("removeItemsContext", DefaultContextMenuActionSite + "/MenuRemoveItems", "RemoveItems")]
	[VisibleStateObserver("removeItemsContext", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[IconSet("removeItemsContext", IconScheme.Colour, "Icons.DeleteToolSmall.png", "Icons.DeleteToolMedium.png", "Icons.DeleteToolLarge.png")]
	//
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class RemoveItemsTool : StudyFilterTool
	{
		public void RemoveItems()
		{
			List<StudyItem> selected = new List<StudyItem>(base.Selection);
			base.Component.BulkOperationsMode = selected.Count > 50;
			try
			{
				foreach (StudyItem item in selected)
					base.Component.Items.Remove(item);
				base.Component.Refresh();
			}
			finally
			{
				base.Component.BulkOperationsMode = false;
			}
		}
	}
}