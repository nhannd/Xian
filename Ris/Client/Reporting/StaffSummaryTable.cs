using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Reporting
{
    public class StaffSummaryTable : Table<StaffSummary>
    {
        public StaffSummaryTable()
        {
            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnID,
                delegate(StaffSummary staff) { return staff.StaffId; }));

            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnName,
                delegate(StaffSummary staff) { return PersonNameFormat.Format(staff.Name); }));
        }
    }
}
