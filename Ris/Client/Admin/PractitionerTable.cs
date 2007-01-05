using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    class PractitionerTable : Table<Practitioner>
    {
        public PractitionerTable()
        {
            this.Columns.Add(new TableColumn<Practitioner, string>(SR.ColumnLicenseNumber,
                delegate(Practitioner practitioner) { return practitioner.LicenseNumber; },
                1.0f));

            this.Columns.Add(new TableColumn<Practitioner, string>(SR.ColumnPrefix,
                delegate(Practitioner practitioner) { return practitioner.Name.Prefix; },
                0.2f));

            this.Columns.Add(new TableColumn<Practitioner, string>(SR.ColumnGivenName,
                delegate(Practitioner practitioner) { return practitioner.Name.GivenName; },
                1.0f));

            this.Columns.Add(new TableColumn<Practitioner, string>(SR.ColumnMiddleName,
                delegate(Practitioner practitioner) { return practitioner.Name.MiddleName; },
                1.0f));

            this.Columns.Add(new TableColumn<Practitioner, string>(SR.ColumnFamilyName,
                delegate(Practitioner practitioner) { return practitioner.Name.FamilyName; },
                1.0f));

            this.Columns.Add(new TableColumn<Practitioner, string>(SR.ColumnSuffix,
                delegate(Practitioner practitioner) { return practitioner.Name.Suffix; },
                0.2f));

            this.Columns.Add(new TableColumn<Practitioner, string>(SR.ColumnDegree,
                delegate(Practitioner practitioner) { return practitioner.Name.Degree; },
                1.0f));
        }
    }
}
