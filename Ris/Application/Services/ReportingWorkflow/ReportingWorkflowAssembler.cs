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
using ClearCanvas.Ris.Application.Services.Admin;

namespace ClearCanvas.Ris.Application.Services.ReportingWorkflow
{
    public class ReportingWorkflowAssembler
    {
        public ReportingWorklistPreview CreateReportingWorklistPreview(ReportingWorklistItem item, IPersistenceContext context)
        {
            ReportingProcedureStep step = context.Load<ReportingProcedureStep>(item.ProcedureStepRef);
            PatientProfile profile = CollectionUtils.SelectFirst<PatientProfile>(step.RequestedProcedure.Order.Patient.Profiles,
                delegate(PatientProfile thisProfile)
                {
                    if (thisProfile.Mrn.AssigningAuthority == step.RequestedProcedure.Order.Visit.VisitNumber.AssigningAuthority)
                        return true;

                    return false;
                });

            ReportingWorklistPreview preview = new ReportingWorklistPreview();

            preview.ReportContent = (step.ReportPart == null ? null : step.ReportPart.Content);

            // PatientProfile Details
            preview.Mrn = new MrnDetail(profile.Mrn.Id, profile.Mrn.AssigningAuthority);
            preview.Name = new PersonNameAssembler().CreatePersonNameDetail(profile.Name);
            preview.DateOfBirth = profile.DateOfBirth;
            preview.Sex = EnumUtils.GetValue(profile.Sex, context);

            // Order Details
            preview.AccessionNumber = step.RequestedProcedure.Order.AccessionNumber;
            preview.RequestedProcedureName = step.RequestedProcedure.Type.Name;

            // Visit Details
            preview.VisitNumberId = step.RequestedProcedure.Order.Visit.VisitNumber.Id;
            preview.VisitNumberAssigningAuthority = step.RequestedProcedure.Order.Visit.VisitNumber.AssigningAuthority;
            
            return preview;
        }

        public ReportingWorklistItem CreateReportingWorklistItem(WorklistItem domainItem, IPersistenceContext context)
        {
            ReportingWorklistItem item = new ReportingWorklistItem();

            item.ProcedureStepRef = domainItem.ProcedureStepRef;

            item.PatientProfileRef = domainItem.PatientProfileRef;
            item.Mrn = new MrnDetail(domainItem.Mrn.Id, domainItem.Mrn.AssigningAuthority);

            PersonNameAssembler assembler = new PersonNameAssembler();
            item.PersonNameDetail = assembler.CreatePersonNameDetail(domainItem.PatientName);

            item.AccessionNumber = domainItem.AccessionNumber;
            item.RequestedProcedureName = domainItem.RequestedProcedureName;
            item.DiagnosticServiceName = domainItem.DiagnosticServiceName;
            item.Priority = EnumUtils.GetValue(domainItem.Priority, context);
            item.ActivityStatus = EnumUtils.GetEnumValueInfo(domainItem.ActivityStatus, context);
            item.StepType = domainItem.StepType;

            return item;
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
            PatientProfile profile = CollectionUtils.SelectFirst<PatientProfile>(order.Patient.Profiles,
                delegate(PatientProfile thisProfile)
                {
                    return thisProfile.Mrn.AssigningAuthority == order.Visit.VisitNumber.AssigningAuthority;
                });

            PersonNameAssembler nameAssembler = new PersonNameAssembler();
            summary.Name = nameAssembler.CreatePersonNameDetail(profile.Name);
            summary.Mrn = new MrnDetail(profile.Mrn.Id, profile.Mrn.AssigningAuthority);
            summary.DateOfBirth = profile.DateOfBirth;
            summary.VisitNumberId = order.Visit.VisitNumber.Id;
            summary.VisitNumberAssigningAuthority = order.Visit.VisitNumber.AssigningAuthority;
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

            if (reportPart.Interpretor != null)
                summary.InterpretedBy = staffAssembler.CreateStaffSummary(reportPart.Interpretor, context);

            if (reportPart.Transcriber != null)
                summary.TranscribedBy = staffAssembler.CreateStaffSummary(reportPart.Transcriber, context);

            if (reportPart.Verifier != null)
                summary.VerifiedBy = staffAssembler.CreateStaffSummary(reportPart.Verifier, context);
            
            
            return summary;
        }
    }
}
