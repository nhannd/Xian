using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Reporting
{
    public class ProtocolEditorProcedurePlanSummaryTable : Table<ProtocolEditorProcedurePlanSummaryTableItem>
    {
        public ProtocolEditorProcedurePlanSummaryTable()
        {
            ITableColumn sortColumn = new TableColumn<ProtocolEditorProcedurePlanSummaryTableItem, string>(
                "Procedure Description",
                delegate(ProtocolEditorProcedurePlanSummaryTableItem item) { return item.RequestedProcedureDetail.Type.Name; },
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