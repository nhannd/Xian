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
    /// Provides a Windows Forms view onto <see cref="EmailAddressEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(EmailAddressEditorComponentViewExtensionPoint))]
    public class EmailAddressEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private EmailAddressEditorComponent _component;
        private EmailAddressEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (EmailAddressEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new EmailAddressEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
