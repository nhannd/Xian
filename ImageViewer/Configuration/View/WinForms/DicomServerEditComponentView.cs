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
using ClearCanvas.ImageViewer.Configuration.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DicomServerEditComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DicomServerEditComponentViewExtensionPoint))]
    public class DicomServerEditComponentView : WinFormsView, IApplicationComponentView
    {
        private DicomServerEditComponent _component;
        private DicomServerEditComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DicomServerEditComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DicomServerEditComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
