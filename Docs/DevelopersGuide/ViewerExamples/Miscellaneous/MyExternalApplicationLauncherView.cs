#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace MyPlugin.Miscellaneous
{
	[ExtensionOf(typeof (MyExternalApplicationLauncherPropertiesViewExtensionPoint))]
	public class MyExternalApplicationLauncherPropertiesView : WinFormsView, IApplicationComponentView
	{
		private MyExternalApplicationLauncherProperties _component;
		private MyExternalApplicationLauncherPropertiesControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (MyExternalApplicationLauncherProperties) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new MyExternalApplicationLauncherPropertiesControl(_component);
				}
				return _control;
			}
		}
	}

	public class MyExternalApplicationLauncherPropertiesControl : Control
	{
		public MyExternalApplicationLauncherPropertiesControl(MyExternalApplicationLauncherProperties component) {}
	}
}