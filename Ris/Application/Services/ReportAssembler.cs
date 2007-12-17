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

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
    public class ReportAssembler
    {
        // TODO: in order for this to be efficent, the Procedures collection should have been obtained in a join fetch
        public ReportSummary CreateReportSummary(RequestedProcedure rp, Report report, IPersistenceContext context)
        {
            ReportSummary summary = new ReportSummary();

            RequestedProcedureAssembler rpAssembler = new RequestedProcedureAssembler();
            if (report != null)
            {
                summary.ReportRef = report.GetRef();
                summary.ReportStatus = EnumUtils.GetEnumValueInfo(report.Status, context);

                // use all procedures attached to report
                summary.Procedures = CollectionUtils.Map<RequestedProcedure, RequestedProcedureSummary>(report.Procedures,
                    delegate(RequestedProcedure p) { return rpAssembler.CreateRequestedProcedureSummary(p, context); });
            }
            else
            {
                // use supplied procedure
                summary.Procedures = CollectionUtils.Map<RequestedProcedure, RequestedProcedureSummary>(new RequestedProcedure[] { rp },
                    delegate(RequestedProcedure p) { return rpAssembler.CreateRequestedProcedureSummary(p, context); });
            }

            Order order = rp.Order;
            //TODO: choose the profile based on some location instead of visit assigning authority
            PatientProfile profile = order.Patient.Profiles.Count == 1 ?
                CollectionUtils.FirstElement<PatientProfile>(order.Patient.Profiles) :
                CollectionUtils.SelectFirst<PatientProfile>(order.Patient.Profiles,
                delegate(PatientProfile thisProfile)
                {
                    return thisProfile.Mrn.AssigningAuthority == order.Visit.VisitNumber.AssigningAuthority;
                });

            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            summary.Name = nameAssembler.CreatePersonNameDetail(profile.Name);
            summary.Mrn = new MrnAssembler().CreateMrnDetail(profile.Mrn);
            summary.DateOfBirth = profile.DateOfBirth;

            summary.VisitNumber = new VisitAssembler().CreateVisitNumberDetail(order.Visit.VisitNumber);
            summary.AccessionNumber = order.AccessionNumber;
            summary.DiagnosticServiceName = order.DiagnosticService.Name;

            return summary;
        }

        public ReportDetail CreateReportDetail(Report report, IPersistenceContext context)
        {
            ReportDetail detail = new ReportDetail();
            detail.ReportRef = report.GetRef();
            detail.ReportStatus = EnumUtils.GetEnumValueInfo(report.Status, context);

            RequestedProcedureAssembler rpAssembler = new RequestedProcedureAssembler();
            detail.Procedures = CollectionUtils.Map<RequestedProcedure, RequestedProcedureDetail>(report.Procedures,
                delegate(RequestedProcedure p) { return rpAssembler.CreateRequestedProcedureDetail(p, context); });

            detail.Parts = CollectionUtils.Map<ReportPart, ReportPartDetail>(report.Parts,
                delegate(ReportPart part) { return CreateReportPartDetail(part, context); });

            return detail;
        }

        public ReportPartDetail CreateReportPartDetail(ReportPart reportPart, IPersistenceContext context)
        {
            ReportPartDetail summary = new ReportPartDetail();

            summary.ReportPartRef = reportPart.GetRef();
            summary.Index = reportPart.Index;
            summary.Content = reportPart.Content;
            summary.Status = EnumUtils.GetEnumValueInfo(reportPart.Status, context);

            StaffAssembler staffAssembler = new StaffAssembler();

            if (reportPart.Supervisor != null)
                summary.Supervisor = staffAssembler.CreateStaffSummary(reportPart.Supervisor, context);

            if (reportPart.Interpreter != null)
                summary.InterpretedBy = staffAssembler.CreateStaffSummary(reportPart.Interpreter, context);

            if (reportPart.Transcriber != null)
                summary.TranscribedBy = staffAssembler.CreateStaffSummary(reportPart.Transcriber, context);

            if (reportPart.Verifier != null)
                summary.VerifiedBy = staffAssembler.CreateStaffSummary(reportPart.Verifier, context);


            return summary;
        }
    }
}
