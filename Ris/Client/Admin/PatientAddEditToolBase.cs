using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    public abstract class PatientAddEditToolBase : Tool
    {
        private PatientEditorComponent _patientEditor;
        private AddressesEditorComponent _addressesEditor;
        private PhoneNumbersEditorComponent _phoneNumbersEditor;

        protected void OpenPatient(string title, Patient patient)
        {
            NavigatorComponent navigator = new NavigatorComponent();

            _patientEditor = new PatientEditorComponent();
            _patientEditor.Subject = patient;

            _addressesEditor = new AddressesEditorComponent();
            _phoneNumbersEditor = new PhoneNumbersEditorComponent();

            navigator.Nodes.Add(new NavigatorNode("Patient", _patientEditor));
            navigator.Nodes.Add(new NavigatorNode("Patient/Addresses", _addressesEditor));
            navigator.Nodes.Add(new NavigatorNode("Patient/Phone Numbers", _phoneNumbersEditor));

            ApplicationComponent.LaunchAsWorkspace(navigator, title, PatientEditorExited);
            //ApplicationComponentExitCode code = ApplicationComponent.LaunchAsDialog(navigator, title);
        }

        private void PatientEditorExited(IApplicationComponent component)
        {
            if (component.ExitCode == ApplicationComponentExitCode.Normal)
            {
                SaveChanges(_patientEditor.Subject);
            }
        }

        protected abstract void SaveChanges(Patient patient);

        protected IPatientAdminToolContext PatientAdminToolContext
        {
            get { return (IPatientAdminToolContext)this.Context; }
        }
    }
}
