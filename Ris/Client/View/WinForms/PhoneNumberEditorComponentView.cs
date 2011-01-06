#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
    [ExtensionOf(typeof(PhoneNumbersEditorComponentViewExtensionPoint))]
    class PhoneNumberEditorComponentView : WinFormsView, IApplicationComponentView
    {
        PhoneNumberEditorComponent _component;
        PhoneNumberEditorControl _control;

        protected PhoneNumberEditorControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new PhoneNumberEditorControl(_component);
                }
                return _control;
            }
        }

        public override object GuiElement
        {
            get { return this.Control; }
        }

        #region IApplicationComponentView Members

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PhoneNumberEditorComponent)component;
        }

        #endregion
    }
}
