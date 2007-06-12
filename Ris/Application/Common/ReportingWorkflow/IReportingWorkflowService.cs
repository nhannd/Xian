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
        GetWorklistCountResponse GetWorklistCount(GetWorklistCountRequest request);

        [OperationContract]
        GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request);

        [OperationContract]
        ClaimInterpretationResponse ClaimInterpretation(ClaimInterpretationRequest request);

        [OperationContract]
        StartInterpretationResponse StartInterpretation(StartInterpretationRequest request);

        [OperationContract]
        EditInterpretationResponse EditInterpretation(EditInterpretationRequest request);

        [OperationContract]
        CompleteInterpretationForTranscriptionResponse CompleteInterpretationForTranscription(CompleteInterpretationForTranscriptionRequest request);

        [OperationContract]
        CompleteInterpretationForVerificationResponse CompleteInterpretationForVerification(CompleteInterpretationForVerificationRequest request);

        [OperationContract]
        CompleteInterpretationAndVerifyResponse CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request);

        [OperationContract]
        CancelPendingTranscriptionResponse CancelPendingTranscription(CancelPendingTranscriptionRequest request);

        [OperationContract]
        StartVerificationResponse StartVerification(StartVerificationRequest request);

        [OperationContract]
        EditVerificationResponse EditVerification(EditVerificationRequest request);

        [OperationContract]
        CompleteVerificationResponse CompleteVerification(CompleteVerificationRequest request);

        [OperationContract]
        SaveReportResponse SaveReport(SaveReportRequest request);

    }
}
