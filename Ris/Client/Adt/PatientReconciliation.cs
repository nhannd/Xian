using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Services;
using ClearCanvas.Common;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Adt
{
    public static class PatientReconciliation
    {
        public static void ShowReconciliationDialog(PatientProfile targetProfile, IDesktopWindow window)
        {
            if (targetProfile != null)
            {
                IAdtService service = ApplicationContext.GetService<IAdtService>();
                IList<PatientProfileMatch> matches = service.FindPatientReconciliationMatches(targetProfile);

                if (matches.Count > 0)
                {
                    ReconciliationComponent component = new ReconciliationComponent(targetProfile, matches);
                    ApplicationComponent.LaunchAsDialog(
                        window,
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
