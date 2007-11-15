using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class OrderAssembler
    {
        public OrderDetail CreateOrderDetail(Order order, IPersistenceContext context)
        {
            return CreateOrderDetail(order, context, true, true);
        }

        public OrderDetail CreateOrderDetail(Order order, IPersistenceContext context,
            bool includeVisit,
            bool includeRequestedProcedures)
        {
            OrderDetail detail = new OrderDetail();

            ExternalPractitionerAssembler pracAssembler = new ExternalPractitionerAssembler();
            FacilityAssembler facilityAssembler = new FacilityAssembler();
            DiagnosticServiceAssembler dsAssembler = new DiagnosticServiceAssembler();

            detail.OrderRef = order.GetRef();
            detail.PatientRef = order.Patient.GetRef();

            if (includeVisit)
            {
                VisitAssembler visitAssembler = new VisitAssembler();
                detail.Visit = visitAssembler.CreateVisitDetail(order.Visit, context);
            }

            detail.PlacerNumber = order.PlacerNumber;
            detail.AccessionNumber = order.AccessionNumber;
            detail.DiagnosticService = dsAssembler.CreateDiagnosticServiceDetail(order.DiagnosticService);
            detail.EnteredDateTime = order.EnteredDateTime;
            detail.SchedulingRequestDateTime = order.SchedulingRequestDateTime;
            detail.OrderingPractitioner = pracAssembler.CreateExternalPractitionerDetail(order.OrderingPractitioner, context);
            detail.OrderingFacility = facilityAssembler.CreateFacilityDetail(order.OrderingFacility);
            detail.ReasonForStudy = order.ReasonForStudy;
            detail.OrderPriority = EnumUtils.GetEnumValueInfo(order.Priority, context);
            detail.CancelReason = EnumUtils.GetEnumValueInfo(order.CancelReason);

            if (includeRequestedProcedures)
            {
                foreach (RequestedProcedure rp in order.RequestedProcedures)
                {
                    detail.RequestedProcedures.Add(this.CreateRequestedProcedureSummary(rp, context));
                }
            }

            return detail;
        }

        public OrderSummary CreateOrderSummary(Order order, IPersistenceContext context)
        {
            ExternalPractitionerAssembler pracAssembler = new ExternalPractitionerAssembler();

            OrderSummary summary = new OrderSummary();

            summary.OrderRef = order.GetRef();
            summary.AccessionNumber = order.AccessionNumber;
            summary.DiagnosticServiceName = order.DiagnosticService.Name;
            summary.EnteredDateTime = order.EnteredDateTime;
            summary.SchedulingRequestDateTime = order.SchedulingRequestDateTime;
            summary.OrderingPractitioner = pracAssembler.CreateExternalPractitionerDetail(order.OrderingPractitioner, context);
            summary.OrderingFacility = order.OrderingFacility.Name;
            summary.ReasonForStudy = order.ReasonForStudy;
            summary.OrderPriority = EnumUtils.GetEnumValueInfo(order.Priority, context);

            return summary;
        }

        public RequestedProcedureSummary CreateRequestedProcedureSummary(RequestedProcedure rp, IPersistenceContext context)
        {
            RequestedProcedureTypeAssembler rptAssembler = new RequestedProcedureTypeAssembler();
            RequestedProcedureSummary detail = new RequestedProcedureSummary();

            detail.OrderRef = rp.Order.GetRef();
            detail.Index = rp.Index;
            detail.Type = rptAssembler.CreateRequestedProcedureTypeDetail(rp.Type);

            foreach (ProcedureStep step in rp.ProcedureSteps)
            {
                //TODO: include other ProcedureStep in RequestedProcedureSummary
                if (step.Is<ModalityProcedureStep>())
                {
                    detail.ProcedureSteps.Add(this.CreateModalityProcedureStepSummary(step.Downcast<ModalityProcedureStep>(), context));
                }
            }

            return detail;
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
