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
using ClearCanvas.ImageViewer.Volume.Mpr.Configuration;

namespace ClearCanvas.ImageViewer.Volume.Mpr.View.WinForms
{
	[ExtensionOf(typeof (MprConfigurationComponentViewExtensionPoint))]
	public class MprConfigurationComponentView : WinFormsView, IApplicationComponentView
	{
		private MprConfigurationComponent _component;
		private MprConfigurationComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (MprConfigurationComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new MprConfigurationComponentControl(_component);
				}
				return _control;
			}
		}
	}
}