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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="AttachedDocumentPreviewComponent"/>
    /// </summary>
    [ExtensionOf(typeof(AttachedDocumentPreviewComponentViewExtensionPoint))]
    public class AttachedDocumentPreviewComponentView : WinFormsView, IApplicationComponentView
    {
        private AttachedDocumentPreviewComponent _component;
        private AttachedDocumentPreviewComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (AttachedDocumentPreviewComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new AttachedDocumentPreviewComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
