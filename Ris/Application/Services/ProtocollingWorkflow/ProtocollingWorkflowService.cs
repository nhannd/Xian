using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;

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

        [UpdateOperation]
        public AddProtocolResponse AddProtocol(AddProtocolRequest request)
        {
            //RequestedProcedure rp = this.PersistenceContext.Load<RequestedProcedure>(request.RequestedProcedureRef);
            Order o = this.PersistenceContext.Load<Order>(request.RequestedProcedureRef);
            RequestedProcedure rp = CollectionUtils.FirstElement<RequestedProcedure>(o.RequestedProcedures);

            Protocol protocol = new Protocol(rp);
            ProtocolProcedureStep protocolStep = new ProtocolProcedureStep(protocol);
            rp.AddProcedureStep(protocolStep);

            protocolStep.Schedule(DateTime.Now);

            this.PersistenceContext.Lock(protocol, DirtyState.New);
            this.PersistenceContext.Lock(protocolStep, DirtyState.New);
            
            this.PersistenceContext.SynchState();

            ProtocollingWorkflowAssembler assembler = new ProtocollingWorkflowAssembler();
            return new AddProtocolResponse(assembler.CreateProtocolDetail(protocol, this.PersistenceContext));
        }

        [UpdateOperation]
        public AcceptProtocolResponse AcceptProtocol(AcceptProtocolRequest request)
        {
            ProtocolProcedureStep protocolStep = this.PersistenceContext.Load<ProtocolProcedureStep>(request.ProtocolRef);
            throw new System.NotImplementedException();
        }

        [UpdateOperation]
        public RejectProtocolResponse RejectProtocol(RejectProtocolRequest request)
        {
            throw new System.NotImplementedException();
        }

        [UpdateOperation]
        public SuspendProtocolResponse SuspendProtocol(SuspendProtocolRequest request)
        {
            throw new System.NotImplementedException();
        }

        [UpdateOperation]
        public ResolveProtocolResponse ResolveProtocol(ResolveProtocolRequest request)
        {
            throw new System.NotImplementedException();
        }

        [UpdateOperation]
        public ApproveResidentProtocolResponse ApproveResidentProtocol(ApproveResidentProtocolRequest request)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
