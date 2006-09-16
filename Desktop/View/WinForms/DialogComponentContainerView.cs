using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.Desktop.View.WinForms
{
	[ExtensionOf(typeof(DialogComponentContainerViewExtensionPoint))]
	public class DialogComponentContainerView : WinFormsView, IApplicationComponentView
	{
		private DialogComponentContainer _component;
		private DialogComponentContainerControl _control;

		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = (DialogComponentContainer)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new DialogComponentContainerControl(_component);
				}
				return _control;
			}
		}
	}
}
