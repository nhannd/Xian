using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Client.Admin
{
    class LocationTable : Table<LocationSummary>
    {
        public LocationTable()
        {
            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnFacility,
                delegate(LocationSummary loc) { return loc.FacilityName; },
                0.5f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnBuilding,
                delegate(LocationSummary loc) { return loc.Building; },
                1.0f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnFloor,
                delegate(LocationSummary loc) { return loc.Floor; },
                0.2f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnPointOfCare,
                delegate(LocationSummary loc) { return loc.PointOfCare; },
                1.0f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnRoom,
                delegate(LocationSummary loc) { return loc.Room; },
                0.2f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnBed,
                delegate(LocationSummary loc) { return loc.Bed; },
                0.2f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnActive,
                delegate(LocationSummary loc) { return loc.Active.ToString(); },
                0.2f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnInactiveDate,
                delegate(LocationSummary loc) { return Format.Date(loc.InactiveDate); },
                0.5f));
        }
    }
}
