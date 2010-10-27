#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Tools.Standard.ImageProperties;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ImagePropertiesApplicationComponent"/>.
    /// </summary>
    [ExtensionOf(typeof(ImagePropertiesApplicationComponentViewExtensionPoint))]
    public class ImagePropertiesApplicationComponentView : WinFormsView, IApplicationComponentView
    {
        private ImagePropertiesApplicationComponent _component;
        private ImagePropertiesApplicationComponentControl _control;

        #region IApplicationComponentView Members

        /// <summary>
        /// Called by the host to assign this view to a component.
        /// </summary>
        public void SetComponent(IApplicationComponent component)
        {
            _component = (ImagePropertiesApplicationComponent)component;
        }

        #endregion

        /// <summary>
        /// Gets the underlying GUI component for this view.
        /// </summary>
        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ImagePropertiesApplicationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
