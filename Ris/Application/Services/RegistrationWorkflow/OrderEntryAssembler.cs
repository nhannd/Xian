#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Services.Admin;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    public class OrderEntryAssembler
    {
        public OrderDetail CreateOrderDetail(Order order, IPersistenceContext context)
        {
            OrderDetail detail = new OrderDetail();

            VisitAssembler visitAssembler = new VisitAssembler();
            ExternalPractitionerAssembler pracAssembler = new ExternalPractitionerAssembler();
            FacilityAssembler facilityAssembler = new FacilityAssembler();

            detail.OrderRef = order.GetRef();
            detail.PatientRef = order.Patient.GetRef();
            detail.Visit = visitAssembler.CreateVisitDetail(order.Visit, context);
            detail.PlacerNumber = order.PlacerNumber;
            detail.AccessionNumber = order.AccessionNumber;
            detail.DiagnosticService = this.CreateDiagnosticServiceDetail(order.DiagnosticService);
            detail.EnteredDateTime = order.EnteredDateTime;
            detail.SchedulingRequestDateTime = order.SchedulingRequestDateTime;
            detail.OrderingPractitioner = pracAssembler.CreateExternalPractitionerDetail(order.OrderingPractitioner, context);
            detail.OrderingFacility = facilityAssembler.CreateFacilityDetail(order.OrderingFacility);
            detail.ReasonForStudy = order.ReasonForStudy;
            detail.OrderPriority = EnumUtils.GetEnumValueInfo(order.Priority, context);
            detail.CancelReason = EnumUtils.GetEnumValueInfo(order.CancelReason);

            foreach (RequestedProcedure rp in order.RequestedProcedures)
            {
                detail.RequestedProcedures.Add(this.CreateRequestedProcedureSummary(rp, context));
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

        public RequestedProcedureSummary CreateRequestedProcedureSummary(RequestedProcedure rp, IPersistenceContext context)
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
