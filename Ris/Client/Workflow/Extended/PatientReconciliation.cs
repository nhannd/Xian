#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Extended.Common.PatientReconciliation;

namespace ClearCanvas.Ris.Client.Workflow.Extended
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
                return exitCode == ApplicationComponentExitCode.Accepted;
            }
            else
            {
                window.ShowMessageBox(SR.MessageNoReconciliationCandidate, MessageBoxActions.Ok);
                return false;
            }
        }
    }
}
