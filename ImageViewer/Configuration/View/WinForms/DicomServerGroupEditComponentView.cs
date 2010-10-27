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
using ClearCanvas.ImageViewer.Configuration.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="DicomServerGroupEditComponent"/>
    /// </summary>
    [ExtensionOf(typeof(DicomServerGroupEditComponentViewExtensionPoint))]
    public class DicomServerGroupEditComponentView : WinFormsView, IApplicationComponentView
    {
        private DicomServerGroupEditComponent _component;
        private DicomServerGroupEditComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (DicomServerGroupEditComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new DicomServerGroupEditComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
