using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Workflow;

namespace ClearCanvas.Ris.Application.Services.ReportingWorkflow
{
    public class ReportingWorklistAssembler
    {
        public ReportingWorklistItem CreateWorklistItem(ReportingWorklistQueryResult result)
        {
            ReportingWorklistItem item = new ReportingWorklistItem();

            //TODO: Detail of ReportingWorklistItem not defined
            item.ProcedureStepRef = result.ProcedureStep;
            item.MRNAssigningAuthority = result.Mrn.AssigningAuthority;
            item.MRNID = result.Mrn.Id;
            PersonNameAssembler assembler = new PersonNameAssembler();
            item.PersonNameDetail = assembler.CreatePersonNameDetail(result.PatientName);
            item.AccessionNumber = result.AccessionNumber;
            item.RequestedProcedureName = result.RequestedProcedureName;
            item.DiagnosticServiceName = result.DiagnosticServiceName;
            item.Priority = result.Priority;

            //TODO: Check Enum conversion
            item.ActivityStatus = result.Status.ToString();

            return item;
        }

        public ReportingProcedureStepSearchCriteria CreateSearchCriteria(ReportingWorklistSearchCriteria criteria)
        {
            ReportingProcedureStepSearchCriteria rpsSearchCriteria = new ReportingProcedureStepSearchCriteria();

            // TODO Check Enum conversion
            ActivityStatus status = (ActivityStatus)Enum.Parse(typeof(ActivityStatus), criteria.ActivityStatus);
            rpsSearchCriteria.State.EqualTo(status);

            return rpsSearchCriteria;
        }
    }
}
