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
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DesktopMonitorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DesktopMonitorComponentViewExtensionPoint))]
    public class DesktopMonitorComponentView : WinFormsView, IApplicationComponentView
    {
        private DesktopMonitorComponent _component;
        private DesktopMonitorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DesktopMonitorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DesktopMonitorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
