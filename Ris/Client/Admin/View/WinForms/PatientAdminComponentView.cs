using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    [ExtensionOf(typeof(PatientAdminComponentViewExtensionPoint))]
    public class PatientAdminComponentView : ApplicationComponentView
    {
        private PatientAdminControl _control;

        public override object GuiElement
        {
            get
            {
                if (_control == null)
                {
                    _control = new PatientAdminControl();
                }
                return _control;
            }
        }
    }
}
