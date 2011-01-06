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
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	[ExtensionOf(typeof (ToolConfigurationComponentViewExtensionPoint))]
	public class ToolConfigurationComponentView : WinFormsView, IApplicationComponentView
	{
		private ToolConfigurationComponent _component;
		private ToolConfigurationComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (ToolConfigurationComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new ToolConfigurationComponentControl(_component);
				}
				return _control;
			}
		}
	}
}