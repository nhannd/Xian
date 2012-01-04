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

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.CoreTools
{
	[ButtonAction("show", DefaultToolbarActionSite + "/ToolbarAddRemoveColumns", "Show")]
	[IconSet("show", "Icons.AddRemoveColumnsToolSmall.png", "Icons.AddRemoveColumnsToolMedium.png", "Icons.AddRemoveColumnsToolLarge.png")]
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class AddRemoveColumnsTool : StudyFilterTool
	{
		public void Show()
		{
			ColumnPickerComponent component = new ColumnPickerComponent(base.Columns);
			SimpleComponentContainer container = new SimpleComponentContainer(component);
			DialogBoxAction action = base.DesktopWindow.ShowDialogBox(container, SR.AddRemoveColumns);
			if (action == DialogBoxAction.Ok)
			{
				base.Columns.Clear();
				foreach (StudyFilterColumn.ColumnDefinition column in component.Columns)
				{
					base.Columns.Add(column.Create());
				}

				base.RefreshTable();
			}
		}
	}
}