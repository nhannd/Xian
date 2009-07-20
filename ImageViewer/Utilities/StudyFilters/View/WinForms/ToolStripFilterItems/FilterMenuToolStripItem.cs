using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems
{
	public class FilterMenuToolStripItem : ActiveMenuItem
	{
		public FilterMenuToolStripItem(FilterMenuAction action) : base(action) {}

		protected override void OnClick(System.EventArgs e) {
			//base.OnClick(e);
		}
	}
}