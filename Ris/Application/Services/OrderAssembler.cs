#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Common.Utilities;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Services
{
    public class OrderAssembler
    {
        public OrderDetail CreateOrderDetail(Order order, IPersistenceContext context)
        {
            return CreateOrderDetail(order, context, true, true, true, null);
        }

        public OrderDetail CreateOrderDetail(Order order, IPersistenceContext context,
            bool includeVisit,
            bool includeProcedures,
            bool includeNotes,
            IList<string> noteCategoriesFilter)
        {
            OrderDetail detail = new OrderDetail();

            ExternalPractitionerAssembler pracAssembler = new ExternalPractitionerAssembler();
            FacilityAssembler facilityAssembler = new FacilityAssembler();
            DiagnosticServiceAssembler dsAssembler = new DiagnosticServiceAssembler();
            ProcedureAssembler rpAssembler = new ProcedureAssembler();

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
            detail.EnteredTime = order.EnteredTime;
            detail.SchedulingRequestTime = order.SchedulingRequestTime;
            detail.OrderingPractitioner = pracAssembler.CreateExternalPractitionerDetail(order.OrderingPractitioner, context);
            detail.OrderingFacility = facilityAssembler.CreateFacilityDetail(order.OrderingFacility);
            detail.ReasonForStudy = order.ReasonForStudy;
            detail.OrderPriority = EnumUtils.GetEnumValueInfo(order.Priority, context);
            detail.CancelReason = EnumUtils.GetEnumValueInfo(order.CancelReason);

            if (includeProcedures)
            {
                detail.Procedures = CollectionUtils.Map<Procedure, ProcedureDetail>(order.Procedures,
                    delegate(Procedure rp)
                    {
                        return rpAssembler.CreateProcedureDetail(rp, context);
                    });
            }

            if(includeNotes)
            {
                OrderNoteAssembler orderNoteAssembler = new OrderNoteAssembler();
                List<OrderNote> notes = new List<OrderNote>(order.Notes);

                // apply category filter, if provided
                if(noteCategoriesFilter != null && noteCategoriesFilter.Count > 0)
                {
                    notes = CollectionUtils.Select(notes,
                        delegate(OrderNote n) { return noteCategoriesFilter.Contains(n.Category); });
                }

                // sort notes by post-time (guaranteed non-null because only "posted" notes are in this collection)
                notes.Sort(delegate(OrderNote x, OrderNote y) { return x.PostTime.Value.CompareTo(y.PostTime.Value); });

                detail.Notes = CollectionUtils.Map<OrderNote, OrderNoteSummary>(notes,
                    delegate(OrderNote note)
                    {
                        return orderNoteAssembler.CreateOrderNoteSummary(note, context);
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
            summary.EnteredTime = order.EnteredTime;
            summary.SchedulingRequestTime = order.SchedulingRequestTime;
            summary.OrderingPractitioner = pracAssembler.CreateExternalPractitionerSummary(order.OrderingPractitioner, context);
            summary.OrderingFacility = order.OrderingFacility.Name;
            summary.ReasonForStudy = order.ReasonForStudy;
            summary.OrderPriority = EnumUtils.GetEnumValueInfo(order.Priority, context);
            summary.OrderStatus = EnumUtils.GetEnumValueInfo(order.Status, context);

            return summary;
        }
    }
}
