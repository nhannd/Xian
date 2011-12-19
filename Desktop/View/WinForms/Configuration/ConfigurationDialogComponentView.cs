#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	[ExtensionOf(typeof(ConfigurationDialogComponentViewExtensionPoint))]
	public class ConfigurationDialogComponentView : WinFormsApplicationComponentView<ConfigurationDialogComponent>
	{
		protected override object CreateGuiElement()
		{
			// NOTE: Yeah, this is a bit weird, but the ConfigurationDialogComponent
			// cannot be a container because the NavigatorComponentContainer cannot be hosted - it
			// has to be at the top level because it has cancel and accept buttons.

			var view = (IApplicationComponentView) ViewFactory.CreateAssociatedView(typeof (ConfigurationDialogComponent).BaseType);
			view.SetComponent(Component);
			var navigatorControl = (Control) view.GuiElement;

			return new ConfigurationDialogComponentControl(Component, navigatorControl);
		}
	}
}
