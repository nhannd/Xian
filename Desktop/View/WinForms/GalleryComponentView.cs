#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
	[ExtensionOf(typeof (GalleryComponentViewExtensionPoint))]
	public class GalleryComponentView : WinFormsView, IApplicationComponentView
	{
		private GalleryComponent _component;
		private GalleryComponentControl _control;

		public void SetComponent(IApplicationComponent component)
		{
			_component = (GalleryComponent) component;
		}

		public override object GuiElement
		{
			get
			{
				if (_control == null)
				{
					_control = new GalleryComponentControl(_component);
				}
				return _control;
			}
		}
	}
}