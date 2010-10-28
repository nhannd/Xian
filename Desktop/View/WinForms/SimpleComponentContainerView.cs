#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
	[ExtensionOf(typeof(SimpleComponentContainerViewExtensionPoint))]
	public class SimpleComponentContainerView : WinFormsView, IApplicationComponentView
	{
		private SimpleComponentContainer _component;
		private SimpleComponentContainerControl _control;

		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = (SimpleComponentContainer)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new SimpleComponentContainerControl(_component);
				}
				return _control;
			}
		}
	}
}
