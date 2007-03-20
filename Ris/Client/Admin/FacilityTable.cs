using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Client.Admin
{
    class FacilityTable : Table<FacilitySummary>
    {
        public FacilityTable()
        {
            this.Columns.Add(new TableColumn<FacilitySummary, string>(SR.ColumnCode,
                delegate(FacilitySummary f) { return f.Code; },
                0.5f));

            this.Columns.Add(new TableColumn<FacilitySummary, string>(SR.ColumnName,
                delegate(FacilitySummary f) { return f.Name; },
                1.0f));
        }
    }
}
