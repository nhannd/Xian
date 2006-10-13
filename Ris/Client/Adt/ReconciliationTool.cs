using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Adt
{
    //[MenuAction("apply", "global-menus/Patient/Reconcile...")]
    [MenuAction("apply", "worklist-contextmenu/Reconcile")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    //[ButtonAction("apply", "global-toolbars/Patient/PatientReconciliationTool")]
    //[Tooltip("apply", "Place tooltip text here")]
    //[IconSet("apply", IconScheme.Colour, "Icons.PatientReconciliationToolSmall.png", "Icons.PatientReconciliationToolMedium.png", "Icons.PatientReconciliationToolLarge.png")]
    [ClickHandler("apply", "Apply")]

    [ExtensionOf(typeof(WorklistToolExtensionPoint))]
    public class ReconciliationTool : Tool<IWorklistToolContext>
    {
        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public ReconciliationTool()
        {
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

        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
            PatientProfile targetProfile = this.Context.SelectedPatientProfile;
            if (targetProfile != null)
            {
                IAdtService service = ApplicationContext.GetService<IAdtService>();
                IList<PatientProfileMatch> matches = service.FindPatientReconciliationMatches(targetProfile);

                if (matches.Count > 0)
                {
                    ReconciliationComponent component = new ReconciliationComponent(targetProfile, matches);
                    ApplicationComponent.LaunchAsDialog(
                        this.Context.DesktopWindow,
                        component,
                        "Patient Reconciliation");
                }
                else
                {
                    Platform.ShowMessageBox("There are no reconciliation candidates for this patient.");
                }
            }
        }
    }
}
