using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Layout.Basic;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
	/// <summary>
	/// A WinForms view of the <see cref="LayoutChangerAction"/>.
	/// </summary>
	[ExtensionOf(typeof (LayoutChangerActionViewExtensionPoint))]
	public class LayoutChangerActionView : WinFormsView, IActionView
	{
		private LayoutChangerAction _action;
		private LayoutChangerToolStripItem _control;

		/// <summary>
		/// Called by the framework to set the action that the view looks at.
		/// </summary>
		public void SetAction(IAction action)
		{
			_action = (LayoutChangerAction) action;
		}

		/// <summary>
		/// Gets the <see cref="System.Windows.Forms.Control"/> that implements this view, allowing
		/// a parent view to insert the control as one of its children.
		/// </summary>
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