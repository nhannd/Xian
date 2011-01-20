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

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    [ExtensionOf(typeof(DicomExplorerConfigurationComponentViewExtensionPoint))]
    public class DicomExplorerConfigurationComponentView : WinFormsView, IApplicationComponentView
    {
        private DicomExplorerConfigurationComponent _component;
        private DicomExplorerConfigurationComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DicomExplorerConfigurationComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DicomExplorerConfigurationComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
