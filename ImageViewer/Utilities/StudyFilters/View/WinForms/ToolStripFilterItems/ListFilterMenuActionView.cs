using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems
{
	[ExtensionOf(typeof (ListFilterMenuActionViewExtensionPoint))]
	public class ListFilterMenuActionView : WinFormsView, IActionView
	{
		private ListFilterMenuAction _action;
		private ListFilterToolStripItem _control;

		public void SetAction(IAction component)
		{
			_action = (ListFilterMenuAction) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new ListFilterToolStripItem(_action);
				}
				return _control;
			}
		}
	}
}