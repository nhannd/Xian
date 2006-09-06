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
    public abstract class PatientAddEditToolBase : ToolBase
    {
        protected IWorkspace OpenPatient(string title, PatientProfile patient)
        {
            NavigatorComponentContainer navigator = new NavigatorComponentContainer();

            PatientEditorComponent patientEditor = new PatientEditorComponent();
            patientEditor.Subject = patient;

            AddressesSummaryComponent addressesSummary = new AddressesSummaryComponent();
            addressesSummary.Subject = patient;
            PhoneNumbersSummaryComponent phoneNumbersSummary = new PhoneNumbersSummaryComponent();
            phoneNumbersSummary.Subject = patient;

            navigator.Pages.Add(new NavigatorPage("Patient", patientEditor));
            navigator.Pages.Add(new NavigatorPage("Patient/Addresses", addressesSummary));
            navigator.Pages.Add(new NavigatorPage("Patient/Phone Numbers", phoneNumbersSummary));

            return ApplicationComponent.LaunchAsWorkspace(
                this.DesktopWindow, navigator, title, PatientEditorExited);
        }

        private void PatientEditorExited(IApplicationComponent component)
        {
            NavigatorComponentContainer navigator = (NavigatorComponentContainer)component;
            PatientEditorComponent patientEditor = (PatientEditorComponent)navigator.Pages[0].Component;
            EditorClosed(patientEditor.Subject, component.ExitCode);
        }

        protected abstract IDesktopWindow DesktopWindow { get; }

        protected abstract void EditorClosed(PatientProfile patient, ApplicationComponentExitCode exitCode);
    }
}
