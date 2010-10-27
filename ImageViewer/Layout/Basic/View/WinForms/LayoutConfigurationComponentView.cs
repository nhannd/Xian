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

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="LayoutSettingsApplicationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(LayoutConfigurationComponentViewExtensionPoint))]
    public class LayoutConfigurationComponentView : WinFormsView, IApplicationComponentView
    {
        private LayoutConfigurationComponent _component;
        private LayoutConfigurationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (LayoutConfigurationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new LayoutConfigurationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
