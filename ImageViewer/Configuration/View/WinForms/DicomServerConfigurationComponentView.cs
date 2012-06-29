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
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DicomServerConfigurationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DicomServerConfigurationComponentViewExtensionPoint))]
    public class DicomServerConfigurationComponentView : WinFormsView, IApplicationComponentView
    {
        private DicomServerConfigurationComponent _component;
        private DicomServerConfigurationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DicomServerConfigurationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DicomServerConfigurationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
