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
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Services.ReportingWorkflow
{
    public class ReportingWorkflowAssembler
    {
        public ReportingWorklistItem CreateReportingWorklistItem(WorklistItem domainItem, IPersistenceContext context)
        {
            PersonNameAssembler assembler = new PersonNameAssembler();
            return new ReportingWorklistItem(
                domainItem.ProcedureStepRef,
                domainItem.RequestedProcedureRef,
                domainItem.OrderRef,
                domainItem.PatientRef,
                domainItem.PatientProfileRef,
                new MrnAssembler().CreateMrnDetail(domainItem.Mrn),
                assembler.CreatePersonNameDetail(domainItem.PatientName),
                domainItem.AccessionNumber,
                EnumUtils.GetEnumValueInfo(domainItem.OrderPriority, context),
                EnumUtils.GetEnumValueInfo(domainItem.PatientClass),
                domainItem.DiagnosticServiceName,
                domainItem.RequestedProcedureName,
                domainItem.ProcedureStepName,
                domainItem.ScheduledStartTime,
                EnumUtils.GetEnumValueInfo(domainItem.ActivityStatus, context)
                );
        }

        public ReportSummary CreateReportSummary(RequestedProcedure rp, Report report, IPersistenceContext context)
        {
            ReportSummary summary = new ReportSummary();

            if (report != null)
            {
                summary.ReportRef = report.GetRef();
                summary.ReportStatus = EnumUtils.GetEnumValueInfo(report.Status, context);
                summary.Parts = CollectionUtils.Map<ReportPart, ReportPartSummary, List<ReportPartSummary>>(report.Parts,
                    delegate(ReportPart part)
                    {
                        return CreateReportPartSummary(part, context);
                    });
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
            summary.RequestedProcedureName = rp.Type.Name;
            summary.PerformedLocation = "Not implemented";
            //summary.PerformedDate = ;

            return summary;
        }

        public ReportPartSummary CreateReportPartSummary(ReportPart reportPart, IPersistenceContext context)
        {
            ReportPartSummary summary = new ReportPartSummary();

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
