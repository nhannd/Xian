using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Reporting
{
    public class ReportSummaryTable : Table<ReportSummary>
    {
        public ReportSummaryTable()
        {
            this.Columns.Add(new TableColumn<ReportSummary, string>("Accession Number",
                delegate(ReportSummary report) { return report.AccessionNumber; }));
            this.Columns.Add(new TableColumn<ReportSummary, string>("Visit Number",
                delegate(ReportSummary report) { return String.Format("{0} {1}", report.VisitNumberAssigningAuthority, report.VisitNumberId); }));
            this.Columns.Add(new TableColumn<ReportSummary, string>("Requested Procedure",
                delegate(ReportSummary report) { return report.RequestedProcedureName; }));
            this.Columns.Add(new TableColumn<ReportSummary, string>("Performed Location",
                delegate(ReportSummary report) { return report.PerformedLocation; }));
            this.Columns.Add(new TableColumn<ReportSummary, DateTime?>("Performed Date",
                delegate(ReportSummary report) { return report.PerformedDate; }));
        }
    }
}
