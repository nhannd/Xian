using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [ServiceContract]
    public interface IReportingWorkflowService
    {
        /// <summary>
        /// Get items for a worklist
        /// </summary>
        /// <param name="request"><see cref="GetWorklistRequest"/></param>
        /// <returns><see cref="GetWorklistResponse"/></returns>
        [OperationContract]
        GetWorklistResponse GetWorklist(GetWorklistRequest request);

        /// <summary>
        /// Get item count for a worklist
        /// </summary>
        /// <param name="request"><see cref="GetWorklistCountRequest"/></param>
        /// <returns><see cref="GetWorklistCountResponse"/></returns>
        [OperationContract]
        GetWorklistCountResponse GetWorklistCount(GetWorklistCountRequest request);

        /// <summary>
        /// Load all the data to populate the preview page for a worklist item
        /// </summary>
        /// <param name="request"><see cref="LoadWorklistPreviewRequest"/></param>
        /// <returns><see cref="LoadWorklistPreviewResponse"/></returns>
        [OperationContract]
        LoadWorklistPreviewResponse LoadWorklistPreview(LoadWorklistPreviewRequest request);

        /// <summary>
        /// Get enablements for operations that can be applied to a worklist item
        /// </summary>
        /// <param name="request"><see cref="GetOperationEnablementRequest"/></param>
        /// <returns><see cref="GetOperationEnablementResponse"/></returns>
        [OperationContract]
        GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request);

        /// <summary>
        /// Claim an interpretation step
        /// </summary>
        /// <param name="request"><see cref="ClaimInterpretationRequest"/></param>
        /// <returns><see cref="ClaimInterpretationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        ClaimInterpretationResponse ClaimInterpretation(ClaimInterpretationRequest request);

        /// <summary>
        /// Start an interpretation step
        /// </summary>
        /// <param name="request"><see cref="StartInterpretationRequest"/></param>
        /// <returns><see cref="StartInterpretationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        StartInterpretationResponse StartInterpretation(StartInterpretationRequest request);

        /// <summary>
        /// Complete an interpretation step and create a transcription step.
        /// </summary>
        /// <param name="request"><see cref="CompleteInterpretationForTranscriptionRequest"/></param>
        /// <returns><see cref="CompleteInterpretationForTranscriptionResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        CompleteInterpretationForTranscriptionResponse CompleteInterpretationForTranscription(CompleteInterpretationForTranscriptionRequest request);

        /// <summary>
        /// Complete an interpretation step and create a verification step.
        /// </summary>
        /// <param name="request"><see cref="CompleteInterpretationForVerificationRequest"/></param>
        /// <returns><see cref="CompleteInterpretationForVerificationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        CompleteInterpretationForVerificationResponse CompleteInterpretationForVerification(CompleteInterpretationForVerificationRequest request);

        /// <summary>
        /// Complete an interpretation step and and verify it.
        /// </summary>
        /// <param name="request"><see cref="CompleteInterpretationAndVerifyRequest"/></param>
        /// <returns><see cref="CompleteInterpretationAndVerifyResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        CompleteInterpretationAndVerifyResponse CompleteInterpretationAndVerify(CompleteInterpretationAndVerifyRequest request);

        /// <summary>
        /// Cancel an interpretation step and and verify it.
        /// </summary>
        /// <param name="request"><see cref="CompleteInterpretationAndVerifyRequest"/></param>
        /// <returns><see cref="CompleteInterpretationAndVerifyResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        CancelPendingTranscriptionResponse CancelPendingTranscription(CancelPendingTranscriptionRequest request);

        /// <summary>
        /// Start an verification step
        /// </summary>
        /// <param name="request"><see cref="StartVerificationRequest"/></param>
        /// <returns><see cref="StartVerificationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        StartVerificationResponse StartVerification(StartVerificationRequest request);

        /// <summary>
        /// Complete a verification step
        /// </summary>
        /// <param name="request"><see cref="CompleteVerificationRequest"/></param>
        /// <returns><see cref="CompleteVerificationResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        CompleteVerificationResponse CompleteVerification(CompleteVerificationRequest request);

        /// <summary>
        /// Start an addendum step
        /// </summary>
        /// <param name="request"><see cref="StartAddendumRequest"/></param>
        /// <returns><see cref="StartAddendumResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        CreateAddendumResponse CreateAddendum(CreateAddendumRequest request);

        /// <summary>
        /// Load the report of a given reporting step
        /// </summary>
        /// <param name="request"><see cref="LoadReportForEditRequest"/></param>
        /// <returns><see cref="LoadReportForEditResponse"/></returns>
        [OperationContract]
        LoadReportForEditResponse LoadReportForEdit(LoadReportForEditRequest request);

        /// <summary>
        /// Save the report of a given reporting step
        /// </summary>
        /// <param name="request"><see cref="SaveReportRequest"/></param>
        /// <returns><see cref="SaveReportResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        SaveReportResponse SaveReport(SaveReportRequest request);

        /// <summary>
        /// Get all the reports of a given patient
        /// </summary>
        /// <param name="request"><see cref="GetPriorReportRequest"/></param>
        /// <returns><see cref="GetPriorReportResponse"/></returns>
        [OperationContract]
        GetPriorReportResponse GetPriorReport(GetPriorReportRequest request);
    }
}
