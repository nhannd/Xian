#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Extended.Common.ProtocollingWorkflow
{
	// protocol uses both Registration and Reporting worklist items - that is why it is defined on WorklistItemSummaryBase
	// and we use ServiceKnownType to make it aware of the possible subclasses.
	[ServiceKnownType(typeof(RegistrationWorklistItemSummary))]
	[ServiceKnownType(typeof(ReportingWorklistItemSummary))]

	[RisApplicationService]
	[ServiceContract]
	public interface IProtocollingWorkflowService : IWorkflowService
	{
		[OperationContract]
		GetProtocolFormDataResponse GetProtocolFormData(GetProtocolFormDataRequest request);

		[OperationContract]
		ListProtocolGroupsForProcedureResponse ListProtocolGroupsForProcedure(ListProtocolGroupsForProcedureRequest request);

		[OperationContract]
		GetProtocolGroupDetailResponse GetProtocolGroupDetail(GetProtocolGroupDetailRequest request);

		[OperationContract]
		GetProcedureProtocolResponse GetProcedureProtocol(GetProcedureProtocolRequest request);

		[OperationContract]
		GetSuspendRejectReasonChoicesResponse GetSuspendRejectReasonChoices(GetSuspendRejectReasonChoicesRequest request);

		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		StartProtocolResponse StartProtocol(StartProtocolRequest request);

		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		DiscardProtocolResponse DiscardProtocol(DiscardProtocolRequest request);

		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(SupervisorValidationException))]
		AcceptProtocolResponse AcceptProtocol(AcceptProtocolRequest request);

		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(SupervisorValidationException))]
		RejectProtocolResponse RejectProtocol(RejectProtocolRequest request);

		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(SupervisorValidationException))]
		SubmitProtocolForApprovalResponse SubmitProtocolForApproval(SubmitProtocolForApprovalRequest request);

		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		ReviseSubmittedProtocolResponse ReviseSubmittedProtocol(ReviseSubmittedProtocolRequest request);

		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(SupervisorValidationException))]
		SaveProtocolResponse SaveProtocol(SaveProtocolRequest request);

		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		ResubmitProtocolResponse ResubmitProtocol(ResubmitProtocolRequest request);

		[OperationContract]
		GetLinkableProtocolsResponse GetLinkableProtocols(GetLinkableProtocolsRequest request);

		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		ReassignProcedureStepResponse ReassignProcedureStep(ReassignProcedureStepRequest request);
	}
}
