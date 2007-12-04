using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services.ProtocollingWorkflow
{
    [ServiceImplementsContract(typeof(IProtocollingWorkflowService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class ProtocollingWorkflowService : WorkflowServiceBase, IProtocollingWorkflowService
    {
        #region IProtocollingWorkflowService Members

        [ReadOperation]
        public ListProtocolGroupsForProcedureResponse ListProtocolGroupsForProcedure(ListProtocolGroupsForProcedureRequest request)
        {
            ProtocollingWorkflowAssembler assembler = new ProtocollingWorkflowAssembler();
            RequestedProcedure rp = this.PersistenceContext.Load<RequestedProcedure>(request.ProcedureRef);

            List<ProtocolGroupSummary> groups = CollectionUtils.Map<ProtocolGroup, ProtocolGroupSummary>(
                this.PersistenceContext.GetBroker<IProtocolGroupBroker>().FindAll(rp.Type),
                delegate(ProtocolGroup protocolGroup) { return assembler.CreateProtocolGroupSummary(protocolGroup); });

            ProtocolGroupSummary initialProtocolGroup = CollectionUtils.FirstElement<ProtocolGroupSummary>(groups);

            return new ListProtocolGroupsForProcedureResponse(groups, initialProtocolGroup);
        }

        [ReadOperation]
        public GetProtocolGroupDetailResponse GetProtocolGroupDetail(GetProtocolGroupDetailRequest request)
        {
            ProtocolGroup protocolGroup = this.PersistenceContext.Load<ProtocolGroup>(request.ProtocolGroup.EntityRef);
            
            ProtocollingWorkflowAssembler assembler = new ProtocollingWorkflowAssembler();

            return new GetProtocolGroupDetailResponse(assembler.CreateProtocolGroupDetail(protocolGroup, this.PersistenceContext));
        }

        [ReadOperation]
        public GetProtocolResponse GetProtocol(GetProtocolRequest request)
        {
            Protocol protocol = this.PersistenceContext.Load<Protocol>(request.ProtocolRef);
            ProtocollingWorkflowAssembler assembler = new ProtocollingWorkflowAssembler();
            return new GetProtocolResponse(protocol.GetRef(), assembler.CreateProtocolDetail(protocol, this.PersistenceContext));
        }

        [ReadOperation]
        public GetProcedureProtocolResponse GetProcedureProtocol(GetProcedureProtocolRequest request)
        {
            RequestedProcedure rp = this.PersistenceContext.Load<RequestedProcedure>(request.RequestedProcedureRef);
            ProtocollingWorkflowAssembler assembler = new ProtocollingWorkflowAssembler();

            ProcedureStep uncastProtcolStep = CollectionUtils.SelectFirst<ProcedureStep>(
                rp.ProcedureSteps,
                delegate(ProcedureStep ps) { return ps.Is<ProtocolProcedureStep>(); });

            ProtocolProcedureStep protocolStep = uncastProtcolStep.Downcast<ProtocolProcedureStep>();

            return new GetProcedureProtocolResponse(protocolStep.Protocol.GetRef(), assembler.CreateProtocolDetail(protocolStep.Protocol, this.PersistenceContext));
        }

        [ReadOperation]
        public GetProcedurePlanForProtocollingWorklistItemResponse GetProcedurePlanForProtocollingWorklistItem(GetProcedurePlanForProtocollingWorklistItemRequest request)
        {
            ProcedureStep mps = this.PersistenceContext.Load<ProcedureStep>(request.ProcedureStepRef);
            Order order = mps.RequestedProcedure.Order;

            ProcedurePlanAssembler assembler = new ProcedurePlanAssembler();
            ProcedurePlanSummary procedurePlanSummary =
                assembler.CreateProcedurePlanSummary(order, this.PersistenceContext);

            return new GetProcedurePlanForProtocollingWorklistItemResponse(procedurePlanSummary);
        }

        [ReadOperation]
        public GetProtocolOperationEnablementResponse GetProtocolOperationEnablement(GetProtocolOperationEnablementRequest request)
        {
            ProcedureStep ps = this.PersistenceContext.Load<ProcedureStep>(request.ProtocolAssignementStepRef);
            GetProtocolOperationEnablementResponse response = new GetProtocolOperationEnablementResponse();

            if (ps.Is<ProtocolAssignmentStep>())
            {
                ProtocolAssignmentStep protocolAssignementStep = ps.As<ProtocolAssignmentStep>();
                response.AcceptEnabled = protocolAssignementStep.CanAccept;
                response.RejectEnabled = protocolAssignementStep.CanReject;
                response.SuspendEnabled = protocolAssignementStep.CanSuspend;
                response.SaveEnabled = protocolAssignementStep.CanSave;
            }

            return response;
        }

        [ReadOperation]
        public GetClericalProtocolOperationEnablementResponse GetClericalProtocolOperationEnablement(GetClericalProtocolOperationEnablementRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);
            RequestedProcedure rp = CollectionUtils.FirstElement<RequestedProcedure>(order.RequestedProcedures);
            ProtocolResolutionStep resolutionStep = ScheduledProcedureStep<ProtocolResolutionStep>(rp);

            GetClericalProtocolOperationEnablementResponse response = new GetClericalProtocolOperationEnablementResponse();

            if (resolutionStep != null)
            {
                response.CanResolveByCancel = resolutionStep.ShouldCancel;
                response.CanResolveByResubmit = resolutionStep.ShouldResubmit;
            }
            else
            {
                response.CanResolveByCancel = false;
                response.CanResolveByResubmit = false;
            }

            return response;
        }

        [ReadOperation]
        public GetSuspendRejectReasonChoicesResponse GetSuspendRejectReasonChoices(GetSuspendRejectReasonChoicesRequest request)
        {
            List<EnumValueInfo> choices = EnumUtils.GetEnumValueList<ProtocolSuspendRejectReasonEnum>(this.PersistenceContext);
            return new GetSuspendRejectReasonChoicesResponse(choices);
        }

        [UpdateOperation]
        public AddOrderProtocolStepsResponse AddOrderProtocolSteps(AddOrderProtocolStepsRequest request)
        {
            Order o = this.PersistenceContext.Load<Order>(request.RequestedProcedureRef);

            foreach (RequestedProcedure rp in o.RequestedProcedures)
            {
                if(InprogressProcedureStep<ProtocolAssignmentStep>(rp) != null)
                    throw new RequestValidationException("Protocol step already exists for one or more requested procedures.  Probably stale data.");

                Protocol protocol = new Protocol(rp);
                ProtocolAssignmentStep assignmentStep = new ProtocolAssignmentStep(protocol);
                rp.AddProcedureStep(assignmentStep);

                assignmentStep.Schedule(DateTime.Now);

                this.PersistenceContext.Lock(protocol, DirtyState.New);
                this.PersistenceContext.Lock(assignmentStep, DirtyState.New);
            }
            
            this.PersistenceContext.SynchState();

            return new AddOrderProtocolStepsResponse();
        }

        [UpdateOperation]
        public StartOrderProtocolResponse StartOrderProtocol(StartOrderProtocolRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

            bool protocolClaimed = false;

            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                ProtocolAssignmentStep assignmentStep = ScheduledProcedureStep<ProtocolAssignmentStep>(rp);

                if(assignmentStep == null)
                    throw new RequestValidationException(SR.ExceptionNoProtocolAssignmentStep);

                assignmentStep.Start(this.CurrentUserStaff);
                protocolClaimed = true;
            }

            return new StartOrderProtocolResponse(protocolClaimed);
        }

        [UpdateOperation]
        public DiscardOrderProtocolResponse DiscardOrderProtocol(DiscardOrderProtocolRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                // Discontinue claimed/in-progress protocol step
                ProtocolAssignmentStep existingAssignmentStep = InprogressProcedureStep<ProtocolAssignmentStep>(rp);

                if (existingAssignmentStep != null)
                {
                    existingAssignmentStep.Discontinue();

                    // Replace with new step scheduled step
                    ProtocolAssignmentStep replacementAssignmentStep = new ProtocolAssignmentStep(existingAssignmentStep.Protocol);
                    rp.AddProcedureStep(replacementAssignmentStep);

                    replacementAssignmentStep.Schedule(DateTime.Now);

                    this.PersistenceContext.Lock(replacementAssignmentStep, DirtyState.New);
                }
            }

            return new DiscardOrderProtocolResponse();
        }

        [UpdateOperation]
        public AcceptOrderProtocolResponse AcceptOrderProtocol(AcceptOrderProtocolRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                ProtocolAssignmentStep assignmentStep = InprogressProcedureStep<ProtocolAssignmentStep>(rp);

                if(assignmentStep != null)
                {
                    assignmentStep.Complete();
                    assignmentStep.Protocol.Accept();
                }
            }

            return new AcceptOrderProtocolResponse();
        }

        [UpdateOperation]
        public RejectOrderProtocolResponse RejectOrderProtocol(RejectOrderProtocolRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                ProtocolAssignmentStep assignmentStep = InprogressProcedureStep<ProtocolAssignmentStep>(rp);

                if (assignmentStep != null)
                {
                    assignmentStep.Discontinue();
                    assignmentStep.Protocol.Reject(EnumUtils.GetEnumValue<ProtocolSuspendRejectReasonEnum>(request.RejectReason, this.PersistenceContext));

                    ProtocolResolutionStep resolutionStep = new ProtocolResolutionStep(assignmentStep.Protocol);
                    rp.AddProcedureStep(resolutionStep);
                }
            }

            return new RejectOrderProtocolResponse();
        }

        [UpdateOperation]
        public SuspendOrderProtocolResponse SuspendOrderProtocol(SuspendOrderProtocolRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                ProtocolAssignmentStep assignmentStep = InprogressProcedureStep<ProtocolAssignmentStep>(rp);

                if (assignmentStep != null)
                {
                    assignmentStep.Discontinue();
                    assignmentStep.Protocol.Suspend(EnumUtils.GetEnumValue<ProtocolSuspendRejectReasonEnum>(request.SuspendReason, this.PersistenceContext));

                    ProtocolResolutionStep resolutionStep = new ProtocolResolutionStep(assignmentStep.Protocol);
                    rp.AddProcedureStep(resolutionStep);
                }
            }

            return new SuspendOrderProtocolResponse();
        }

        [UpdateOperation]
        public SaveProtocolResponse SaveProtocol(SaveProtocolRequest request)
        {
            Protocol protocol = this.PersistenceContext.Load<Protocol>(request.ProtocolRef);
            ProtocollingWorkflowAssembler assembler = new ProtocollingWorkflowAssembler();

            assembler.UpdateProtocol(protocol, request.ProtocolDetail, this.PersistenceContext);

            return new SaveProtocolResponse();
        }

        [UpdateOperation]
        public ResubmitProtocolResponse ResubmitProtocol(ResubmitProtocolRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                ProtocolResolutionStep resolutionStep = ScheduledProcedureStep<ProtocolResolutionStep>(rp);

                if (resolutionStep != null)
                {
                    resolutionStep.Complete(this.CurrentUserStaff);
                    resolutionStep.Protocol.Resolve();
                    ProtocolAssignmentStep assignmentStep = new ProtocolAssignmentStep(resolutionStep.Protocol);
                    rp.AddProcedureStep(assignmentStep);
                }
            }

            return new ResubmitProtocolResponse();
        }

        [UpdateOperation]
        public CancelProtocolAndOrderResponse CancelProtocolAndOrder(CancelProtocolAndOrderRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                ProtocolResolutionStep resolutionStep = ScheduledProcedureStep<ProtocolResolutionStep>(rp);

                if (resolutionStep != null)
                {
                    resolutionStep.Complete(this.CurrentUserStaff);
                }
            }

            EnumValueInfo reason =
                CollectionUtils.FirstElement<EnumValueInfo>(EnumUtils.GetEnumValueList<OrderCancelReasonEnum>(this.PersistenceContext));
            order.Discontinue(EnumUtils.GetEnumValue<OrderCancelReasonEnum>(reason, this.PersistenceContext));

            return new CancelProtocolAndOrderResponse();
        }

        [UpdateOperation]
        public ApproveResidentProtocolResponse ApproveResidentProtocol(ApproveResidentProtocolRequest request)
        {
            Protocol protocol = this.PersistenceContext.Load<Protocol>(request.ProtocolRef);

            protocol.Accept();

            this.PersistenceContext.SynchState();
            
            ProtocollingWorkflowAssembler assembler = new ProtocollingWorkflowAssembler();
            return new ApproveResidentProtocolResponse(assembler.CreateProtocolDetail(protocol, this.PersistenceContext));
        }

        #endregion

        private T InprogressProcedureStep<T>(RequestedProcedure rp) where T : ProcedureStep
        {
            return CurrentProcedureStep<T>(rp, ActivityStatus.IP);
        }

        private T ScheduledProcedureStep<T>(RequestedProcedure rp) where T : ProcedureStep
        {
            return CurrentProcedureStep<T>(rp, ActivityStatus.SC);
        }

        private T CurrentProcedureStep<T>(RequestedProcedure rp, ActivityStatus status) where T : ProcedureStep
        {
            ProcedureStep uncastProcedureStep = CollectionUtils.SelectFirst<ProcedureStep>(
                rp.ProcedureSteps,
                delegate(ProcedureStep ps) { return ps.Is<T>() && ps.State == status; });

            return uncastProcedureStep != null ? uncastProcedureStep.Downcast<T>() : null;
        }
    }
}
