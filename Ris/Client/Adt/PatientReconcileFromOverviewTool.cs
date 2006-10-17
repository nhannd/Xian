using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt
{
    [MenuAction("apply", "global-menus/Patient/Reconcile...")]
    [ButtonAction("apply", "global-toolbars/Patient/Reconcile")]
    [Tooltip("apply", "Reconcile patient profiles")]
    [IconSet("apply", IconScheme.Colour, "Icons.PatientReconciliationToolSmall.png", "Icons.PatientReconciliationToolMedium.png", "Icons.PatientReconciliationToolLarge.png")]
    [ClickHandler("apply", "Apply")]

    [ExtensionOf(typeof(PatientOverviewToolExtensionPoint))]
    public class PatientReconcileFromOverviewTool : Tool<IPatientOverviewToolContext>
    {
        public void Apply()
        {
            PatientReconciliation.ShowReconciliationDialog(this.Context.SelectedProfile, this.Context.DesktopWindow);
        }
    }
}
