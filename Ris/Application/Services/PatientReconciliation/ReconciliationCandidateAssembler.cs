using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Ris.Application.Common.PatientReconciliation;
using ClearCanvas.Healthcare.PatientReconciliation;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Application.Services.PatientReconciliation
{
    public class ReconciliationCandidateAssembler
    {
        public ReconciliationCandidate CreateReconciliationCandidate(PatientProfileMatch profileMatch, IPersistenceContext context)
        {
            ReconciliationCandidate rc = new ReconciliationCandidate();

            PatientProfileAssembler profileAssembler = new PatientProfileAssembler();
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
