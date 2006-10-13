using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("view", "worklist-contextmenu/View Patient")]
    [ClickHandler("view", "ViewPatient")]
    [EnabledStateObserver("view", "Enabled", "EnabledChanged")]
    //[Tooltip("edit", "Edit Patient Details")]
    //[IconSet("edit", IconScheme.Colour, "Icons.EditPatientMedium.png", "Icons.EditPatientMedium.png", "Icons.EditPatientMedium.png")]

    //[ButtonAction("edit", "global-menus/PatientAdminToolbar/Edit")]
    [MenuAction("edit", "worklist-contextmenu/Edit Patient")]
    [ClickHandler("edit", "EditPatient")]
    [EnabledStateObserver("edit", "Enabled", "EnabledChanged")]
    //[Tooltip("edit", "Edit Patient Details")]
    //[IconSet("edit", IconScheme.Colour, "Icons.EditPatientMedium.png", "Icons.EditPatientMedium.png", "Icons.EditPatientMedium.png")]

    [ExtensionOf(typeof(WorklistToolExtensionPoint))]
    public class PatientOverviewTool : Tool<IWorklistToolContext>
    {
        private Dictionary<long, IWorkspace> _openPatients = new Dictionary<long,IWorkspace>();


        public override void Initialize()
        {
            base.Initialize();

            this.Context.DefaultAction = ViewPatient;
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

        public void ViewPatient()
        {
            OpenPatient(false);
        }

        public void EditPatient()
        {
            OpenPatient(true);
        }

        private void OpenPatient(bool edit)
        {
            PatientProfile profile = this.Context.SelectedPatientProfile;

            // check for an already open editor
            if (_openPatients.ContainsKey(profile.OID))
            {
                // activate existing editor
                _openPatients[profile.OID].Activate();
            }
            else
            {
                IWorkspace workspace = ApplicationComponent.LaunchAsWorkspace(
                   this.Context.DesktopWindow,
                   new PatientEditorShellComponent(profile, edit),
                   string.Format("{0} - {1}", profile.Name.Format(), profile.MRN.Id),
                   PatientWorkspaceClosed);
                _openPatients[profile.OID] = workspace;
            }
        }

        private void PatientWorkspaceClosed(IApplicationComponent c)
        {
            PatientEditorShellComponent component = (PatientEditorShellComponent)c;
            PatientProfile profile = component.Subject;

            // remove from list of open editors
            _openPatients.Remove(profile.OID);
        }
    }
}
