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
using ClearCanvas.ImageViewer.Externals.Config;

namespace ClearCanvas.ImageViewer.Externals.View.WinForms
{
	[ExtensionOf(typeof (ExternalsConfigurationComponentViewExtensionPoint))]
	public class ExternalsConfigurationComponentView : WinFormsView, IApplicationComponentView
	{
		private ExternalsConfigurationComponent _component;
		private ExternalsConfigurationComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (ExternalsConfigurationComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new ExternalsConfigurationComponentControl(_component);
				}
				return _control;
			}
		}
	}
}