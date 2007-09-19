using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    /// <summary>
    /// Provides data loading/saving for the <see cref="OrderListComponent"/> and <see cref="OrderEntryComponent"/>
    /// </summary>
    [ServiceContract]
    public interface IOrderEntryService
    {
        /// <summary>
        /// Search for all active visits for a patient
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListActiveVisitsForPatientResponse ListActiveVisitsForPatient(ListActiveVisitsForPatientRequest request);

        /// <summary>
        /// Loads all order entry form data for the <see cref="OrderEntryComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetOrderEntryFormDataResponse GetOrderEntryFormData(GetOrderEntryFormDataRequest request);

        /// <summary>
        /// Loads data for the Diagnostic Service breakdown for the <see cref="OrderEntryComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadDiagnosticServiceBreakdownResponse LoadDiagnosticServiceBreakdown(LoadDiagnosticServiceBreakdownRequest request);

        /// <summary>
        /// Place a new order via the information provided by the <see cref="OrderEntryComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        PlaceOrderResponse PlaceOrder(PlaceOrderRequest request);

        /// <summary>
        /// List all the orders based on the Assigning Authority for the <see cref="OrderListComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        GetOrdersWorkListResponse GetOrdersWorkList(GetOrdersWorkListRequest request);

        /// <summary>
        /// Get all the orders for a patient
        /// </summary>
        /// <param name="request"><see cref="ListOrderForPatientRequest"/></param>
        /// <returns><see cref="ListOrderForPatientResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        ListOrdersForPatientResponse ListOrdersForPatient(ListOrdersForPatientRequest request);

        /// <summary>
        /// Get all the orders for a patient
        /// </summary>
        /// <param name="request"><see cref="LoadOrderDetailRequest"/></param>
        /// <returns><see cref="LoadOrderDetailResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        LoadOrderDetailResponse LoadOrderDetail(LoadOrderDetailRequest request);

        [OperationContract]
        GetDiagnosticServiceSubTreeResponse GetDiagnosticServiceSubTree(GetDiagnosticServiceSubTreeRequest request);
    }
}
