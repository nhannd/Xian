using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.PatientReconcilliation;

namespace ClearCanvas.Ris.Client.Adt
{
    public static class PatientReconciliation
    {
        public static void ShowReconciliationDialog(EntityRef targetProfile, IDesktopWindow window)
        {
            if (targetProfile != null)
            {
                try
                {
                    IList<PatientProfileMatch> matches = null;
                    IList<PatientProfileSummary> reconciledProfiles = null;

                    Platform.GetService<IPatientReconciliationService>(
                        delegate(IPatientReconciliationService service)
                        {
                            ListPatientReconciliationMatchesResponse response =
                                service.ListPatientReconciliationMatches(new ListPatientReconciliationMatchesRequest(targetProfile));

                            matches = response.CandidateMatches;
                            reconciledProfiles = response.ReconciledProfiles;
                        });

                    if (matches.Count > 0)
                    {
                        ReconciliationComponent component = new ReconciliationComponent(targetProfile, reconciledProfiles, matches);
                        ApplicationComponent.LaunchAsDialog(
                            window,
                            component,
                            SR.TitlePatientReconciliation);
                    }
                    else
                    {
                        window.ShowMessageBox(SR.MessageNoReconciliationCandidate, MessageBoxActions.Ok);
                    }

                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, window);
                }
            }
        }
    }
}
