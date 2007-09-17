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
        /// <param name="request"><see cref="SearchRequest"/></param>
        /// <returns><see cref="SearchResponse"/></returns>
        [OperationContract]
        SearchResponse Search(SearchRequest request);

        [OperationContract]
        ListWorklistsResponse ListWorklists(ListWorklistsRequest request);

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
