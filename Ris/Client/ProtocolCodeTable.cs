using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    public class ProtocolCodeTable : Table<ProtocolCodeDetail>
    {
        public ProtocolCodeTable()
        {
            this.Columns.Add(new TableColumn<ProtocolCodeDetail, string>("Code",
                                                                         delegate(ProtocolCodeDetail detail)
                                                                             { return detail.Name; },
                                                                         0.5f));
        }
    }
}