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

namespace ClearCanvas.Ris.Application.Services.ReportingWorkflow
{
    public class ReportingWorklistAssembler
    {
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
