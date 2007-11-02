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
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using Iesi.Collections;
using Iesi.Collections.Generic;

namespace ClearCanvas.Ris.Application.Services.RegistrationWorkflow
{
    public class OrderEntryAssembler
    {
        public OrderRequisition CreateOrderRequisition(Order order, IPersistenceContext context)
        {
            VisitAssembler visitAssembler = new VisitAssembler();
            ExternalPractitionerAssembler pracAssembler = new ExternalPractitionerAssembler();
            FacilityAssembler facilityAssembler = new FacilityAssembler();
            DiagnosticServiceAssembler dsAssembler = new DiagnosticServiceAssembler();

            OrderRequisition requisition = new OrderRequisition();
            requisition.Patient = order.Patient.GetRef();
            requisition.Visit = visitAssembler.CreateVisitSummary(order.Visit, context);
            requisition.DiagnosticService = dsAssembler.CreateDiagnosticServiceSummary(order.DiagnosticService);
            requisition.SchedulingRequestTime = order.SchedulingRequestDateTime;
            requisition.OrderingPractitioner = pracAssembler.CreateExternalPractitionerSummary(order.OrderingPractitioner, context);
            //TODO requisition.PerformingFacility = facilityAssembler.CreateFacilitySummary(order.PerformingFacility);
            requisition.OrderingFacility = facilityAssembler.CreateFacilitySummary(order.OrderingFacility);
            requisition.ReasonForStudy = order.ReasonForStudy;
            requisition.Priority = EnumUtils.GetEnumValueInfo(order.Priority, context);
            requisition.CopiesToPractitioners = CollectionUtils.Map<ExternalPractitioner, ExternalPractitionerSummary>(
                order.ResultCopiesToPractitioners,
                delegate(ExternalPractitioner p)
                {
                    return pracAssembler.CreateExternalPractitionerSummary(p, context);
                });

            requisition.RequestedProcedures = CollectionUtils.Map<RequestedProcedure, ProcedureRequisition>(
                order.RequestedProcedures,
                delegate(RequestedProcedure rp)
                {
                    return CreateProcedureRequisition(rp, context);
                });

            return requisition;
        }

        public void UpdateOrderFromRequisition(Order order, OrderRequisition requisition, IPersistenceContext context)
        {
            // only certain properties of an order may be updated from a requisition
            // Patient cannot not be updated
            // DiagnosticService cannot be updated
            // OrderingFacility cannot be updated - or can it ???

            // do not update the individual requested procedures, as this is done separately - see UpdateProcedureFromRequisition


            order.Visit = context.Load<Visit>(requisition.Visit.VisitRef, EntityLoadFlags.Proxy);
            order.SchedulingRequestDateTime = requisition.SchedulingRequestTime;
            order.OrderingPractitioner = context.Load<ExternalPractitioner>(requisition.OrderingPractitioner.PractitionerRef, EntityLoadFlags.Proxy);
            //TODO order.PerformingFacility = context.Load<Facility>(requisition.PerformingFacility.FacilityRef, EntityLoadFlags.Proxy);
            order.ReasonForStudy = requisition.ReasonForStudy;
            order.Priority = EnumUtils.GetEnumValue<OrderPriority>(requisition.Priority);

            // wipe out and reset the copies to practitioners
            order.ResultCopiesToPractitioners.Clear();
            order.ResultCopiesToPractitioners.AddAll(CollectionUtils.Map<ExternalPractitionerSummary, ExternalPractitioner>(
                requisition.CopiesToPractitioners,
                delegate(ExternalPractitionerSummary s)
                {
                    return context.Load<ExternalPractitioner>(s.PractitionerRef, EntityLoadFlags.Proxy);
                }));
        }

        public ProcedureRequisition CreateProcedureRequisition(RequestedProcedure rp, IPersistenceContext context)
        {
            RequestedProcedureTypeAssembler rptAssembler = new RequestedProcedureTypeAssembler();
            FacilityAssembler facilityAssembler = new FacilityAssembler();

            // create requisition - canModify is true iff the procedure is in SC status
            return new ProcedureRequisition(
                rptAssembler.GetRequestedProcedureTypeSummary(rp.Type),
                rp.Index,
                rp.ScheduledStartTime,
                facilityAssembler.CreateFacilitySummary(rp.PerformingFacility),
                EnumUtils.GetEnumValueInfo(rp.Laterality, context),
                false, //TODO portable modality
                EnumUtils.GetEnumValueInfo(rp.Status, context),
                rp.Status == RequestedProcedureStatus.SC);
        }

        public void UpdateProcedureFromRequisition(RequestedProcedure rp, ProcedureRequisition requisition, IPersistenceContext context)
        {
            rp.Schedule(requisition.ScheduledTime);
            rp.PerformingFacility = context.Load<Facility>(requisition.PerformingFacility.FacilityRef, EntityLoadFlags.Proxy);
            rp.Laterality = EnumUtils.GetEnumValue<Laterality>(requisition.Laterality);
            //TODO portable
        }

        public DiagnosticServiceTreeItem CreateDiagnosticServiceTreeItem(DiagnosticServiceTreeNode node)
        {
            DiagnosticServiceAssembler dsAssembler = new DiagnosticServiceAssembler();
            return new DiagnosticServiceTreeItem(
                node.GetRef(),
                node.Description,
                node.DiagnosticService == null ? null : dsAssembler.CreateDiagnosticServiceSummary(node.DiagnosticService));
        }
    }
}
