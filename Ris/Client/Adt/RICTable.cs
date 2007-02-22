using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Registration;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    public class RICTable : Table<WorklistQueryResult>
    {
        public RICTable()
        {
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("Requested Procedure",
                delegate(WorklistQueryResult item) { return Format.Custom(item.RequestedProcedureName); }));
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("Ordering Physician",
                delegate(WorklistQueryResult item) { return Format.Custom(item.OrderingPractitioner); }));
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("Insurance",
                delegate(WorklistQueryResult item) { return "N/A"; }));
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("Scheduled For",
                delegate(WorklistQueryResult item) 
                {
                    if (item.ProcedureStepScheduledStartTime == null)
                        return "Not Scheduled";
                    else
                        return Format.Custom(item.ProcedureStepScheduledStartTime); 
                }));
            this.Columns.Add(new TableColumn<WorklistQueryResult, string>("Facility",
                delegate(WorklistQueryResult item) { return "N/A"; }));
        }
   }
}
