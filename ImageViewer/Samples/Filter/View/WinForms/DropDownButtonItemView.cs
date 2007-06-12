using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Samples.Filter.View.WinForms
{
	[GuiToolkitAttribute(GuiToolkitID.WinForms)]
	[ExtensionOf(typeof(DropDownButtonActionViewExtensionPoint))]
	public class DropDownButtonItemView : IActionView
	{
		DropDownButtonAction _action;
		DropDownButtonItem _button;

		#region IActionView Members

		public void SetAction(IAction action)
		{
			_action = action as DropDownButtonAction;
		}

		#endregion

		#region IView Members

		public GuiToolkitID GuiToolkitID
		{
			get { return GuiToolkitID.WinForms; }
		}

		public object GuiElement
		{
			get
			{
				if (_button == null)
					_button = new DropDownButtonItem(_action);

				return _button;
			}
		}

		#endregion
	}
}
