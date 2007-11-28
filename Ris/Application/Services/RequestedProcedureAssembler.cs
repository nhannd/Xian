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
            RequestedProcedureDetail detail = new RequestedProcedureDetail();

            ModalityProcedureStepAssembler modalityProcedureStepAssembler = new ModalityProcedureStepAssembler();
            ProtocolProcedureStepAssembler protocolProcedureStepAssembler = new ProtocolProcedureStepAssembler();

            detail.RequestedProcedureRef = rp.GetRef();
            detail.Name = rp.Type.Name;
            detail.Status = EnumUtils.GetEnumValueInfo(rp.Status, context);
            detail.Type = new RequestedProcedureTypeAssembler().CreateRequestedProcedureTypeDetail(rp.Type);
            detail.ModalityProcedureSteps = CollectionUtils.Map<ModalityProcedureStep, ModalityProcedureStepDetail>(
                rp.ModalityProcedureSteps,
                delegate(ModalityProcedureStep mp) { return modalityProcedureStepAssembler.CreateModalityProcedureStepDetail(mp, context); });
            detail.ProtocolProcedureStepDetail = rp.ProtocolProcedureStep != null
                ? protocolProcedureStepAssembler.CreateProtocolProcedureStepDetail(rp.ProtocolProcedureStep, context)
                : null;

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
