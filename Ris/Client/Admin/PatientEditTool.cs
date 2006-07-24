using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
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
            long oid = this.PatientAdminToolContext.SelectedPatient.OID;

            // reload the patient for 3 reasons:
            // 1. the existing copy in memory may be stale
            // 2. the editor should operate on a copy
            // 3. it may need patient details that were not loaded when the patients were listed
            IPatientAdminService service = ApplicationContext.GetService<IPatientAdminService>();
            Patient patient = service.LoadPatientDetails(oid);
            
            string title = string.Format("Edit Patient - {0}", patient.Name.Format());
            OpenPatient(title, patient);
        }

        protected override void SaveChanges(Patient patient)
        {
            IPatientAdminService service = ApplicationContext.GetService<IPatientAdminService>();
            service.UpdatePatient(patient);
        }
    }
}
