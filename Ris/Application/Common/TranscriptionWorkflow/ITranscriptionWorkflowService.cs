using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Common.TranscriptionWorkflow
{
	[RisServiceProvider]
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
