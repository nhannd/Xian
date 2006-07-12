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
            ApplicationComponent.LaunchAsWorkspace(
                new PatientEditorComponent(),
                title,
                PatientEditorExited);
        }

        protected void PatientEditorExited(IApplicationComponent component)
        {
            if (component.ExitCode == ApplicationComponentExitCode.Normal)
            {
                PatientEditorComponent patientEditor = (PatientEditorComponent)component;

                // TODO use service to save patient
            }
        }

        protected IPatientAdminToolContext PatientAdminToolContext
        {
            get { return (IPatientAdminToolContext)this.Context; }
        }
    }
}
