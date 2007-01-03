using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    class LocationTable : Table<Location>
    {
        public LocationTable()
        {
            this.Columns.Add(new TableColumn<Location, string>(SR.ColumnFacility,
                delegate(Location loc) { return (loc.Facility == null ? "" : loc.Facility.Name); },
                0.5f));

            this.Columns.Add(new TableColumn<Location, string>(SR.ColumnBuilding,
                delegate(Location loc) { return loc.Building; },
                1.0f));

            this.Columns.Add(new TableColumn<Location, string>(SR.ColumnFloor,
                delegate(Location loc) { return loc.Floor; },
                0.2f));

            this.Columns.Add(new TableColumn<Location, string>(SR.ColumnPointOfCare,
                delegate(Location loc) { return loc.PointOfCare; },
                1.0f));

            this.Columns.Add(new TableColumn<Location, string>(SR.ColumnRoom,
                delegate(Location loc) { return loc.Room; },
                0.2f));

            this.Columns.Add(new TableColumn<Location, string>(SR.ColumnBed,
                delegate(Location loc) { return loc.Bed; },
                0.2f));

            this.Columns.Add(new TableColumn<Location, string>(SR.ColumnActive,
                delegate(Location loc) { return loc.Active.ToString(); },
                0.2f));

            this.Columns.Add(new TableColumn<Location, string>(SR.ColumnInactiveDate,
                delegate(Location loc) { return Format.Date(loc.InactiveDate); },
                0.5f));
        }
    }
}
