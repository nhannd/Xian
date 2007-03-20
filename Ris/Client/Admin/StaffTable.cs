using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Client.Admin
{
    class StaffTable : Table<StaffSummary>
    {
        public StaffTable()
        {
            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnLicenseNumber,
                delegate(StaffSummary staff) { return staff.LicenseNumber; },
                1.0f));

            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnPrefix,
                delegate(StaffSummary staff) { return staff.PersonNameDetail.Prefix; },
                0.2f));

            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnGivenName,
                delegate(StaffSummary staff) { return staff.PersonNameDetail.GivenName; },
                1.0f));

            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnMiddleName,
                delegate(StaffSummary staff) { return staff.PersonNameDetail.MiddleName; },
                1.0f));

            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnFamilyName,
                delegate(StaffSummary staff) { return staff.PersonNameDetail.FamilyName; },
                1.0f));

            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnSuffix,
                delegate(StaffSummary staff) { return staff.PersonNameDetail.Suffix; },
                0.2f));

            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnDegree,
                delegate(StaffSummary staff) { return staff.PersonNameDetail.Degree; },
                1.0f));
        }
    }
}
