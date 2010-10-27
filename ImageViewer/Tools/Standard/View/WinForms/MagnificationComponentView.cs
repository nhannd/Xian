#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.InputManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	[ExtensionOf(typeof(MagnificationViewExtensionPoint))]
    public class MagnificationComponentView : WinFormsView, IMagnificationView
    {
        private MagnificationForm _form;

		public MagnificationComponentView()
		{
		}

		public override object GuiElement
        {
            get { throw new InvalidOperationException("Not valid for this view type."); }
        }

		#region IMagnificationView Members

		public void Open(float magnificationFactor, PresentationImage image, Point location)
		{
			_form = new MagnificationForm(magnificationFactor, image, location);
			_form.Show();
		}

		public void Close()
		{
			if (_form != null)
			{
				_form.Dispose();
				_form = null;
			}
		}

		public void UpdateMouseLocation(Point location)
		{
			if (_form == null)
				throw new InvalidOperationException("Open must be called before UpdateMouseInformation");

			_form.UpdateMousePosition(location);
		}

		#endregion
	}
}
