#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	/// <summary>
	/// Provides a Windows Forms view onto <see cref="SettingsManagementComponent"/>
	/// </summary>
	[ExtensionOf(typeof(SettingsManagementComponentViewExtensionPoint))]
	public class SettingsManagementComponentView : WinFormsView, IApplicationComponentView
	{
		private SettingsManagementComponent _component;
		private SettingsManagementComponentControl _control;


		#region IApplicationComponentView Members

		public void SetComponent(IApplicationComponent component)
		{
			_component = (SettingsManagementComponent)component;
		}

		#endregion

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new SettingsManagementComponentControl(_component);
				}
				return _control;
			}
		}
	}
}