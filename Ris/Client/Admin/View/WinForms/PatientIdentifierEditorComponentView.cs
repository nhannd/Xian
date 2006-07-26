using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    [ExtensionOf(typeof(PatientIdentifierEditorComponentViewExtensionPoint))]
    class PatientIdentifierEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private PatientIdentifierEditorControl _control;
        private PatientIdentifierEditorComponent _component;

        protected PatientIdentifierEditorControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new PatientIdentifierEditorControl(_component);
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
            _component = (PatientIdentifierEditorComponent)component;
        }

        #endregion

    }
}
