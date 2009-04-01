using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	[ButtonAction("show", DefaultToolbarActionSite + "/ToolbarAddRemoveColumns", "Show")]
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
				foreach (StudyFilterColumn column in component.Columns)
				{
					base.Columns.Add(column);
				}
			}
		}
	}
}