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

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DiskspaceManagerConfigurationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DiskspaceManagerConfigurationComponentViewExtensionPoint))]
    public class DiskspaceManagerConfigurationComponentView : WinFormsView, IApplicationComponentView
    {
        private DiskspaceManagerConfigurationComponent _component;
        private DiskspaceManagerConfigurationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DiskspaceManagerConfigurationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DiskspaceManagerConfigurationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
