using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools.Actions
{
	[ExtensionPoint]
	public class FilterMenuActionViewExtensionPoint : ExtensionPoint<IActionView> {}

	[AssociateView(typeof (FilterMenuActionViewExtensionPoint))]
	public class FilterMenuAction : MenuAction
	{
		public FilterMenuAction(string actionID, string actionPath, IResourceResolver resourceResolver)
			: this(actionID, actionPath, ClickActionFlags.None, resourceResolver) {}

		public FilterMenuAction(string actionID, string actionPath, ClickActionFlags flags, IResourceResolver resourceResolver)
			: base(actionID, new ActionPath(actionPath, resourceResolver), flags, resourceResolver)
		{
			base.Label = base.Path.LastSegment.LocalizedText;
		}
	}
}