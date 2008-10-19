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

using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Protocolling;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Services.ReportingWorkflow;
using ClearCanvas.Workflow;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Application.Services.ProtocollingWorkflow
{
	[ServiceImplementsContract(typeof(IProtocollingWorkflowService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class ProtocollingWorkflowService : WorkflowServiceBase<WorklistItemSummaryBase>, IProtocollingWorkflowService
	{
		/// <summary>
		/// Provides a context for determining if protocol operations are enabled.
		/// </summary>
		public class ProtocolOperationEnablementContext
		{
			private readonly EntityRef _procedureStepRef;
			private readonly EntityRef _orderRef;

			/// <summary>
			/// Constructor.  One of the entity refs should be non-null.
			/// </summary>
			/// <param name="orderRef"></param>
			/// <param name="procedureStepRef"></param>
			public ProtocolOperationEnablementContext(EntityRef orderRef, EntityRef procedureStepRef)
			{
				this._orderRef = orderRef;
				this._procedureStepRef = procedureStepRef;
			}

			public EntityRef OrderRef
			{
				get { return _orderRef; }
			}

			public EntityRef ProcedureStepRef
			{
				get { return _procedureStepRef; }
			}
		}

		#region IProtocollingWorkflowService Members

		[ReadOperation]
		public GetProtocolFormDataResponse GetProtocolFormData(GetProtocolFormDataRequest request)
		{
			GetProtocolFormDataResponse response = new GetProtocolFormDataResponse();
			response.ProtocolUrgencyChoices = EnumUtils.GetEnumValueList<ProtocolUrgencyEnum>(PersistenceContext);
			return response;
		}

		[ReadOperation]
		public ListProtocolGroupsForProcedureResponse ListProtocolGroupsForProcedure(ListProtocolGroupsForProcedureRequest request)
		{
			ProtocolAssembler assembler = new ProtocolAssembler();
			Procedure rp = this.PersistenceContext.Load<Procedure>(request.ProcedureRef);

			List<ProtocolGroupSummary> groups = CollectionUtils.Map<ProtocolGroup, ProtocolGroupSummary>(
				this.PersistenceContext.GetBroker<IProtocolGroupBroker>().FindAll(rp.Type),
				delegate(ProtocolGroup protocolGroup) { return assembler.CreateProtocolGroupSummary(protocolGroup); });

			ProtocolGroupSummary initialProtocolGroup = CollectionUtils.FirstElement<ProtocolGroupSummary>(groups);

			return new ListProtocolGroupsForProcedureResponse(groups, initialProtocolGroup);
		}

		[ReadOperation]
		public GetProtocolGroupDetailResponse GetProtocolGroupDetail(GetProtocolGroupDetailRequest request)
		{
			ProtocolGroup protocolGroup = this.PersistenceContext.Load<ProtocolGroup>(request.ProtocolGroup.ProtocolGroupRef);

			ProtocolAssembler assembler = new ProtocolAssembler();

			return new GetProtocolGroupDetailResponse(assembler.CreateProtocolGroupDetail(protocolGroup, this.PersistenceContext));
		}

		[ReadOperation]
		public GetProcedureProtocolResponse GetProcedureProtocol(GetProcedureProtocolRequest request)
		{
			Procedure rp = this.PersistenceContext.Load<Procedure>(request.ProcedureRef);
			ProtocolAssembler assembler = new ProtocolAssembler();

			ProcedureStep uncastProtocolStep = CollectionUtils.SelectFirst(
				rp.ProcedureSteps,
				delegate(ProcedureStep ps) { return ps.Is<ProtocolProcedureStep>(); });

			if (uncastProtocolStep != null)
			{
				ProtocolProcedureStep protocolStep = uncastProtocolStep.Downcast<ProtocolProcedureStep>();
				return new GetProcedureProtocolResponse(assembler.CreateProtocolDetail(protocolStep.Protocol, this.PersistenceContext));
			}
			else
			{
				return new GetProcedureProtocolResponse(null);
			}
		}

		[ReadOperation]
		public GetProcedurePlanForProtocollingWorklistItemResponse GetProcedurePlanForProtocollingWorklistItem(GetProcedurePlanForProtocollingWorklistItemRequest request)
		{
			Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

			ProcedurePlanAssembler assembler = new ProcedurePlanAssembler();
			ProcedurePlanDetail procedurePlanSummary =
				assembler.CreateProcedurePlanSummary(order, this.PersistenceContext);

			return new GetProcedurePlanForProtocollingWorklistItemResponse(procedurePlanSummary);
		}

		[ReadOperation]
		public GetSuspendRejectReasonChoicesResponse GetSuspendRejectReasonChoices(GetSuspendRejectReasonChoicesRequest request)
		{
			List<EnumValueInfo> choices = EnumUtils.GetEnumValueList<ProtocolRejectReasonEnum>(this.PersistenceContext);
			return new GetSuspendRejectReasonChoicesResponse(choices);
		}

		[UpdateOperation]
		[OperationEnablement("CanStartProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Create)]
		public StartProtocolResponse StartProtocol(StartProtocolRequest request)
		{
			ProtocolAssignmentStep assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			bool protocolClaimed = false;
			bool canPerformerAcceptProtocols = Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Accept);
			Staff assignedStaff = null;

			// todo: linked procedures

			if (request.ShouldClaim)
			{
				try
				{
					ProtocollingOperations.StartProtocolOperation op = new ProtocollingOperations.StartProtocolOperation();
					op.Execute(assignmentStep, null, this.CurrentUserStaff, canPerformerAcceptProtocols, out protocolClaimed, out assignedStaff);
				}
				catch (Exception e)
				{
					throw new RequestValidationException(e.Message);
				}
			}

			List<OrderNoteDetail> noteDetails = GetNoteDetails(assignmentStep.Procedure.Order, request.NoteCategory);

			this.PersistenceContext.SynchState();

			return new StartProtocolResponse(
				assignmentStep.GetRef(),
				assignedStaff == null ? null : assignedStaff.GetRef(),
				protocolClaimed,
				noteDetails);
		}

		[UpdateOperation]
		[OperationEnablement("CanDiscardProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Create)]
		public DiscardProtocolResponse DiscardProtocol(DiscardProtocolRequest request)
		{
			ProtocolAssignmentStep assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);
			Staff staff = request.ReassignToStaff == null ? null : this.PersistenceContext.Load<Staff>(request.ReassignToStaff);

			if (request.ShouldUnclaim)
			{
				// demand authority token if trying to cancel a protocol that is perfomed by someone else
				if ((assignmentStep.State == ActivityStatus.SC && !Equals(assignmentStep.AssignedStaff, this.CurrentUserStaff)) ||
					(assignmentStep.State == ActivityStatus.IP && !Equals(assignmentStep.PerformingStaff, this.CurrentUserStaff)))
				{
					PrincipalPermission permission = new PrincipalPermission(null, AuthorityTokens.Workflow.Protocol.Cancel);
					permission.Demand();
				}
			}

			if (request.ShouldUnclaim)
			{
				ProtocollingOperations.DiscardProtocolOperation op = new ProtocollingOperations.DiscardProtocolOperation();
				op.Execute(assignmentStep, staff);
			}

			// Does it make sense to save notes here?
			if (request.OrderNotes != null)
				UpdateOrderNotes(assignmentStep.Procedure.Order, request.OrderNotes);

			this.PersistenceContext.SynchState();

			return new DiscardProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanAcceptProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Accept)]
		public AcceptProtocolResponse AcceptProtocol(AcceptProtocolRequest request)
		{
			ProtocolAssignmentStep assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			SaveProtocolHelper(assignmentStep, request.Protocol, request.OrderNotes);

			ProtocollingOperations.AcceptProtocolOperation op = new ProtocollingOperations.AcceptProtocolOperation();
			op.Execute(assignmentStep, this.CurrentUserStaff);

			this.PersistenceContext.SynchState();

			return new AcceptProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanRejectProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Create)]
		public RejectProtocolResponse RejectProtocol(RejectProtocolRequest request)
		{
			ProtocolAssignmentStep assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			SaveProtocolHelper(assignmentStep, request.Protocol, request.OrderNotes);

			ProtocolRejectReasonEnum reason =
				EnumUtils.GetEnumValue<ProtocolRejectReasonEnum>(request.RejectReason, this.PersistenceContext);

			ProtocollingOperations.RejectProtocolOperation op = new ProtocollingOperations.RejectProtocolOperation();
			op.Execute(assignmentStep, this.CurrentUserStaff, reason);

			AddAdditionalCommentsNote(request.AdditionalCommentsNote, assignmentStep.Procedure.Order);

			this.PersistenceContext.SynchState();

			return new RejectProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanSaveProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Create)]
		public SaveProtocolResponse SaveProtocol(SaveProtocolRequest request)
		{
			ProtocolAssignmentStep assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			SaveProtocolHelper(assignmentStep, request.Protocol, request.OrderNotes);

			return new SaveProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanResubmitProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Resubmit)]
		public ResubmitProtocolResponse ResubmitProtocol(ResubmitProtocolRequest request)
		{
			Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

			ProtocollingOperations.ResubmitProtocolOperation op = new ProtocollingOperations.ResubmitProtocolOperation();
			op.Execute(order, this.CurrentUserStaff);

			this.PersistenceContext.SynchState();

			return new ResubmitProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanSubmitProtocolForApproval")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.SubmitForReview)]
		public SubmitProtocolForApprovalResponse SubmitProtocolForApproval(SubmitProtocolForApprovalRequest request)
		{
			ProtocolAssignmentStep assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);
			Staff supervisor = null;

			if (request.Protocol != null)
			{
				supervisor = GetSupervisor(request.Protocol);
			}

			SaveProtocolHelper(assignmentStep, request.Protocol, request.OrderNotes);

			bool canOmitSupervisor = Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.OmitSupervisor);

			try
			{
				ProtocollingOperations.SubmitForApprovalOperation op = new ProtocollingOperations.SubmitForApprovalOperation();
				op.Execute(assignmentStep, supervisor, canOmitSupervisor);
			}
			catch (SupervisorRequiredException e)
			{
				throw new RequestValidationException(e.Message);
			}

			this.PersistenceContext.SynchState();

			return new SubmitProtocolForApprovalResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanReviseSubmittedProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.SubmitForReview)]
		public ReviseSubmittedProtocolResponse ReviseSubmittedProtocol(ReviseSubmittedProtocolRequest request)
		{
			ProtocolAssignmentStep assignmentStep = this.PersistenceContext.Load<ProtocolAssignmentStep>(request.ProtocolAssignmentStepRef);

			ProtocollingOperations.ReviseSubmittedProtocolOperation op = new ProtocollingOperations.ReviseSubmittedProtocolOperation();
			ProtocolAssignmentStep step = op.Execute(assignmentStep, this.CurrentUserStaff);

			this.PersistenceContext.SynchState();

			return new ReviseSubmittedProtocolResponse(GetWorklistItemSummary(step));
		}

		#endregion

		#region OperationEnablement methods

		public bool CanStartProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Create))
				return false;
			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.StartProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanDiscardProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Create))
				return false;

			// if there is no proc step ref, operation is not available
			if (enablementContext.ProcedureStepRef == null)
				return false;

			ProcedureStep step = PersistenceContext.Load<ProcedureStep>(enablementContext.ProcedureStepRef);

			// cannot cancel a protocol that is performed by someone else without the authority token
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Cancel) &&
				((step.State == ActivityStatus.SC && !Equals(step.AssignedStaff, this.CurrentUserStaff)) ||
				(step.State == ActivityStatus.IP && !Equals(step.PerformingStaff, this.CurrentUserStaff))))
				return false;

			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.DiscardProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanAcceptProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Accept))
				return false;
			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.AcceptProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanRejectProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Create))
				return false;
			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.RejectProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanSaveProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Create))
				return false;

			if (enablementContext.ProcedureStepRef == null)
				return false;

			ProcedureStep step = PersistenceContext.Load<ProcedureStep>(enablementContext.ProcedureStepRef);

			if (!step.Is<ProtocolAssignmentStep>())
				return false;

			if (step.AssignedStaff != null && !Equals(step.AssignedStaff, this.CurrentUserStaff))
				return false;

			if (step.PerformingStaff != null && !Equals(step.PerformingStaff, CurrentUserStaff))
				return false;

			// items submitted for review should not be editable.
			ProtocolAssignmentStep assignmentStep = step.As<ProtocolAssignmentStep>();
			if (assignmentStep.Protocol.Status == ProtocolStatus.AA)
			{
				if (Equals(assignmentStep.Protocol.Author, this.CurrentUserStaff))
					return false;
			}

			if (step.IsTerminated)
				return false;

			return true;
		}

		public bool CanResubmitProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Resubmit))
				return false;
			return CanExecuteOperation(new ProtocollingOperations.ResubmitProtocolOperation(), enablementContext.OrderRef);
		}

		public bool CanSubmitProtocolForApproval(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.SubmitForReview))
				return false;

			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.SubmitForApprovalOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanReviseSubmittedProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.SubmitForReview))
				return false;

			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.ReviseSubmittedProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		private bool CanExecuteOperation<T>(ProtocollingOperations.ProtocollingOperation op, EntityRef procedureStepRef)
			where T : ProtocolProcedureStep
		{
			// if there is no proc step ref, operation is not available
			if (procedureStepRef == null)
				return false;

			ProcedureStep step = PersistenceContext.Load<ProcedureStep>(procedureStepRef);

			if (!step.Is<T>())
				return false;

			return op.CanExecute(step.As<T>(), this.CurrentUserStaff);
		}

		private bool CanExecuteOperation(ProtocollingOperations.ProtocollingOperation op, EntityRef orderRef)
		{
			if (orderRef == null)
				return false;

			Order order = this.PersistenceContext.Load<Order>(orderRef);

			return op.CanExecute(order, this.CurrentUserStaff);
		}

		#endregion

		private void SaveProtocolHelper(ProtocolAssignmentStep step, ProtocolDetail protocolDetail, List<OrderNoteDetail> notes)
		{
			if (protocolDetail != null)
			{
				ProtocolAssembler assembler = new ProtocolAssembler();
				Protocol protocol = this.PersistenceContext.Load<Protocol>(protocolDetail.ProtocolRef);
				assembler.UpdateProtocol(protocol, protocolDetail, this.PersistenceContext);
			}

			if (notes != null)
				UpdateOrderNotes(step.Procedure.Order, notes);
		}

		private Staff GetSupervisor(ProtocolDetail protocolDetail)
		{
			EntityRef supervisorRef = (protocolDetail != null && protocolDetail.Supervisor != null) ? protocolDetail.Supervisor.StaffRef : null;
			return supervisorRef == null ? null : this.PersistenceContext.Load<Staff>(supervisorRef);
		}

		protected override object GetWorkItemKey(WorklistItemSummaryBase item)
		{
			return new ProtocolOperationEnablementContext(item.OrderRef, item.ProcedureStepRef);
		}

		private void AddAdditionalCommentsNote(OrderNoteDetail detail, Order order)
		{
			if(detail != null)
			{
				OrderNoteAssembler noteAssembler = new OrderNoteAssembler();
				noteAssembler.CreateOrderNote(detail, order, this.CurrentUserStaff, true, this.PersistenceContext);
			}
		}

		private List<OrderNoteDetail> GetNoteDetails(Order order, string category)
		{
			OrderNoteAssembler noteAssembler = new OrderNoteAssembler();
			List<OrderNote> notes = CollectionUtils.Select(
				order.Notes,
				delegate(OrderNote note)
					{
						return note.Category == category;
					});
			return CollectionUtils.Map<OrderNote, OrderNoteDetail>(
				notes,
				delegate(OrderNote note)
					{
						return noteAssembler.CreateOrderNoteDetail(note, this.PersistenceContext);
					});
		}

		private void UpdateOrderNotes(Order order, IList<OrderNoteDetail> notes)
		{
			OrderNoteAssembler noteAssembler = new OrderNoteAssembler();
			
			noteAssembler.SynchronizeOrderNotes(order, notes, this.CurrentUserStaff, this.PersistenceContext);
		}

		private ReportingWorklistItem GetWorklistItemSummary(ProtocolProcedureStep reportingProcedureStep)
		{
			IList<ProtocolProcedureStep> procedureSteps = new List<ProtocolProcedureStep>();
			procedureSteps.Add(reportingProcedureStep);

			IList<WorklistItem> items = this.PersistenceContext.GetBroker<IReportingWorklistItemBroker>().GetWorklistItems(procedureSteps);
			return new ReportingWorkflowAssembler().CreateWorklistItemSummary(CollectionUtils.FirstElement(items), this.PersistenceContext);
		}

	}
}
