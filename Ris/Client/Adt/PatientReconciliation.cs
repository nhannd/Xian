using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Services;
using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    public static class PatientReconciliation
    {
        public static void ShowReconciliationDialog(EntityRef<PatientProfile> targetProfile, IDesktopWindow window)
        {
            if (targetProfile != null)
            {
                IAdtService service = ApplicationContext.GetService<IAdtService>();

                IList<PatientProfileMatch> matches = service.FindPatientReconciliationMatches(targetProfile);

                if (matches.Count > 0)
                {
                    // load the target patient and all profiles
                    Patient targetPatient = service.LoadPatientAndAllProfiles(targetProfile);

                    // find the target profile
                    FunctionalList<PatientProfile> allProfiles = new FunctionalList<PatientProfile>(targetPatient.Profiles);
                    PatientProfile profile = allProfiles.SelectFirst(
                        delegate(PatientProfile p) { return targetProfile.RefersTo(p); });

                    ReconciliationComponent component = new ReconciliationComponent(profile, matches);
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
