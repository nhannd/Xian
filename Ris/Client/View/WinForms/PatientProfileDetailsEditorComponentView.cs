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
    [ExtensionOf(typeof(PatientEditorComponentViewExtensionPoint))]
    public class PatientProfileDetailsEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private PatientProfileDetailsEditorControl _control;
        private PatientProfileDetailsEditorComponent _component;

        public PatientProfileDetailsEditorComponentView()
        {
        }

        protected PatientProfileDetailsEditorControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new PatientProfileDetailsEditorControl(_component);
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
            _component = (PatientProfileDetailsEditorComponent)component;
        }

        #endregion
    }
}