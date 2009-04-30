#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;

namespace ClearCanvas.Ris.Application.Services.BrowsePatientData
{
    public class BrowsePatientDataAssembler
    {
        public OrderListItem CreateOrderListItem(Order order, IPersistenceContext context)
        {
            OrderListItem data = new OrderListItem();

            UpdateListItem(data, order, context);
            UpdateListItem(data, order.Visit, context);

            return data;
        }

        public OrderListItem CreateOrderListItem(Procedure rp, IPersistenceContext context)
        {
            OrderListItem data = new OrderListItem();

            UpdateListItem(data, rp.Order, context);
            UpdateListItem(data, rp.Order.Visit, context);
            UpdateListItem(data, rp, context);

            return data;
        }

		public ReportListItem CreateReportListItem(Report report, Procedure rp, IPersistenceContext context)
		{
			ReportListItem data = new ReportListItem();

			UpdateListItem(data, rp.Order, context);
			UpdateListItem(data, rp.Order.Visit, context);
			UpdateListItem(data, rp, context);
			UpdateListItem(data, report, context);

			return data;
		}

		public VisitListItem CreateVisitListItem(Visit visit, IPersistenceContext context)
		{
			VisitListItem data = new VisitListItem();

			UpdateListItem(data, visit, context);

			return data;
		}

        #region Private Helpers

        private void UpdateListItem(VisitListItem data, Visit visit, IPersistenceContext context)
        {
            FacilityAssembler facilityAssembler = new FacilityAssembler();

            data.VisitRef = visit.GetRef();
            data.VisitNumber = new CompositeIdentifierDetail(visit.VisitNumber.Id,
                EnumUtils.GetEnumValueInfo(visit.VisitNumber.AssigningAuthority));
            data.PatientClass = EnumUtils.GetEnumValueInfo(visit.PatientClass);
            data.PatientType = EnumUtils.GetEnumValueInfo(visit.PatientType);
            data.AdmissionType = EnumUtils.GetEnumValueInfo(visit.AdmissionType);
            data.VisitStatus = EnumUtils.GetEnumValueInfo(visit.Status, context);
            data.AdmitTime = visit.AdmitTime;
            data.DischargeTime = visit.DischargeTime;
            data.VisitFacility = facilityAssembler.CreateFacilitySummary(visit.Facility);
            data.PreadmitNumber = visit.PreadmitNumber;
        }

        private void UpdateListItem(OrderListItem data, Order order, IPersistenceContext context)
        {
            ExternalPractitionerAssembler practitionerAssembler = new ExternalPractitionerAssembler();
            DiagnosticServiceAssembler dsAssembler = new DiagnosticServiceAssembler();
            FacilityAssembler facilityAssembler = new FacilityAssembler();

            data.OrderRef = order.GetRef();
            data.PlacerNumber = order.PlacerNumber;
            data.AccessionNumber = order.AccessionNumber;
            data.DiagnosticService = dsAssembler.CreateSummary(order.DiagnosticService);
            data.EnteredTime = order.EnteredTime;
            data.SchedulingRequestTime = order.SchedulingRequestTime;
            data.OrderingPractitioner = practitionerAssembler.CreateExternalPractitionerSummary(order.OrderingPractitioner, context);
            data.OrderingFacility = facilityAssembler.CreateFacilitySummary(order.OrderingFacility);
            data.ReasonForStudy = order.ReasonForStudy;
            data.OrderPriority = EnumUtils.GetEnumValueInfo(order.Priority, context);
            data.CancelReason = order.CancelInfo != null && order.CancelInfo.Reason != null ? EnumUtils.GetEnumValueInfo(order.CancelInfo.Reason) : null;
            data.OrderStatus = EnumUtils.GetEnumValueInfo(order.Status, context);
            data.OrderScheduledStartTime = order.ScheduledStartTime;
        }

        private void UpdateListItem(OrderListItem data, Procedure rp, IPersistenceContext context)
        {
            ProcedureTypeAssembler rptAssembler = new ProcedureTypeAssembler();
            data.ProcedureRef = rp.GetRef();
			data.ProcedureType = rptAssembler.CreateSummary(rp.Type);
            data.ProcedureScheduledStartTime = rp.ScheduledStartTime;
            data.ProcedureCheckInTime = rp.ProcedureCheckIn.CheckInTime;
            data.ProcedureCheckOutTime = rp.ProcedureCheckIn.CheckOutTime;
            data.ProcedureStatus = EnumUtils.GetEnumValueInfo(rp.Status, context);
            data.ProcedurePerformingFacility = new FacilityAssembler().CreateFacilitySummary(rp.PerformingFacility);
            data.ProcedurePortable = rp.Portable;
            data.ProcedureLaterality = EnumUtils.GetEnumValueInfo(rp.Laterality, context);
        }

        private void UpdateListItem(ReportListItem data, Report report, IPersistenceContext context)
        {
        	data.ReportRef = report.GetRef();
            data.ReportStatus = EnumUtils.GetEnumValueInfo(report.Status, context);
        }
        #endregion
	}
}
