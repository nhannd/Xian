using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Services.Admin;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    public class OrderEntryAssembler
    {
        public OrderDetail CreateOrderDetail(Order order, IPersistenceContext context)
        {
            OrderDetail detail = new OrderDetail();

            VisitAssembler visitAssembler = new VisitAssembler();
            StaffAssembler StaffAssembler = new StaffAssembler();
            FacilityAssembler facilityAssembler = new FacilityAssembler();

            detail.PatientRef = order.Patient.GetRef();
            detail.Visit = visitAssembler.CreateVisitDetail(order.Visit, context);
            detail.PlacerNumber = order.PlacerNumber;
            detail.AccessionNumber = order.AccessionNumber;
            detail.DiagnosticService = this.CreateDiagnosticServiceDetail(order.DiagnosticService);
            detail.EnteredDateTime = order.EnteredDateTime;
            detail.SchedulingRequestDateTime = order.SchedulingRequestDateTime;
            detail.OrderingPractitioner = StaffAssembler.CreateStaffDetail(order.OrderingPractitioner, context);
            detail.OrderingFacility = facilityAssembler.CreateFacilityDetail(order.OrderingFacility);
            detail.ReasonForStudy = order.ReasonForStudy;
            detail.OrderPriority = EnumUtils.GetEnumValueInfo(order.Priority, context);
            detail.CancelReason = EnumUtils.GetEnumValueInfo(order.CancelReason);

            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                detail.RequestedProcedures.Add(this.CreateRequestedProcedureDetail(rp, context));
            }

            return detail;
        }

        public OrderSummary CreateOrderSummary(Order order, IPersistenceContext context)
        {
            StaffAssembler StaffAssembler = new StaffAssembler();

            OrderSummary summary = new OrderSummary();

            summary.OrderRef = order.GetRef();
            summary.AccessionNumber = order.AccessionNumber;
            summary.DiagnosticServiceName = order.DiagnosticService.Name;
            summary.EnteredDateTime = order.EnteredDateTime;
            summary.SchedulingRequestDateTime = order.SchedulingRequestDateTime;
            summary.OrderingPractitioner = StaffAssembler.CreateStaffDetail(order.OrderingPractitioner, context);
            summary.OrderingFacility = order.OrderingFacility.Name;
            summary.ReasonForStudy = order.ReasonForStudy;
            summary.OrderPriority = EnumUtils.GetEnumValueInfo(order.Priority, context);

            return summary;
        }

        public DiagnosticServiceSummary CreateDiagnosticServiceSummary(DiagnosticService diagnosticService)
        {
            return new DiagnosticServiceSummary(
                diagnosticService.GetRef(),
                diagnosticService.Id,
                diagnosticService.Name);
        }

        public DiagnosticServiceDetail CreateDiagnosticServiceDetail(DiagnosticService diagnosticService)
        {
            return new DiagnosticServiceDetail(
                diagnosticService.Id,
                diagnosticService.Name,
                CollectionUtils.Map<RequestedProcedureType, RequestedProcedureTypeDetail, List<RequestedProcedureTypeDetail>>(
                    diagnosticService.RequestedProcedureTypes,
                    delegate(RequestedProcedureType rpType)
                    {
                        return CreateRequestedProcedureTypeDetail(rpType);
                    }));
        }

        public DiagnosticServiceTreeItem CreateDiagnosticServiceTreeItem(DiagnosticServiceTreeNode node)
        {
            return new DiagnosticServiceTreeItem(
                node.GetRef(),
                node.Description,
                node.DiagnosticService == null ? null : CreateDiagnosticServiceSummary(node.DiagnosticService));
        }

        public RequestedProcedureSummary CreateRequestedProcedureDetail(RequestedProcedure rp, IPersistenceContext context)
        {
            RequestedProcedureSummary detail = new RequestedProcedureSummary();

            detail.OrderRef = rp.Order.GetRef();
            detail.Index = rp.Index;
            detail.Type = this.CreateRequestedProcedureTypeDetail(rp.Type);

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

        public RequestedProcedureTypeDetail CreateRequestedProcedureTypeDetail(RequestedProcedureType requestedProcedureType)
        {
            return new RequestedProcedureTypeDetail(
                requestedProcedureType.Id,
                requestedProcedureType.Name,
                CollectionUtils.Map<ModalityProcedureStepType, ModalityProcedureStepTypeDetail, List<ModalityProcedureStepTypeDetail>>(
                    requestedProcedureType.ModalityProcedureStepTypes,
                    delegate(ModalityProcedureStepType mpsType)
                    {
                        return CreateModalityProcedureStepTypeDetail(mpsType);
                    }));
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
