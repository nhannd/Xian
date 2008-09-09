using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	[ExtensionOf(typeof (LayoutChangerActionViewExtensionPoint))]
	public class LayoutChangerActionView : WinFormsView, IActionView
	{
		private LayoutChangerAction _action;
		private LayoutChangerToolStripItem _control;

		public void SetAction(IAction action)
		{
			_action = (LayoutChangerAction) action;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new LayoutChangerToolStripItem(_action);
				}
				return _control;
			}
		}
	}
}