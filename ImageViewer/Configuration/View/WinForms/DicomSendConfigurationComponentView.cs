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
    /// Provides a Windows Forms view onto <see cref="DicomSendConfigurationComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DicomSendConfigurationComponentViewExtensionPoint))]
    public class DicomSendConfigurationComponentView : WinFormsView, IApplicationComponentView
    {
        private DicomSendConfigurationComponent _component;
        private DicomSendConfigurationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DicomSendConfigurationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DicomSendConfigurationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
