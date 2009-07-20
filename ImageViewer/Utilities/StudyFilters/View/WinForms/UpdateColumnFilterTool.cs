using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	[MenuAction("update", "studyfilters-columnfilters/MenuUpdateAndClose", "Update")]
	[ExtensionOf(typeof (StudyFilterColumnToolExtensionPoint))]
	public class UpdateColumnFilterTool : StudyFilterColumnTool
	{
		public void Update()
		{
			base.StudyFilter.Refresh();
		}
	}
}