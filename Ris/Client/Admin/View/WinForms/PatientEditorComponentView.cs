using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    [ExtensionOf(typeof(PatientEditorComponentViewExtensionPoint))]
    public class PatientEditorComponentView : WinFormsView, IApplicationComponentView
    {
        private PatientEditorControl _control;
        private PatientEditorComponent _component;

        public PatientEditorComponentView()
        {
        }

        protected PatientEditorControl Control
        {
            get
            {
                if (_control == null)
                {
                    _control = new PatientEditorControl(_component);
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
            _component = (PatientEditorComponent)component;
        }

        #endregion
    }
}
