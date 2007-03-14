using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [ServiceContract]
    public interface IReportingWorkflowService
    {
        [OperationContract]
        GetWorklistResponse GetWorklist(GetWorklistRequest request);

        [OperationContract(IsOneWay=true)]
        void ScheduleInterpretation(ScheduleInterpretationRequest request);

        [OperationContract(IsOneWay = true)]
        void ClaimInterpretation(ClaimInterpretationRequest request);

        [OperationContract(IsOneWay = true)]
        void StartInterpretation(StartInterpretationRequest request);

        [OperationContract(IsOneWay = true)]
        void CompleteInterpretationForTranscription(CompleteInterpretationForTranscriptionRequest request);

        [OperationContract(IsOneWay = true)]
        void CompleteInterpretationForVerification(CompleteInterpretationForVerificationRequest request);

        [OperationContract(IsOneWay = true)]
        void CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request);

        [OperationContract(IsOneWay = true)]
        void CancelPendingTranscription(CancelPendingTranscriptionRequest request);

        [OperationContract(IsOneWay = true)]
        void StartVerification(StartVerificationRequest request);

        [OperationContract(IsOneWay = true)]
        void CompleteVerification(CompleteVerificationRequest request);
    }
}
