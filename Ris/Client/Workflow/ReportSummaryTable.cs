#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Reporting
{
    public class ReportSummaryTable : Table<ReportSummary>
    {
        public ReportSummaryTable()
        {
            this.Columns.Add(new TableColumn<ReportSummary, string>("Accession Number",
                delegate(ReportSummary report) { return report.AccessionNumber; }));
            this.Columns.Add(new TableColumn<ReportSummary, string>("Visit Number",
                delegate(ReportSummary report) { return VisitNumberFormat.Format(report.VisitNumber); }));
            this.Columns.Add(new TableColumn<ReportSummary, string>("Requested Procedure",
                delegate(ReportSummary report)
                {
                    return StringUtilities.Combine(report.Procedures, ", ",
                        delegate(RequestedProcedureSummary summary) { return summary.Type.Name; });
                }));
            this.Columns.Add(new TableColumn<ReportSummary, string>("Performed Location",
                delegate(ReportSummary report) { return "WHAT?"; }));
            this.Columns.Add(new TableColumn<ReportSummary, string>("Performed Date",
                delegate(ReportSummary report) { return "WHAT?"; }));
            this.Columns.Add(new TableColumn<ReportSummary, string>("Status",
                delegate(ReportSummary report) { return report.ReportStatus.Value; }));
        }
    }
}
