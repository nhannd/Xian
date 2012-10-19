#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	[ExtensionOf(typeof(StackedComponentContainerViewExtensionPoint))]
	public class StackedComponentContainerView : WinFormsView, IApplicationComponentView
	{
		private StackedComponentContainer _component;
		private StackedComponentContainerControl _control;

		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			Platform.CheckForNullReference(component, "component");
			_component = (StackedComponentContainer)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new StackedComponentContainerControl(_component);
				}
				return _control;
			}
		}
	}
}
