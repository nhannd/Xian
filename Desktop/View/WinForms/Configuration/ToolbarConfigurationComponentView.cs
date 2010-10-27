#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration.Standard;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	[ExtensionOf(typeof (ToolbarConfigurationComponentViewExtensionPoint))]
	public sealed class ToolbarConfigurationComponentView : WinFormsView, IApplicationComponentView
	{
		private ToolbarConfigurationComponent _component;
		private ToolbarConfigurationComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (ToolbarConfigurationComponent)component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
					_control = new ToolbarConfigurationComponentControl(_component);
				return _control;
			}
		}
	}
}