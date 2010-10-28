#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
    public class PriorSummaryTable : Table<PriorProcedureSummary>
    {
        public PriorSummaryTable()
        {
            this.Columns.Add(new TableColumn<PriorProcedureSummary, string>("Accession Number",
                delegate(PriorProcedureSummary item) { return AccessionFormat.Format(item.AccessionNumber); }));
            this.Columns.Add(new TableColumn<PriorProcedureSummary, string>("Procedure",
                delegate(PriorProcedureSummary item) { return ProcedureFormat.Format(item); }));

            this.Columns.Add(new DateTableColumn<PriorProcedureSummary>("Performed Date",
                delegate(PriorProcedureSummary item) { return item.PerformedDate.Value; }));
            this.Columns.Add(new TableColumn<PriorProcedureSummary, string>("Report Status",
                delegate(PriorProcedureSummary item) { return item.ReportStatus.Value; }));
        }
    }
}
