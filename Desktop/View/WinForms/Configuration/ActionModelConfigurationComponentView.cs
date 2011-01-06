#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
	[ExtensionOf(typeof (ActionModelConfigurationComponentViewExtensionPoint))]
	public class ActionModelConfigurationComponentView : WinFormsView, IApplicationComponentView
	{
		private ActionModelConfigurationComponent _component;
		private ActionModelConfigurationComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (ActionModelConfigurationComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new ActionModelConfigurationComponentControl(_component);
				}
				return _control;
			}
		}
	}
}