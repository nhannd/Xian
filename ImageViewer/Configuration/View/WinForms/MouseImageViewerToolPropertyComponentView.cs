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

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
	[ExtensionOf(typeof (MouseImageViewerToolPropertyComponentViewExtensionPoint))]
	public class MouseImageViewerToolPropertyComponentView : WinFormsView, IApplicationComponentView
	{
		private MouseImageViewerToolPropertyComponent _component;
		private MouseImageViewerToolPropertyComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (MouseImageViewerToolPropertyComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new MouseImageViewerToolPropertyComponentControl(_component);
				}
				return _control;
			}
		}
	}
}