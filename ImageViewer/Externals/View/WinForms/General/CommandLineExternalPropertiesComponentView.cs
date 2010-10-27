#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Externals.General;

namespace ClearCanvas.ImageViewer.Externals.View.WinForms.General
{
	[ExtensionOf(typeof (CommandLineExternalPropertiesComponentViewExtensionPoint))]
	public class CommandLineExternalPropertiesComponentView : WinFormsView, IApplicationComponentView
	{
		private CommandLineExternalPropertiesComponent _component;
		private CommandLineExternalPropertiesComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (CommandLineExternalPropertiesComponent)component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new CommandLineExternalPropertiesComponentControl(_component);
				}
				return _control;
			}
		}
	}
}