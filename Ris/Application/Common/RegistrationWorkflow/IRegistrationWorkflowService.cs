using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    /// <summary>
    /// Provides data loading/saving for the <see cref="WorklistComponent"/>, <see cref="RegistrationPreviewComponent"/>, 
    /// <see cref="RequestedProcedureCheckInComponent"/> and <see cref="PatientSearchComponent"/>
    /// </summary>
    [ServiceContract]
    public interface IRegistrationWorkflowService
    {
        /// <summary>
        /// Search for a list of Patient
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        SearchPatientResponse SearchPatient(SearchPatientRequest request);

        /// <summary>
        /// Get the worklist for each folder in the <see cref="WorklistComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetWorklistResponse GetWorklist(GetWorklistRequest request);

        /// <summary>
        /// Load all the data for the <see cref="RegistrationPreviewComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadWorklistPreviewResponse LoadWorklistPreview(LoadWorklistPreviewRequest request);

        /// <summary>
        /// Get a list of operation enablement for tools that acts on the folders in the <see cref="WorklistComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetOperationEnablementResponse GetOperationEnablement(GetOperationEnablementRequest request);

        /// <summary>
        /// Loads all data for the <see cref="RequestedProcedureCheckInComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetDataForCheckInTableResponse GetDataForCheckInTable(GetDataForCheckInTableRequest request);

        /// <summary>
        /// Check in the Requested Procedures selected in the <see cref="RequestedProcedureCheckInComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        CheckInProcedureResponse CheckInProcedure(CheckInProcedureRequest request);

        /// <summary>
        /// Loads all form data for the <see cref="PatientSearchComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadPatientSearchComponentFormDataResponse LoadPatientSearchComponentFormData(LoadPatientSearchComponentFormDataRequest request);

        [OperationContract]
        GetDataForCancelOrderTableResponse GetDataForCancelOrderTable(GetDataForCancelOrderTableRequest request);

        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        CancelOrderResponse CancelOrder(CancelOrderRequest request);
    }
}
