#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.CoreTools
{
	[ButtonAction("remove", DefaultToolbarActionSite + "/ToolbarRemoveItems", "RemoveItems")]
	[MenuAction("remove", DefaultContextMenuActionSite + "/MenuRemoveItems", "RemoveItems")]
	[EnabledStateObserver("remove", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[IconSet("remove", "Icons.DeleteToolSmall.png", "Icons.DeleteToolMedium.png", "Icons.DeleteToolLarge.png")]
	//
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class RemoveItemsTool : StudyFilterTool
	{
		public void RemoveItems()
		{
			List<IStudyItem> selected = new List<IStudyItem>(base.SelectedItems);
			base.Context.BulkOperationsMode = selected.Count > 50;
			try
			{
				foreach (IStudyItem item in selected)
				{
					base.Items.Remove(item);
					item.Dispose();
				}
				base.RefreshTable();
			}
			finally
			{
				base.Context.BulkOperationsMode = false;
			}
		}
	}
}