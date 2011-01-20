#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Admin
{
    public class LocationTable : Table<LocationSummary>
    {
        public LocationTable()
        {
			this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnID,
				delegate(LocationSummary loc) { return loc.Id; },
				0.2f));

			this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnName,
				delegate(LocationSummary loc) { return loc.Name; },
				1.0f));

			this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnFacility,
                delegate(LocationSummary loc) { return loc.Facility.Name; },
                1.0f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnBuilding,
                delegate(LocationSummary loc) { return loc.Building; },
                0.5f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnFloor,
                delegate(LocationSummary loc) { return loc.Floor; },
                0.2f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnPointOfCare,
                delegate(LocationSummary loc) { return loc.PointOfCare; },
                0.5f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnRoom,
                delegate(LocationSummary loc) { return loc.Room; },
                0.2f));

            this.Columns.Add(new TableColumn<LocationSummary, string>(SR.ColumnBed,
                delegate(LocationSummary loc) { return loc.Bed; },
                0.2f));

        }
    }
}
