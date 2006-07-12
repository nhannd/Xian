using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    [ExtensionOf(typeof(PatientAdminComponentViewExtensionPoint))]
    public class PatientAdminComponentView : WinFormsView, IApplicationComponentView
    {
        private PatientAdminControl _control;
        private PatientAdminComponent _component;

        public void SetComponent(IApplicationComponent component)
        {
            _component = (PatientAdminComponent)component;
        }

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PatientAdminControl(_component);
                }
                return _control;
            }
        }
    }
}
