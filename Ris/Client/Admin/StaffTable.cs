using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    class StaffTable : Table<Staff>
    {
        public StaffTable()
        {
            this.Columns.Add(new TableColumn<Staff, string>(SR.ColumnPrefix,
                delegate(Staff staff) { return staff.Name.Prefix; },
                0.2f));

            this.Columns.Add(new TableColumn<Staff, string>(SR.ColumnGivenName,
                delegate(Staff staff) { return staff.Name.GivenName; },
                1.0f));

            this.Columns.Add(new TableColumn<Staff, string>(SR.ColumnMiddleName,
                delegate(Staff staff) { return staff.Name.MiddleName; },
                1.0f));

            this.Columns.Add(new TableColumn<Staff, string>(SR.ColumnFamilyName,
                delegate(Staff staff) { return staff.Name.FamilyName; },
                1.0f));

            this.Columns.Add(new TableColumn<Staff, string>(SR.ColumnSuffix,
                delegate(Staff staff) { return staff.Name.Suffix; },
                0.2f));

            this.Columns.Add(new TableColumn<Staff, string>(SR.ColumnDegree,
                delegate(Staff staff) { return staff.Name.Degree; },
                1.0f));
        }
    }
}
