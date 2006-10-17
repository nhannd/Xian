using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
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
