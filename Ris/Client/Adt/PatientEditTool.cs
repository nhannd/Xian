using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{

    [ExtensionOf(typeof(WorklistToolExtensionPoint))]
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
        }

        public bool Enabled
        {
            get { return this.Context.SelectedPatientProfile != null; }
        }

        public event EventHandler EnabledChanged
        {
            add { this.Context.SelectedPatientProfileChanged += value; }
            remove { this.Context.SelectedPatientProfileChanged -= value; }
        }

        public void EditPatient()
        {
            long oid = this.Context.SelectedPatientProfile.OID;

            // check for an already open editor
            if (_openEditors.ContainsKey(oid))
            {
                // activate existing editor
                _openEditors[oid].Activate();
            }
            else
            {
                // reload the patient for 3 reasons:
                // 1. the existing copy in memory may be stale
                // 2. the editor should operate on a copy
                // 3. it may need patient details that were not loaded when the patients were listed
                IAdtService service = ApplicationContext.GetService<IAdtService>();
                PatientProfile profile = service.LoadPatientProfile(oid, true);

                // create a new editing workspace
                string title = string.Format("Edit Patient - {0} {1}", profile.Name.Format(), profile.MRN.Format());
                IWorkspace workspace = OpenPatient(title, profile);
                _openEditors[oid] = workspace;
            }
        }

        protected IWorklistToolContext Context
        {
            get { return (IWorklistToolContext)this.ContextBase; }
        }

        protected override IDesktopWindow DesktopWindow
        {
            get
            { 
                return this.Context.DesktopWindow;
            }
        }

        protected override void EditorClosed(PatientProfile profile, ApplicationComponentExitCode exitCode)
        {
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                try
                {
                    IAdtService service = ApplicationContext.GetService<IAdtService>();
                    service.UpdatePatientProfile(profile);
                }
                catch (ConcurrentModificationException)
                {
                    Platform.ShowMessageBox("The patient was modified by another user.  Your changes could not be saved.");
                }
            }

            // remove from list of open editors
            _openEditors.Remove(profile.OID);
        }
    }
}
