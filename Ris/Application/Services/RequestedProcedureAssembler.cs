using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class RequestedProcedureAssembler
    {
        public RequestedProcedureDetail CreateRequestedProcedureDetail(RequestedProcedure rp, IPersistenceContext context)
        {
            return CreateRequestedProcedureDetail(rp, true, true, context);
        }

        public RequestedProcedureDetail CreateRequestedProcedureDetail(RequestedProcedure rp, bool includeProcedureSteps, bool includeProtocol, IPersistenceContext context)
        {
            RequestedProcedureDetail detail = new RequestedProcedureDetail();

            detail.RequestedProcedureRef = rp.GetRef();
            detail.Name = rp.Type.Name;
            detail.Status = EnumUtils.GetEnumValueInfo(rp.Status, context);
            detail.Type = new RequestedProcedureTypeAssembler().CreateRequestedProcedureTypeDetail(rp.Type);

            if (includeProcedureSteps)
            {
                //TODO: what about other kinds of procedure steps ??
                ModalityProcedureStepAssembler modalityProcedureStepAssembler = new ModalityProcedureStepAssembler();
                detail.ModalityProcedureSteps = CollectionUtils.Map<ModalityProcedureStep, ModalityProcedureStepDetail>(
                    rp.ModalityProcedureSteps,
                    delegate(ModalityProcedureStep mp)
                    { return modalityProcedureStepAssembler.CreateModalityProcedureStepDetail(mp, context); });
            }

            // the Protocol may be null, if this procedure has not been protocolled
            if(includeProtocol && rp.Protocol != null)
            {
                ProtocolAssembler protocolAssembler = new ProtocolAssembler();
                detail.Protocol = protocolAssembler.CreateProtocolDetail(rp.Protocol, context);
            }

            return detail;
        }

        public RequestedProcedureSummary CreateRequestedProcedureSummary(RequestedProcedure rp, IPersistenceContext context)
        {
            RequestedProcedureTypeAssembler rptAssembler = new RequestedProcedureTypeAssembler();
            RequestedProcedureSummary summary = new RequestedProcedureSummary();

            summary.OrderRef = rp.Order.GetRef();
            summary.RequestedProcedureRef = rp.GetRef();
            summary.Index = rp.Index;
            summary.ScheduledStartTime = rp.ScheduledStartTime;
            summary.PerformingFacility = new FacilityAssembler().CreateFacilitySummary(rp.PerformingFacility);
            summary.Type = rptAssembler.CreateRequestedProcedureTypeSummary(rp.Type);

            return summary;
        }
    }
}
