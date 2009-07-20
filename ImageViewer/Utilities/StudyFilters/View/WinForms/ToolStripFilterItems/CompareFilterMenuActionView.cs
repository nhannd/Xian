using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.ToolStripFilterItems
{
	[ExtensionOf(typeof (CompareFilterMenuActionViewExtensionPoint))]
	public class CompareFilterMenuActionView : WinFormsView, IActionView
	{
		private CompareFilterMenuAction _action;
		private ToolStripItem _control;

		public void SetAction(IAction action)
		{
			_action = (CompareFilterMenuAction) action;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new CompareFilterToolStripItem(_action);
				}
				return _control;
			}
		}
	}
}