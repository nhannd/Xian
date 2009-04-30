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
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Common.TranscriptionWorkflow
{
	[RisApplicationService]
	[ServiceContract]
	[ServiceKnownType(typeof(ReportingWorklistItem))]
	public interface ITranscriptionWorkflowService : IWorklistService<ReportingWorklistItem>, IWorkflowService
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		GetRejectReasonChoicesResponse GetRejectReasonChoices(GetRejectReasonChoicesRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		StartTranscriptionResponse StartTranscription(StartTranscriptionRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		DiscardTranscriptionResponse DiscardTranscription(DiscardTranscriptionRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof (RequestValidationException))]
		[FaultContract(typeof (ConcurrentModificationException))]
		SaveTranscriptionResponse SaveTranscription(SaveTranscriptionRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		SubmitTranscriptionForReviewResponse SubmitTranscriptionForReview(SubmitTranscriptionForReviewRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof (RequestValidationException))]
		[FaultContract(typeof (ConcurrentModificationException))]
		CompleteTranscriptionResponse CompleteTranscription(CompleteTranscriptionRequest request);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof (RequestValidationException))]
		[FaultContract(typeof (ConcurrentModificationException))]
		RejectTranscriptionResponse RejectTranscription(RejectTranscriptionRequest request);

		/// <summary>
		/// Load the report of a given reporting step
		/// </summary>
		/// <param name="request"><see cref="LoadTranscriptionForEditRequest"/></param>
		/// <returns><see cref="LoadTranscriptionForEditResponse"/></returns>
		[OperationContract]
		LoadTranscriptionForEditResponse LoadTranscriptionForEdit(LoadTranscriptionForEditRequest request);
	}
}
