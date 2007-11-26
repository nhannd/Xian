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
            summary.Type = rptAssembler.CreateRequestedProcedureTypeDetail(rp.Type);

            foreach (ProcedureStep step in rp.ProcedureSteps)
            {
                //TODO: include other ProcedureStep in RequestedProcedureSummary
                if (step.Is<ModalityProcedureStep>())
                {
                    summary.ProcedureSteps.Add(this.CreateModalityProcedureStepSummary(step.Downcast<ModalityProcedureStep>(), context));
                }
            }

            return summary;
        }

        public ModalityProcedureStepSummary CreateModalityProcedureStepSummary(ModalityProcedureStep mps, IPersistenceContext context)
        {
            StaffAssembler staffAssembler = new StaffAssembler();

            ModalityProcedureStepSummary summary = new ModalityProcedureStepSummary();

            summary.Type = this.CreateModalityProcedureStepTypeDetail(mps.Type);

            summary.State = EnumUtils.GetEnumValueInfo(mps.State, context);

            summary.PerformerStaff = staffAssembler.CreateStaffSummary(mps.PerformingStaff, context);
            summary.StartTime = mps.StartTime;
            summary.EndTime = mps.EndTime;

            if (mps.Scheduling != null)
            {
                //TODO ScheduledPerformerStaff for ModalityProcedureStepSummary
                //summary.ScheduledPerformerStaff = staffAssembler.CreateStaffSummary(mps.Scheduling.Performer);
                summary.ScheduledStartTime = mps.Scheduling.StartTime;
                summary.ScheduledEndTime = mps.Scheduling.EndTime;
            }

            return summary;
        }

        public ModalityProcedureStepTypeDetail CreateModalityProcedureStepTypeDetail(ModalityProcedureStepType modalityProcedureStepType)
        {
            ModalityAssembler assembler = new ModalityAssembler();
            return new ModalityProcedureStepTypeDetail(
                modalityProcedureStepType.Id,
                modalityProcedureStepType.Name,
                assembler.CreateModalityDetail(modalityProcedureStepType.DefaultModality));
        }
    }
}
