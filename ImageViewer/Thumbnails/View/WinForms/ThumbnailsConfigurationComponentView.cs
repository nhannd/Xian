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
using ClearCanvas.ImageViewer.Thumbnails.Configuration;

namespace ClearCanvas.ImageViewer.Thumbnails.View.WinForms
{
	[ExtensionOf(typeof (ThumbnailsConfigurationComponentViewExtensionPoint))]
	public class ThumbnailsConfigurationComponentView : WinFormsView, IApplicationComponentView
	{
		private ThumbnailsConfigurationComponent _component;
		private ThumbnailsConfigurationComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (ThumbnailsConfigurationComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new ThumbnailsConfigurationComponentControl(_component);
				}
				return _control;
			}
		}
	}
}