using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Client.Adt
{
    public class RICTable : Table<RegistrationWorklistQueryResult>
    {
        public RICTable()
        {
            this.Columns.Add(new TableColumn<RegistrationWorklistQueryResult, string>("Requested Procedure",
                delegate(RegistrationWorklistQueryResult item) { return Format.Custom(item.RequestedProcedureName); }));
            this.Columns.Add(new TableColumn<RegistrationWorklistQueryResult, string>("Ordering Physician",
                delegate(RegistrationWorklistQueryResult item) { return Format.Custom(item.OrderingPractitioner); }));
            this.Columns.Add(new TableColumn<RegistrationWorklistQueryResult, string>("Insurance",
                delegate(RegistrationWorklistQueryResult item) { return "N/A"; }));
            this.Columns.Add(new TableColumn<RegistrationWorklistQueryResult, string>("Scheduled For",
                delegate(RegistrationWorklistQueryResult item) 
                {
                    if (item.ProcedureStepScheduledStartTime == null)
                        return "Not Scheduled";
                    else
                        return Format.Custom(item.ProcedureStepScheduledStartTime); 
                }));
            this.Columns.Add(new TableColumn<RegistrationWorklistQueryResult, string>("Facility",
                delegate(RegistrationWorklistQueryResult item) { return "N/A"; }));
        }
   }
}
