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
        protected void OpenPatient(string title, Patient patient)
        {
            NavigatorComponent navigator = new NavigatorComponent();

            PatientEditorComponent patientEditor = new PatientEditorComponent();
            patientEditor.Subject = patient;

            AddressesSummaryComponent addressesSummary = new AddressesSummaryComponent();
            addressesSummary.Subject = patient;
            PhoneNumbersSummaryComponent phoneNumbersSummary = new PhoneNumbersSummaryComponent();
            phoneNumbersSummary.Subject = patient;

            navigator.Nodes.Add(new NavigatorNode("Patient", patientEditor));
            navigator.Nodes.Add(new NavigatorNode("Patient/Addresses", addressesSummary));
            navigator.Nodes.Add(new NavigatorNode("Patient/Phone Numbers", phoneNumbersSummary));

            ApplicationComponent.LaunchAsWorkspace(navigator, title, PatientEditorExited);
        }

        private void PatientEditorExited(IApplicationComponent component)
        {
            if (component.ExitCode == ApplicationComponentExitCode.Normal)
            {
                NavigatorComponent navigator = (NavigatorComponent)component;
                PatientEditorComponent patientEditor = (PatientEditorComponent)navigator.Nodes[0].Component;
                SaveChanges(patientEditor.Subject);
            }
        }

        protected abstract void SaveChanges(Patient patient);

        protected IPatientAdminToolContext PatientAdminToolContext
        {
            get { return (IPatientAdminToolContext)this.Context; }
        }
    }
}
