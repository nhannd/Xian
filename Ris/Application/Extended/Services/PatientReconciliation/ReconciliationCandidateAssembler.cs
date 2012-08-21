#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Healthcare.PatientReconciliation;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Extended.Common.PatientReconciliation;
using ClearCanvas.Ris.Application.Services;

namespace ClearCanvas.Ris.Application.Extended.Services.PatientReconciliation
{
    public class ReconciliationCandidateAssembler
    {
        public ReconciliationCandidate CreateReconciliationCandidate(PatientProfileMatch profileMatch, IPersistenceContext context)
        {
            var rc = new ReconciliationCandidate();

            var profileAssembler = new PatientProfileAssembler();
            rc.PatientProfile = profileAssembler.CreatePatientProfileSummary(profileMatch.PatientProfile, context);
            switch (profileMatch.Score)
            {
                case PatientProfileMatch.ScoreValue.High:
                    rc.Score = ReconciliationCandidate.ProbabilityScore.High;
                    break;
                case PatientProfileMatch.ScoreValue.Moderate:
                    rc.Score = ReconciliationCandidate.ProbabilityScore.Moderate;
                    break;
                case PatientProfileMatch.ScoreValue.Low:
                    rc.Score = ReconciliationCandidate.ProbabilityScore.Low;
                    break;
            }
            return rc;
        }
    }
}
