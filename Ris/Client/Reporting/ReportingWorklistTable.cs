using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Reporting
{
    public class ReportingWorklistTable : Table<ReportingWorklistQueryResult>
    {
        public ReportingWorklistTable(OrderPriorityEnumTable orderPriorities)
        {
            this.Columns.Add(new TableColumn<ReportingWorklistQueryResult, string>("MRN",
                delegate(ReportingWorklistQueryResult item) { return Format.Custom(item.Mrn); }));
            this.Columns.Add(new TableColumn<ReportingWorklistQueryResult, string>("Name",
                delegate(ReportingWorklistQueryResult item) { return Format.Custom(item.PatientName); }));
            this.Columns.Add(new TableColumn<ReportingWorklistQueryResult, string>("Accession #",
                delegate(ReportingWorklistQueryResult item) { return item.AccessionNumber; }));
            this.Columns.Add(new TableColumn<ReportingWorklistQueryResult, string>("Service",
                delegate(ReportingWorklistQueryResult item) { return item.DiagnosticServiceName; }));
            this.Columns.Add(new TableColumn<ReportingWorklistQueryResult, string>("Procedure",
                delegate(ReportingWorklistQueryResult item) { return item.RequestedProcedureName; }));
            this.Columns.Add(new TableColumn<ReportingWorklistQueryResult, string>("Priority",
                delegate(ReportingWorklistQueryResult item) { return orderPriorities[item.Priority].Value; }));
        }
    }
}
