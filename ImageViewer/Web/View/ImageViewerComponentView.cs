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

namespace ClearCanvas.ImageViewer.Web.View
{
    [ExtensionOf(typeof(ImageViewerComponentViewExtensionPoint))]
    public class ImageViewerComponentView : WebView, IApplicationComponentView
    {
        private ImageViewerComponent _component;

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ImageViewerComponent)component;
        }

        #endregion

       
    }
}
