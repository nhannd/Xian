using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Reporting
{
    public class ProtocolEditorProcedurePlanSummaryTable : Table<ProtocolEditorProcedurePlanSummaryTableItem>
    {
        public ProtocolEditorProcedurePlanSummaryTable()
        {
            ITableColumn sortColumn = new TableColumn<ProtocolEditorProcedurePlanSummaryTableItem, string>(
                "Procedure Description",
                delegate(ProtocolEditorProcedurePlanSummaryTableItem item) { return item.RequestedProcedureDetail.Name; },
                0.5f);

            this.Columns.Add(sortColumn);

            this.Columns.Add(new TableColumn<ProtocolEditorProcedurePlanSummaryTableItem, string>(
                                 "Protocol Status",
                                 delegate(ProtocolEditorProcedurePlanSummaryTableItem item)
                                 {
                                     switch(item.ProtocolStepDetail.Status.Code)
                                     {
                                         case "SC":
                                         case "IP":
                                             return "Pending";
                                         case "SU":
                                             return "Suspended/Rejected";
                                         case "DC":
                                             return "Rejected/Cancelled";
                                         case "CM":
                                             return "Accepted";
                                     }
                                     return item.ProtocolStepDetail.Status.Value;
                                 },
                                 0.5f));

            this.Sort(new TableSortParams(sortColumn, true));
        }
    }
}