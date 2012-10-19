#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
    public class ProtocolEditorProcedurePlanSummaryTable : Table<ProtocolEditorProcedurePlanSummaryTableItem>
    {
        public ProtocolEditorProcedurePlanSummaryTable()
        {
            ITableColumn sortColumn = new TableColumn<ProtocolEditorProcedurePlanSummaryTableItem, string>(
                "Procedure Description",
                delegate(ProtocolEditorProcedurePlanSummaryTableItem item) { return ProcedureFormat.Format(item.ProcedureDetail); },
                0.5f);

            this.Columns.Add(sortColumn);

            this.Columns.Add(new TableColumn<ProtocolEditorProcedurePlanSummaryTableItem, string>(
                                 "Protocol Status",
                                 delegate(ProtocolEditorProcedurePlanSummaryTableItem item)
                                 {
                                     return item.ProtocolDetail.Status.Value;
                                 },
                                 0.5f));

            this.Sort(new TableSortParams(sortColumn, true));
        }
    }
}