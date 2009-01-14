#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [RisApplicationService]
    [ServiceContract]
	[ServiceKnownType(typeof(ModalityWorklistItem))]
	public interface IModalityWorkflowService : IWorklistService<ModalityWorklistItem>, IWorkflowService
    {
        /// <summary>
        /// Returns a summary of the procedure plan for a specified order.
        /// </summary>
        /// <param name="request"><see cref="GetProcedurePlanRequest"/></param>
        /// <returns><see cref="GetProcedurePlanResponse"/></returns>
        [OperationContract]
		GetProcedurePlanResponse GetProcedurePlan(GetProcedurePlanRequest request);

        /// <summary>
        /// Returns a list of all modality performed procedure steps for a particular order.
        /// </summary>
        /// <param name="request"><see cref="ListPerformedProcedureStepsRequest"/></param>
        /// <returns><see cref="ListPerformedProcedureStepsResponse"/></returns>
        [OperationContract]
        ListPerformedProcedureStepsResponse ListPerformedProcedureSteps(ListPerformedProcedureStepsRequest request);

        /// <summary>
        /// Starts a specified set of modality procedure steps with a single modality performed procedure step.
        /// </summary>
        /// <param name="request"><see cref="StartModalityProcedureStepsRequest"/></param>
        /// <returns><see cref="StartModalityProcedureStepsResponse"/></returns>
        [OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		StartModalityProcedureStepsResponse StartModalityProcedureSteps(StartModalityProcedureStepsRequest request);

        /// <summary>
        /// Discontinues a set of specified modality procedure steps.
        /// </summary>
        /// <param name="request"><see cref="DiscontinueModalityProcedureStepsResponse"/></param>
        /// <returns><see cref="DiscontinueModalityProcedureStepsRequest"/></returns>
        [OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DiscontinueModalityProcedureStepsResponse DiscontinueModalityProcedureSteps(DiscontinueModalityProcedureStepsRequest request);

        /// <summary>
        /// Completes a specified modality performed procedure step.
        /// </summary>
        /// <param name="request"><see cref="CompleteModalityPerformedProcedureStepRequest"/></param>
        /// <returns><see cref="CompleteModalityPerformedProcedureStepResponse"/></returns>
        [OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		CompleteModalityPerformedProcedureStepResponse CompleteModalityPerformedProcedureStep(CompleteModalityPerformedProcedureStepRequest request);

        /// <summary>
        /// Discontinues a specified modality performed procedure step.
        /// </summary>
        /// <param name="request"><see cref="DiscontinueModalityPerformedProcedureStepRequest"/></param>
        /// <returns><see cref="DiscontinueModalityPerformedProcedureStepResponse"/></returns>
        [OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DiscontinueModalityPerformedProcedureStepResponse DiscontinueModalityPerformedProcedureStep(DiscontinueModalityPerformedProcedureStepRequest request);

		/// <summary>
		/// Load performing documentation data for an order.
		/// </summary>
		/// <param name="request"><see cref="LoadOrderDocumentationDataRequest"/></param>
		/// <returns><see cref="LoadOrderDocumentationDataResponse"/></returns>
		[OperationContract]
		LoadOrderDocumentationDataResponse LoadOrderDocumentationData(LoadOrderDocumentationDataRequest request);

		/// <summary>
		/// Save performing documentation data for an order.
		/// </summary>
		/// <param name="request"><see cref="SaveOrderDocumentationDataRequest"/></param>
		/// <returns><see cref="SaveOrderDocumentationDataResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		SaveOrderDocumentationDataResponse SaveOrderDocumentationData(SaveOrderDocumentationDataRequest request);

		/// <summary>
		/// Verify if an order has documentation to complete.
		/// </summary>
		/// <param name="request"><see cref="CanCompleteOrderDocumentationRequest"/></param>
		/// <returns><see cref="CanCompleteOrderDocumentationResponse"/></returns>
		[OperationContract]
		CanCompleteOrderDocumentationResponse CanCompleteOrderDocumentation(CanCompleteOrderDocumentationRequest request);

		/// <summary>
		/// Complete documentation for an order.
		/// </summary>
		/// <param name="request"><see cref="CompleteOrderDocumentationRequest"/></param>
		/// <returns><see cref="CompleteOrderDocumentationResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		CompleteOrderDocumentationResponse CompleteOrderDocumentation(CompleteOrderDocumentationRequest request);
	}
}
