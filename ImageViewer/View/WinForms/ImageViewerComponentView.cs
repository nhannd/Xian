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

namespace ClearCanvas.ImageViewer.View.WinForms
{
    [ExtensionOf(typeof(ImageViewerComponentViewExtensionPoint))]
    public class ImageViewerComponentView : WinFormsView, IApplicationComponentView
    {
        private ImageViewerControl _control;
        private ImageViewerComponent _component;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ImageViewerComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ImageViewerControl(_component);
                }
                return _control;
            }
        }
    }
}
