using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Reporting
{
    public class ReportingWorklistTable : Table<ReportingWorklistItem>
    {
        public ReportingWorklistTable()
        {
            this.Columns.Add(new TableColumn<ReportingWorklistItem, string>("MRN",
                delegate(ReportingWorklistItem item) { return MrnFormat.Format(item.Mrn); }));
            this.Columns.Add(new TableColumn<ReportingWorklistItem, string>("Name",
                delegate(ReportingWorklistItem item) { return PersonNameFormat.Format(item.PersonNameDetail); }));
            this.Columns.Add(new TableColumn<ReportingWorklistItem, string>("Accession #",
                delegate(ReportingWorklistItem item) { return item.AccessionNumber; }));
            this.Columns.Add(new TableColumn<ReportingWorklistItem, string>("Service",
                delegate(ReportingWorklistItem item) { return item.DiagnosticServiceName; }));
            this.Columns.Add(new TableColumn<ReportingWorklistItem, string>("Procedure",
                delegate(ReportingWorklistItem item) { return item.RequestedProcedureName; }));
            this.Columns.Add(new TableColumn<ReportingWorklistItem, string>("Priority",
                delegate(ReportingWorklistItem item) { return item.Priority; }));
        }
    }
}
