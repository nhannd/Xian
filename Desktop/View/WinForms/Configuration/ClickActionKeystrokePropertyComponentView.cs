#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration.ActionModel;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	[ExtensionOf(typeof (ClickActionKeystrokePropertyComponentViewExtensionPoint))]
	public class ClickActionKeystrokePropertyComponentView : WinFormsView, IApplicationComponentView
	{
		private ClickActionKeystrokePropertyComponent _component;
		private ClickActionKeystrokePropertyComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (ClickActionKeystrokePropertyComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new ClickActionKeystrokePropertyComponentControl(_component);
				}
				return _control;
			}
		}
	}
}