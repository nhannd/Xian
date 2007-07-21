using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.ImageProcessing.Filter.View.WinForms
{
	[ExtensionOf(typeof(DropDownButtonActionViewExtensionPoint))]
	public class DropDownButtonItemView : WinFormsView, IActionView
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

		public override object GuiElement
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
