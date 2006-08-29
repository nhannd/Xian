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
    [IconSet("edit", IconScheme.Colour, "Icons.EditPatientMedium.png", "Icons.EditPatientMedium.png", "Icons.EditPatientMedium.png")]

    [ExtensionOf(typeof(PatientAdminToolExtensionPoint))]
    public class PatientEditTool : PatientAddEditToolBase
    {
        private Dictionary<long, IWorkspace> _openEditors;

        public PatientEditTool()
        {
            _openEditors = new Dictionary<long, IWorkspace>();
        }

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

            // check for an already open editor
            if (_openEditors.ContainsKey(oid))
            {
                // activate existing editor
                _openEditors[oid].IsActivated = true;
            }
            else
            {
                // reload the patient for 3 reasons:
                // 1. the existing copy in memory may be stale
                // 2. the editor should operate on a copy
                // 3. it may need patient details that were not loaded when the patients were listed
                IPatientAdminService service = ApplicationContext.GetService<IPatientAdminService>();
                PatientProfile patient = service.LoadPatientDetails(oid);

                // create a new editing workspace
                string title = string.Format("Edit Patient - {0}", patient.Name.Format());
                IWorkspace workspace = OpenPatient(title, patient);
                _openEditors[oid] = workspace;
            }
        }

        protected override void EditorClosed(PatientProfile patient, ApplicationComponentExitCode exitCode)
        {
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                try
                {
                    IPatientAdminService service = ApplicationContext.GetService<IPatientAdminService>();
                    service.UpdatePatient(patient);
                }
                catch (ConcurrentModificationException)
                {
                    Platform.ShowMessageBox("The patient was modified by another user.  Your changes could not be saved.");
                }
            }

            // remove from list of open editors
            _openEditors.Remove(patient.OID);
        }
    }
}
