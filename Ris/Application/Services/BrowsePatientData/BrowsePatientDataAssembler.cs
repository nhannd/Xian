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

using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.BrowsePatientData;
using ClearCanvas.Ris.Application.Services.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Services.BrowsePatientData
{
    public class BrowsePatientDataAssembler
    {
        public PatientOrderData CreatePatientOrderData(Order order, IPersistenceContext context)
        {
            PatientOrderData data = new PatientOrderData();

            UpdatePatientOrderData(data, order, context);
            UpdatePatientOrderData(data, order.Visit, context);

            return data;
        }

        public PatientOrderData CreatePatientOrderData(RequestedProcedure rp, IPersistenceContext context)
        {
            PatientOrderData data = new PatientOrderData();

            UpdatePatientOrderData(data, rp.Order, context);
            UpdatePatientOrderData(data, rp.Order.Visit, context);
            UpdatePatientOrderData(data, rp, context);

            return data;
        }

        #region Private Helpers

        private void UpdatePatientOrderData(PatientOrderData data, Visit visit, IPersistenceContext context)
        {
            data.VisitNumber = visit.VisitNumber.Id;
            data.VisitNumberAssigningAuthority = visit.VisitNumber.AssigningAuthority.Code;
            data.PatientClass = EnumUtils.GetDisplayValue(visit.PatientClass);
            data.PatientType = EnumUtils.GetDisplayValue(visit.PatientType);
            data.AdmissionType = EnumUtils.GetDisplayValue(visit.AdmissionType);
            data.VisitStatus = EnumUtils.GetEnumValueInfo(visit.VisitStatus, context);
            data.AdmitTime = visit.AdmitTime;
            data.DischargeTime = visit.DischargeTime;
            data.VisitFacility = visit.Facility.Name;
            data.PreadmitNumber = visit.PreadmitNumber;
        }

        private void UpdatePatientOrderData(PatientOrderData data, Order order, IPersistenceContext context)
        {
            PersonNameAssembler nameAssembler = new PersonNameAssembler();

            data.PlacerNumber = order.PlacerNumber;
            data.AccessionNumber = order.AccessionNumber;
            data.DiagnosticServiceName = order.DiagnosticService.Name;
            data.DiagnosticServiceCode = order.DiagnosticService.Id;
            data.EnteredTime = order.EnteredTime;
            data.SchedulingRequestTime = order.SchedulingRequestTime;
            data.OrderingPractitioner = nameAssembler.CreatePersonNameDetail(order.OrderingPractitioner.Name);
            data.OrderingFacility = order.OrderingFacility.Name;
            data.ReasonForStudy = order.ReasonForStudy;
            data.OrderPriority = EnumUtils.GetValue(order.Priority, context);
            data.CancelReason = EnumUtils.GetDisplayValue(order.CancelReason);
            data.OrderStatus = EnumUtils.GetEnumValueInfo(order.Status, context);
            data.OrderScheduledStartTime = order.ScheduledStartTime;
        }

        private void UpdatePatientOrderData(PatientOrderData data, RequestedProcedure rp, IPersistenceContext context)
        {
            data.ProcedureName = rp.Type.Name;
            data.ProcedureCode = rp.Type.Id;
            data.ProcedureScheduledStartTime = rp.ScheduledStartTime;
            data.ProcedureCheckInTime = rp.ProcedureCheckIn.CheckInTime;
            data.ProcedureCheckOutTime = rp.ProcedureCheckIn.CheckOutTime;
            data.ProcedureStatus = EnumUtils.GetEnumValueInfo(rp.Status, context);
        }

        #endregion
    }
}
