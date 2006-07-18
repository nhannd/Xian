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
            PatientEditorComponent editor = new PatientEditorComponent();
            editor.Subject = patient;

            ApplicationComponent.LaunchAsWorkspace(editor, title, PatientEditorExited);
        }

        protected abstract void PatientEditorExited(IApplicationComponent component);

        protected IPatientAdminToolContext PatientAdminToolContext
        {
            get { return (IPatientAdminToolContext)this.Context; }
        }
    }
}
