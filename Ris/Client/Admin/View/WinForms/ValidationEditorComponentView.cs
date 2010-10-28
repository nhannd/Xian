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

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms view onto <see cref="ValidationEditorComponent"/>
    /// </summary>
    [ExtensionOf(typeof(ValidationEditorComponentViewExtensionPoint))]
    public class ValidationEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private ValidationEditorComponent _component;
        private ValidationEditorComponentControl _control;


        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (ValidationEditorComponent)component;
        }

        #endregion

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new ValidationEditorComponentControl(_component);
                }
                return _control;
            }
        }
    }
}
