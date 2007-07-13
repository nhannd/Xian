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
            this.Columns.Add(new TableColumn<ReportSummary, string>("Diagnostic Service",
                delegate(ReportSummary report) { return report.DiagnosticServiceName; }));
            this.Columns.Add(new TableColumn<ReportSummary, string>("Requested Procedure",
                delegate(ReportSummary report) { return report.RequestedProcedureName; }));
            this.Columns.Add(new TableColumn<ReportSummary, string>("Report Content",
                delegate(ReportSummary report) { return report.Parts[0].Content; }));
        }
    }
}
