using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Services
{
    public interface IReportingWorkflowService : IHealthcareServiceLayer
    {
        IList<ReportingWorklistQueryResult> GetWorklist(Type stepClass, ReportingProcedureStepSearchCriteria criteria);

        void ScheduleInterpretation(EntityRef<RequestedProcedure> procedure);

        void ClaimInterpretation(EntityRef<InterpretationStep> step);
        void StartInterpretation(EntityRef<InterpretationStep> step);
        void CompleteInterpretationForTranscription(EntityRef<InterpretationStep> step);
        void CompleteInterpretationForVerification(EntityRef<InterpretationStep> step);
        void CompleteInterpretationAndVerify(EntityRef<InterpretationStep> step);

        void CancelPendingTranscription(EntityRef<TranscriptionStep> step);

        void StartVerification(EntityRef<VerificationStep> step);
        void CompleteVerification(EntityRef<VerificationStep> step);
    }
}
