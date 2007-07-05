using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    public class RequestedProcedureTypeGroupSummaryTable : Table<RequestedProcedureTypeGroupSummary>
    {
        public RequestedProcedureTypeGroupSummaryTable()
        {
            this.Columns.Add(new TableColumn<RequestedProcedureTypeGroupSummary, string>(SR.ColumnName,
                delegate (RequestedProcedureTypeGroupSummary summary) { return summary.Name; },
                0.5f));

            this.Columns.Add(new TableColumn<RequestedProcedureTypeGroupSummary, string>(SR.ColumnDescription,
                delegate (RequestedProcedureTypeGroupSummary summary) { return summary.Description; },
                0.5f));

            this.Columns.Add(new TableColumn<RequestedProcedureTypeGroupSummary, string>(SR.ColumnCategory,
                delegate (RequestedProcedureTypeGroupSummary summary) { return summary.Category.Value; },
                0.5f));
        }
    }
}