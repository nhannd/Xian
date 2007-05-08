using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    /// <summary>
    /// Provides registration workflow related operations, including retrieving registration worklist, worklist preview, 
    /// patient search, cancel orders and check-in patients
    /// </summary>
    [ServiceContract]
    public interface IRegistrationWorkflowService
    {
        /// <summary>
        /// Search for patients
        /// </summary>
        /// <param name="request"><see cref="SearchPatientRequest"/></param>
        /// <returns><see cref="SearchPatientResponse"/></returns>
        [OperationContract]
        SearchPatientResponse SearchPatient(SearchPatientRequest request);

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
        /// Load all the patient data to populate a Patient detail/biography page
        /// </summary>
        /// <param name="request"><see cref="LoadWorklistPreviewRequest"/></param>
        /// <returns><see cref="LoadWorklistPreviewResponse"/></returns>
        [OperationContract]
        LoadPatientProfileForBiographyResponse LoadPatientProfileForBiography(LoadPatientProfileForBiographyRequest request);

        /// <summary>
        /// Get enablements for operations that can be applied to a worklist item
        /// </summary>
        /// <param name="request"><see cref="GetOperationEnablementRequest"/></param>
        /// <returns><see cref="GetOperationEnablementResponse"/></returns>
        [OperationContract]
        GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request);

        /// <summary>
        /// Get requested procedures that can be checked-in for a patient
        /// </summary>
        /// <param name="request"><see cref="GetDataForCheckInTableRequest"/></param>
        /// <returns><see cref="GetDataForCheckInTableResponse"/></returns>
        [OperationContract]
        GetDataForCheckInTableResponse GetDataForCheckInTable(GetDataForCheckInTableRequest request);

        /// <summary>
        /// Check in requested procedures for a patient
        /// </summary>
        /// <param name="request"><see cref="CheckInProcedureRequest"/></param>
        /// <returns><see cref="CheckInProcedureResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        CheckInProcedureResponse CheckInProcedure(CheckInProcedureRequest request);

        /// <summary>
        /// Loads all form data needed to search for a patient
        /// </summary>
        /// <param name="request"><see cref="LoadPatientSearchComponentFormDataRequest"/></param>
        /// <returns><see cref="LoadPatientSearchComponentFormDataResponse"/></returns>
        [OperationContract]
        LoadPatientSearchComponentFormDataResponse LoadPatientSearchComponentFormData(LoadPatientSearchComponentFormDataRequest request);

        /// <summary>
        /// Get orders that can be cancelled for a patient
        /// </summary>
        /// <param name="request"><see cref="GetDataForCancelOrderTableRequest"/></param>
        /// <returns><see cref="GetDataForCancelOrderTableResponse"/></returns>
        [OperationContract]
        GetDataForCancelOrderTableResponse GetDataForCancelOrderTable(GetDataForCancelOrderTableRequest request);

        /// <summary>
        /// Cancel orders with a cancellation reason for a patient
        /// </summary>
        /// <param name="request"><see cref="CancelOrderRequest"/></param>
        /// <returns><see cref="CancelOrderResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        CancelOrderResponse CancelOrder(CancelOrderRequest request);
    }
}
