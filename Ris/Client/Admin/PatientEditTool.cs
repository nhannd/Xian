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
    [ButtonAction("edit", "PatientAdminToolbar/Edit")]
    [ClickHandler("edit", "EditPatient")]
    [EnabledStateObserver("edit", "Enabled", "EnabledChanged")]
    [Tooltip("edit", "Edit Patient")]

    [ExtensionOf(typeof(PatientAdminToolExtensionPoint))]
    public class PatientEditTool : PatientAddEditToolBase
    {
        public override void Initialize()
        {
            base.Initialize();

            this.PatientAdminToolContext.DefaultActionHandler = EditPatient;
        }

        public bool Enabled
        {
            get { return this.PatientAdminToolContext.SelectedPatient != null; }
        }

        public event EventHandler EnabledChanged
        {
            add { this.PatientAdminToolContext.SelectedPatientChanged += value; }
            remove { this.PatientAdminToolContext.SelectedPatientChanged -= value; }
        }

        public void EditPatient()
        {
            Patient patient = this.PatientAdminToolContext.SelectedPatient;
            if (patient != null) // should never be null, but just in case
            {
                string title = string.Format("Edit Patient - {0}, {1}", patient.PatientId, patient.PrimaryName);
                OpenPatient(title, patient);
            }
        }
    }
}
