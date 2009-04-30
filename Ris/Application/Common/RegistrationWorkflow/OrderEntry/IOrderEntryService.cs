#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    /// <summary>
    /// Provides services for entering orders into the system, and modifying existing orders.
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    [ServiceKnownType(typeof(RegistrationWorklistItem))]
    [ServiceKnownType(typeof(ModalityWorklistItem))]
    public interface IOrderEntryService : IWorkflowService
    {
        /// <summary>
        /// List visits for the specified patient.  Orders can be placed on any visits, including discharged visits.
        /// </summary>
		/// <param name="request"><see cref="ListVisitsForPatientRequest"/></param>
		/// <returns><see cref="ListVisitsForPatientResponse"/></returns>
		[OperationContract]
        ListVisitsForPatientResponse ListVisitsForPatient(ListVisitsForPatientRequest request);

        /// <summary>
        /// List the active orders for the specified patient.  Active orders are either Scheduled or In-Progress.
        /// </summary>
        /// <param name="request"><see cref="ListOrdersForPatientRequest"/></param>
        /// <returns><see cref="ListOrdersForPatientResponse"/></returns>
        [OperationContract]
        ListOrdersForPatientResponse ListActiveOrdersForPatient(ListOrdersForPatientRequest request);

        /// <summary>
        /// Loads all order entry form data.
        /// </summary>
		/// <param name="request"><see cref="GetOrderEntryFormDataRequest"/></param>
		/// <returns><see cref="GetOrderEntryFormDataResponse"/></returns>
		[OperationContract]
        GetOrderEntryFormDataResponse GetOrderEntryFormData(GetOrderEntryFormDataRequest request);

        /// <summary>
        /// Get order cancel form data.
        /// </summary>
		/// <param name="request"><see cref="GetCancelOrderFormDataRequest"/></param>
		/// <returns><see cref="GetCancelOrderFormDataResponse"/></returns>
        [OperationContract]
        GetCancelOrderFormDataResponse GetCancelOrderFormData(GetCancelOrderFormDataRequest request);

        /// <summary>
        /// Loads order requisition so that the order editing form can be populated. This method will
        /// fail with a RequestValidationException if the order requisition cannot be edited.
        /// </summary>
		/// <param name="request"><see cref="GetOrderRequisitionForEditRequest"/></param>
		/// <returns><see cref="GetOrderRequisitionForEditResponse"/></returns>
		[OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        GetOrderRequisitionForEditResponse GetOrderRequisitionForEdit(GetOrderRequisitionForEditRequest request);

        /// <summary>
        /// Gets the details of a diagnostic service plan.
        /// </summary>
		/// <param name="request"><see cref="LoadDiagnosticServiceBreakdownRequest"/></param>
		/// <returns><see cref="LoadDiagnosticServiceBreakdownRequest"/></returns>
		[OperationContract]
        LoadDiagnosticServiceBreakdownResponse LoadDiagnosticServiceBreakdown(LoadDiagnosticServiceBreakdownRequest request);

        /// <summary>
        /// List procedure types that can be ordered based on the procedure types that have already been added to
        /// an order.
        /// </summary>
		/// <param name="request"><see cref="ListOrderableProcedureTypesRequest"/></param>
		/// <returns><see cref="ListOrderableProcedureTypesResponse"/></returns>
		[OperationContract]
        ListOrderableProcedureTypesResponse ListOrderableProcedureTypes(ListOrderableProcedureTypesRequest request);

        /// <summary>
        /// Gets detailed information about all of the contact points associated with a specified external practitioner.
        /// </summary>
		/// <param name="request"><see cref="GetExternalPractitionerContactPointsRequest"/></param>
		/// <returns><see cref="GetExternalPractitionerContactPointsResponse"/></returns>
		[OperationContract]
        GetExternalPractitionerContactPointsResponse GetExternalPractitionerContactPoints(GetExternalPractitionerContactPointsRequest request);

        /// <summary>
        /// Places a new order based on the specified information.
        /// </summary>
		/// <param name="request"><see cref="PlaceOrderRequest"/></param>
		/// <returns><see cref="PlaceOrderResponse"/></returns>
		[OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        PlaceOrderResponse PlaceOrder(PlaceOrderRequest request);

        /// <summary>
        /// Modifies an existing order based on the specified information.
        /// </summary>
		/// <param name="request"><see cref="ModifyOrderRequest"/></param>
		/// <returns><see cref="ModifyOrderResponse"/></returns>
		[OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        ModifyOrderResponse ModifyOrder(ModifyOrderRequest request);

        /// <summary>
        /// Cancels an existing order and places a new order as a single transaction.
        /// </summary>
		/// <param name="request"><see cref="ReplaceOrderRequest"/></param>
		/// <returns><see cref="ReplaceOrderResponse"/></returns>
		[OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        ReplaceOrderResponse ReplaceOrder(ReplaceOrderRequest request);


        /// <summary>
        /// Cancel orders with a cancellation reason for a patient
        /// </summary>
        /// <param name="request"><see cref="CancelOrderRequest"/></param>
        /// <returns><see cref="CancelOrderResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        CancelOrderResponse CancelOrder(CancelOrderRequest request);

        /// <summary>
        /// This method is for testing/demo purposes and is not intended to be called in production.
        /// It shifts the order and associated visit in time by the specified number of minutes, which may be negative or positive.
        /// </summary>
        /// <remarks>
        /// This method does not really belong on this interface but there was no other
        /// convenient place to put it.
        /// </remarks>
		/// <param name="request"><see cref="TimeShiftOrderRequest"/></param>
		/// <returns><see cref="TimeShiftOrderResponse"/></returns>
		[OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        TimeShiftOrderResponse TimeShiftOrder(TimeShiftOrderRequest request);

        /// <summary>
        /// Reserve an accession number.
        /// </summary>
		/// <param name="request"><see cref="ReserveAccessionNumberRequest"/></param>
		/// <returns><see cref="ReserveAccessionNumberResponse"/></returns>
		[OperationContract]
        ReserveAccessionNumberResponse ReserveAccessionNumber(ReserveAccessionNumberRequest request);
    }
}
