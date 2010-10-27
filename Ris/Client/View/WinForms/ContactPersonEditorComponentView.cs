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
    /// Provides a Windows Forms view onto <see cref="ContactPersonEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ContactPersonEditorComponentViewExtensionPoint))]
    public class ContactPersonEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private ContactPersonEditorComponent _component;
        private ContactPersonEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ContactPersonEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ContactPersonEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
