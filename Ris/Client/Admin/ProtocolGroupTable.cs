using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
    public class ProtocolGroupTable : Table<ProtocolGroupSummary>
    {
        public ProtocolGroupTable()
        {
            this.Columns.Add(new TableColumn<ProtocolGroupSummary, string>(SR.ColumnName,
                                                                           delegate(ProtocolGroupSummary summary) { return summary.Name; },
                                                                           0.5f));

            this.Columns.Add(new TableColumn<ProtocolGroupSummary, string>(SR.ColumnDescription,
                                                                           delegate(ProtocolGroupSummary summary) { return summary.Description; },
                                                                           1.0f));
        }
    }
}