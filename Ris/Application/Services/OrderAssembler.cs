using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Application.Services
{
    public class OrderAssembler
    {
        public OrderDetail CreateOrderDetail(Order order, IPersistenceContext context)
        {
            return CreateOrderDetail(order, context, true, true, true);
        }

        public OrderDetail CreateOrderDetail(Order order, IPersistenceContext context,
            bool includeVisit,
            bool includeRequestedProcedures,
            bool includeNotes)
        {
            OrderDetail detail = new OrderDetail();

            ExternalPractitionerAssembler pracAssembler = new ExternalPractitionerAssembler();
            FacilityAssembler facilityAssembler = new FacilityAssembler();
            DiagnosticServiceAssembler dsAssembler = new DiagnosticServiceAssembler();
            RequestedProcedureAssembler rpAssembler = new RequestedProcedureAssembler();

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
            detail.EnteredDateTime = order.EnteredTime;
            detail.SchedulingRequestDateTime = order.SchedulingRequestTime;
            detail.OrderingPractitioner = pracAssembler.CreateExternalPractitionerDetail(order.OrderingPractitioner, context);
            detail.OrderingFacility = facilityAssembler.CreateFacilityDetail(order.OrderingFacility);
            detail.ReasonForStudy = order.ReasonForStudy;
            detail.OrderPriority = EnumUtils.GetEnumValueInfo(order.Priority, context);
            detail.CancelReason = EnumUtils.GetEnumValueInfo(order.CancelReason);

            if (includeRequestedProcedures)
            {
                detail.RequestedProcedures = CollectionUtils.Map<RequestedProcedure, RequestedProcedureDetail>(order.RequestedProcedures,
                    delegate(RequestedProcedure rp)
                    {
                        return rpAssembler.CreateRequestedProcedureDetail(rp, context);
                    });
            }

            if(includeNotes)
            {
                OrderNoteAssembler orderNoteAssembler = new OrderNoteAssembler();
                detail.Notes = CollectionUtils.Map<OrderNote, OrderNoteDetail>(order.Notes,
                    delegate(OrderNote note)
                    {
                        return orderNoteAssembler.CreateOrderNoteDetail(note, context);
                    });
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
            summary.EnteredDateTime = order.EnteredTime;
            summary.SchedulingRequestDateTime = order.SchedulingRequestTime;
            summary.OrderingPractitioner = pracAssembler.CreateExternalPractitionerDetail(order.OrderingPractitioner, context);
            summary.OrderingFacility = order.OrderingFacility.Name;
            summary.ReasonForStudy = order.ReasonForStudy;
            summary.OrderPriority = EnumUtils.GetEnumValueInfo(order.Priority, context);
            summary.OrderStatus = EnumUtils.GetEnumValueInfo(order.Status, context);

            return summary;
        }
    }
}
