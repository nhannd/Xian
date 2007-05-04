using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.PatientReconciliation;

namespace ClearCanvas.Ris.Client.Adt
{
    public static class PatientReconciliation
    {
        public static bool ShowReconciliationDialog(EntityRef targetProfile, IDesktopWindow window)
        {
            IList<ReconciliationCandidate> candidates = null;
            IList<PatientProfileSummary> reconciledProfiles = null;

            Platform.GetService<IPatientReconciliationService>(
                delegate(IPatientReconciliationService service)
                {
                    ListPatientReconciliationMatchesResponse response =
                        service.ListPatientReconciliationMatches(new ListPatientReconciliationMatchesRequest(targetProfile));

                    candidates = response.MatchCandidates;
                    reconciledProfiles = response.ReconciledProfiles;
                });

            if (candidates.Count > 0)
            {
                ReconciliationComponent component = new ReconciliationComponent(targetProfile, reconciledProfiles, candidates);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    window,
                    component,
                    SR.TitlePatientReconciliation);
                return exitCode == ApplicationComponentExitCode.Normal;
            }
            else
            {
                window.ShowMessageBox(SR.MessageNoReconciliationCandidate, MessageBoxActions.Ok);
                return false;
            }
        }
    }
}
