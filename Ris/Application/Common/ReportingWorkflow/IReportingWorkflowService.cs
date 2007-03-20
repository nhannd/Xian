using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [ServiceContract]
    public interface IReportingWorkflowService
    {
        [OperationContract]
        GetWorklistResponse GetWorklist(GetWorklistRequest request);

        [OperationContract]
        void ScheduleInterpretation(ScheduleInterpretationRequest request);

        [OperationContract]
        void ClaimInterpretation(ClaimInterpretationRequest request);

        [OperationContract]
        void StartInterpretation(StartInterpretationRequest request);

        [OperationContract]
        void CompleteInterpretationForTranscription(CompleteInterpretationForTranscriptionRequest request);

        [OperationContract]
        void CompleteInterpretationForVerification(CompleteInterpretationForVerificationRequest request);

        [OperationContract]
        void CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request);

        [OperationContract]
        void CancelPendingTranscription(CancelPendingTranscriptionRequest request);

        [OperationContract]
        void StartVerification(StartVerificationRequest request);

        [OperationContract]
        void CompleteVerification(CompleteVerificationRequest request);
    }
}
