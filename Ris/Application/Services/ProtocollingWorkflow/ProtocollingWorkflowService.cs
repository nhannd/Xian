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
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
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

			ProcedureStep uncastProtocolStep = CollectionUtils.SelectFirst<ProcedureStep>(
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
			ProcedureStep mps = this.PersistenceContext.Load<ProcedureStep>(request.ProcedureStepRef);
			Order order = mps.Procedure.Order;

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
		[OperationEnablement("CanStartOrderProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role=AuthorityTokens.Workflow.Protocol.Create)]
		public StartOrderProtocolResponse StartOrderProtocol(StartOrderProtocolRequest request)
		{
			Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

			bool protocolClaimed = false;
			bool canPerformerAcceptProtocols = Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Accept);

			if (request.ShouldClaim)
			{
				ProtocollingOperations.StartProtocolOperation op = new ProtocollingOperations.StartProtocolOperation();
				op.Execute(order, this.CurrentUserStaff, canPerformerAcceptProtocols, out protocolClaimed);
			}

			List<OrderNoteDetail> noteDetails = GetNoteDetails(order, request.NoteCategory);

			this.PersistenceContext.SynchState();

			return new StartOrderProtocolResponse(order.GetRef(), protocolClaimed, noteDetails);
		}

		[UpdateOperation]
		[OperationEnablement("CanDiscardOrderProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Create)]
		public DiscardOrderProtocolResponse DiscardOrderProtocol(DiscardOrderProtocolRequest request)
		{
			Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

			if (request.ShouldUnclaim)
			{
				ProtocollingOperations.DiscardProtocolOperation op = new ProtocollingOperations.DiscardProtocolOperation();
				op.Execute(order);
			}

			// Does it make sense to save notes here?
			UpdateOrderNotes(order, request.OrderNotes);

			this.PersistenceContext.SynchState();

			return new DiscardOrderProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanAcceptOrderProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Accept)]
		public AcceptOrderProtocolResponse AcceptOrderProtocol(AcceptOrderProtocolRequest request)
		{
			Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

			ProtocollingOperations.AcceptProtocolOperation op = new ProtocollingOperations.AcceptProtocolOperation();
			op.Execute(order);

			UpdateProtocols(request.Protocols);
			UpdateOrderNotes(order, request.OrderNotes);

			this.PersistenceContext.SynchState();

			return new AcceptOrderProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanRejectOrderProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Create)]
		public RejectOrderProtocolResponse RejectOrderProtocol(RejectOrderProtocolRequest request)
		{
			Order order = this.PersistenceContext.Load<Order>(request.OrderRef);
			ProtocolRejectReasonEnum reason =
				EnumUtils.GetEnumValue<ProtocolRejectReasonEnum>(request.RejectReason, this.PersistenceContext);

			ProtocollingOperations.RejectProtocolOperation op = new ProtocollingOperations.RejectProtocolOperation();
			op.Execute(order, reason);

			UpdateProtocols(request.Protocols);
			UpdateOrderNotes(order, request.OrderNotes);
			AddAdditionalCommentsNote(request.AdditionalCommentsNote, order);

			this.PersistenceContext.SynchState();

			return new RejectOrderProtocolResponse();
		}

		[UpdateOperation]
		[OperationEnablement("CanSaveProtocol")]
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.Create)]
		public SaveProtocolResponse SaveOrderProtocol(SaveProtocolRequest request)
		{
			Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

			UpdateProtocols(request.Protocols);
			UpdateOrderNotes(order, request.OrderNotes);

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
		[PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.Workflow.Protocol.SubmitForApproval)]
		public SubmitProtocolForApprovalResponse SubmitProtocolForApproval(SubmitProtocolForApprovalRequest request)
		{
			Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

			ProtocollingOperations.SubmitForApprovalOperation op = new ProtocollingOperations.SubmitForApprovalOperation();
			op.Execute(order);

			UpdateProtocols(request.Protocols);
			UpdateOrderNotes(order, request.OrderNotes);

			this.PersistenceContext.SynchState();

			return new SubmitProtocolForApprovalResponse();
		}

		#endregion

		#region OperationEnablement methods

		public bool CanStartOrderProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Create))
				return false;
			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.StartProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanDiscardOrderProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Create))
				return false;
			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.DiscardProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanAcceptOrderProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Accept))
				return false;
			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.AcceptProtocolOperation(), enablementContext.ProcedureStepRef);
		}

		public bool CanRejectOrderProtocol(ProtocolOperationEnablementContext enablementContext)
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

			return !step.IsTerminated;
		}

		public bool CanResubmitProtocol(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.Resubmit))
				return false;
			return CanExecuteOperation(new ProtocollingOperations.ResubmitProtocolOperation(), enablementContext.OrderRef);
		}

		public bool CanSubmitProtocolForApproval(ProtocolOperationEnablementContext enablementContext)
		{
			if (!Thread.CurrentPrincipal.IsInRole(AuthorityTokens.Workflow.Protocol.SubmitForApproval))
				return false;
			return CanExecuteOperation<ProtocolAssignmentStep>(new ProtocollingOperations.SubmitForApprovalOperation(), enablementContext.ProcedureStepRef);
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

		private void UpdateProtocols(List<ProtocolDetail> protocolDetails)
		{
			ProtocolAssembler assembler = new ProtocolAssembler();
			foreach (ProtocolDetail protocolDetail in protocolDetails)
			{
				Protocol protocol = this.PersistenceContext.Load<Protocol>(protocolDetail.ProtocolRef);
				assembler.UpdateProtocol(protocol, protocolDetail, this.PersistenceContext);
			}
		}

		private void UpdateOrderNotes(Order order, IList<OrderNoteDetail> notes)
		{
			OrderNoteAssembler noteAssembler = new OrderNoteAssembler();
			
			noteAssembler.SynchronizeOrderNotes(order, notes, this.CurrentUserStaff, this.PersistenceContext);
		}
	}
}
