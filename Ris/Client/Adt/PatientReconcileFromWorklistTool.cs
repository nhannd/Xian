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
    [MenuAction("apply", "global-menus/Patient/Reconcile...")]
    [ButtonAction("apply", "global-toolbars/Patient/Reconcile")]
    [MenuAction("apply", "worklist-contextmenu/Reconcile")]
    [ButtonAction("apply", "worklist-toolbar/Reconcile")]
    [Tooltip("apply", "Reconcile patient profiles")]
    [IconSet("apply", IconScheme.Colour, "Icons.PatientReconciliationToolSmall.png", "Icons.PatientReconciliationToolMedium.png", "Icons.PatientReconciliationToolLarge.png")]
    [ClickHandler("apply", "Apply")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

    [ExtensionOf(typeof(WorklistToolExtensionPoint))]
    public class PatientReconcileFromWorklistTool : PatientWorklistTool
    {
        /// <summary>
        /// Default constructor.  A no-args constructor is required by the
        /// framework.  Do not remove.
        /// </summary>
        public PatientReconcileFromWorklistTool()
        {
        }

        /// <summary>
        /// Called by the framework when the user clicks the "apply" menu item or toolbar button.
        /// </summary>
        public void Apply()
        {
            PatientReconciliation.ShowReconciliationDialog(this.Context.SelectedPatientProfile, this.Context.DesktopWindow);
        }
    }
}
