using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Client
{
    class StaffTable : Table<StaffSummary>
    {
        public StaffTable()
        {
            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnStaffId,
              delegate(StaffSummary staff) { return staff.StaffId; },
              1.0f));
            
            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnFamilyName,
               delegate(StaffSummary staff) { return staff.Name.FamilyName; },
               1.0f));
            
            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnGivenName,
                delegate(StaffSummary staff) { return staff.Name.GivenName; },
                1.0f));

            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnStaffType,
               delegate(StaffSummary staff) { return staff.StaffType.Value; },
               1.0f));
        }
    }
}
