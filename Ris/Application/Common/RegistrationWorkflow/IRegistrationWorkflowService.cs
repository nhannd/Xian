#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
        /// Search for registration worklist item
        /// </summary>
        /// <param name="request"><see cref="SearchRequest"/></param>
        /// <returns><see cref="SearchResponse"/></returns>
        [OperationContract]
        SearchResponse Search(SearchRequest request);

        /// <summary>
        /// Search for Patient
        /// </summary>
        /// <param name="request"><see cref="SearchPatientRequest"/></param>
        /// <returns><see cref="SearchPatientResponse"/></returns>
        [OperationContract]
        SearchPatientResponse SearchPatient(SearchPatientRequest request);

        /// <summary>
        /// Load data required for the patient search
        /// </summary>
        /// <param name="request"><see cref="LoadSearchPatientFormDataRequest"/></param>
        /// <returns><see cref="LoadSearchPatientFormDataResponse"/></returns>
        [OperationContract]
        LoadSearchPatientFormDataResponse LoadSearchPatientFormData(LoadSearchPatientFormDataRequest request);

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

        /// <summary>
        /// Replace an order with a cancellation reason for a patient
        /// </summary>
        /// <param name="request"><see cref="CancelOrderRequest"/></param>
        /// <returns><see cref="CancelOrderResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        ReplaceOrderResponse ReplaceOrder(ReplaceOrderRequest request);
    }
}
