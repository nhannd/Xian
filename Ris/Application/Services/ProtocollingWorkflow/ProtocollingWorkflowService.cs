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
        public ListProtocolCodesResponse ListProtocolCodes(ListProtocolCodesRequest request)
        {
            ProtocollingWorkflowAssembler assembler = new ProtocollingWorkflowAssembler();

            List<ProtocolCodeDetail> codes = CollectionUtils.Map<ProtocolCode, ProtocolCodeDetail>(
                this.PersistenceContext.GetBroker<IProtocolCodeBroker>().FindAll(),
                delegate(ProtocolCode pc) { return assembler.CreateProtocolCodeDetail(pc); });

            return new ListProtocolCodesResponse(codes);
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
            return new GetProcedureProtocolResponse(assembler.CreateProtocolDetail(rp.ProtocolProcedureStep.Protocol, this.PersistenceContext));
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
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);
            RequestedProcedure protocolledRequestedProcedure = CollectionUtils.SelectFirst<RequestedProcedure>(
                order.RequestedProcedures,
                delegate(RequestedProcedure rp) { return rp.ProtocolProcedureStep != null; });

            ProtocolProcedureStep protocolStep = protocolledRequestedProcedure.ProtocolProcedureStep;

            GetProtocolOperationEnablementResponse response = new GetProtocolOperationEnablementResponse();

            response.AcceptEnabled = protocolStep.CanAccept;
            response.RejectEnabled = protocolStep.CanReject;
            response.SuspendEnabled = protocolStep.CanSuspend;
            response.SaveEnabled = protocolStep.CanSave;

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
                if(rp.ProtocolProcedureStep != null)
                    throw new RequestValidationException("Protocol step already exists for one or more requested procedures.  Probably stale data.");

                Protocol protocol = new Protocol(rp);
                ProtocolProcedureStep protocolStep = new ProtocolProcedureStep(protocol);
                rp.AddProcedureStep(protocolStep);

                protocolStep.Schedule(DateTime.Now);

                this.PersistenceContext.Lock(protocol, DirtyState.New);
                this.PersistenceContext.Lock(protocolStep, DirtyState.New);
            }
            
            this.PersistenceContext.SynchState();

            return new AddOrderProtocolStepsResponse();
        }

        [UpdateOperation]
        public AcceptOrderProtocolResponse AcceptOrderProtocol(AcceptOrderProtocolRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                if(rp.ProtocolProcedureStep != null)
                {
                    if(rp.ProtocolProcedureStep.Performer != null)
                        rp.ProtocolProcedureStep.Complete();
                    else 
                        rp.ProtocolProcedureStep.Complete(this.CurrentUserStaff);

                    rp.ProtocolProcedureStep.Protocol.Accept();
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
                if (rp.ProtocolProcedureStep != null)
                {
                    rp.ProtocolProcedureStep.Suspend();
                    rp.ProtocolProcedureStep.Protocol.Reject(EnumUtils.GetEnumValue<ProtocolSuspendRejectReasonEnum>(request.RejectReason, this.PersistenceContext));
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
                if (rp.ProtocolProcedureStep != null)
                {
                    rp.ProtocolProcedureStep.Suspend();
                    rp.ProtocolProcedureStep.Protocol.Suspend(EnumUtils.GetEnumValue<ProtocolSuspendRejectReasonEnum>(request.SuspendReason, this.PersistenceContext));
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
        public ResolveOrderProtocolResponse ResolveOrderProtocol(ResolveOrderProtocolRequest request)
        {
            Order order = this.PersistenceContext.Load<Order>(request.OrderRef);

            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                if (rp.ProtocolProcedureStep != null)
                {
                    rp.ProtocolProcedureStep.Resume();
                    rp.ProtocolProcedureStep.Protocol.Resolve();
                }
            }

            return new ResolveOrderProtocolResponse();
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
    }
}
