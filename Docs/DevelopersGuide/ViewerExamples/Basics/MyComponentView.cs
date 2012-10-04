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

namespace MyPlugin.Basics
{
	[ExtensionOf(typeof (MyComponentViewExtensionPoint))]
	public class MyComponentView : WinFormsView, IApplicationComponentView
	{
		private MyComponent _component;
		private MyComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (MyComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new MyComponentControl(_component);
				}
				return _control;
			}
		}
	}
}