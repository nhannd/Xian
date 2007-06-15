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
            
            ReportingProcedureStep step = (ReportingProcedureStep)context.Load(item.ProcedureStepRef);
            PatientProfile profile = CollectionUtils.SelectFirst<PatientProfile>(step.RequestedProcedure.Order.Patient.Profiles,
                delegate(PatientProfile thisProfile)
                {
                    if (thisProfile.Mrn.AssigningAuthority == step.RequestedProcedure.Order.Visit.VisitNumber.AssigningAuthority)
                        return true;

                    return false;
                });

            ReportingWorklistPreview preview = new ReportingWorklistPreview();

            preview.ReportContent = (step.Report == null ? null : step.Report.ReportContent);

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

        //public ReportingWorklistItem CreateReportingWorklistItem(ReportingWorklistQueryResult result)
        //{
        //    ReportingWorklistItem item = new ReportingWorklistItem();

        //    //TODO: Detail of ReportingWorklistItem not defined
        //    item.ProcedureStepRef = result.ProcedureStep;
        //    item.Mrn = new MrnDetail(result.Mrn.Id, result.Mrn.AssigningAuthority);

        //    PersonNameAssembler assembler = new PersonNameAssembler();
        //    item.PersonNameDetail = assembler.CreatePersonNameDetail(result.PatientName);

        //    item.AccessionNumber = result.AccessionNumber;
        //    item.RequestedProcedureName = result.RequestedProcedureName;
        //    item.DiagnosticServiceName = result.DiagnosticServiceName;
        //    item.Priority = result.Priority.ToString();
        //    item.ActivityStatusCode = result.Status.ToString();

        //    return item;
        //}

        //public ReportingProcedureStepSearchCriteria CreateSearchCriteria(ReportingWorklistSearchCriteria criteria)
        //{
        //    ReportingProcedureStepSearchCriteria rpsSearchCriteria = new ReportingProcedureStepSearchCriteria();

        //    ActivityStatus status = (ActivityStatus)Enum.Parse(typeof(ActivityStatus), criteria.ActivityStatusCode);
        //    rpsSearchCriteria.State.EqualTo(status);

        //    return rpsSearchCriteria;
        //}
    }
}
