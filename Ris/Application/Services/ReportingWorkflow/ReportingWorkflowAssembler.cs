using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Workflow;
using ClearCanvas.Workflow.Brokers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Services.Admin;

namespace ClearCanvas.Ris.Application.Services.ReportingWorkflow
{
    public class ReportingWorkflowAssembler
    {
        public ReportingWorklistPreview CreateReportingWorklistPreview(ReportingWorklistItem item, IPersistenceContext context)
        {
            SexEnumTable sexEnumTable = context.GetBroker<ISexEnumBroker>().Load();

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
            preview.Sex = sexEnumTable[profile.Sex].Value;

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
            item.Mrn = new MrnDetail(domainItem.Mrn.Id, domainItem.Mrn.AssigningAuthority);

            PersonNameAssembler assembler = new PersonNameAssembler();
            item.PersonNameDetail = assembler.CreatePersonNameDetail(domainItem.PatientName);

            item.AccessionNumber = domainItem.AccessionNumber;
            item.RequestedProcedureName = domainItem.RequestedProcedureName;
            item.DiagnosticServiceName = domainItem.DiagnosticServiceName;

            OrderPriorityEnumTable orderPriorityTable = context.GetBroker<IOrderPriorityEnumBroker>().Load();
            item.Priority = orderPriorityTable[domainItem.Priority].Value;

            ActivityStatusEnumTable activityStatusTable = context.GetBroker<IActivityStatusEnumBroker>().Load();
            item.ActivityStatus = new EnumValueInfo(domainItem.ActivityStatus.ToString(), activityStatusTable[domainItem.ActivityStatus].Value);

            item.StepType = domainItem.StepType;

            return item;
        }

        public ReportSummary CreateReportSummary(RequestedProcedure rp, Report report)
        {
            ReportSummary summary = new ReportSummary();

            if (report != null)
            {
                summary.ReportRef = report.GetRef();
                summary.Parts = CollectionUtils.Map<ReportPart, ReportPartSummary, List<ReportPartSummary>>(report.Parts,
                    delegate(ReportPart part)
                    {
                        return CreateReportPartSummary(part);
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

        public ReportPartSummary CreateReportPartSummary(ReportPart reportPart)
        {
            ReportPartSummary summary = new ReportPartSummary();

            summary.ReportPartRef = reportPart.GetRef();
            summary.Index = reportPart.Index;
            summary.Content = reportPart.Content;

            return summary;
        }
    }
}
