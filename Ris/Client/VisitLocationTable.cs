#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class VisitLocationTable : Table<VisitLocationDetail>
	{
		public VisitLocationTable()
		{
			this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
				SR.ColumnRole, vl => vl.Role.Value, 0.8f));
			this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
				SR.ColumnLocation, FormatVisitLocation, 2.5f));
			this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
				SR.ColumnRoom, vl => vl.Room, 0.2f));
			this.Columns.Add(new TableColumn<VisitLocationDetail, string>(
				SR.ColumnBed, vl => vl.Bed, 0.2f));
			this.Columns.Add(new DateTimeTableColumn<VisitLocationDetail>(
				SR.ColumnStartTime, vl => vl.StartTime, 0.8f));
			this.Columns.Add(new DateTimeTableColumn<VisitLocationDetail>(
				SR.ColumnEndTime, vl => vl.EndTime, 0.8f));
		}

		private static string FormatVisitLocation(VisitLocationDetail vl)
		{
			return string.Format("{0}, {1}, {2}, {3}, {4}", vl.Bed, vl.Room, vl.Location.Floor, vl.Location.Building, vl.Location.Facility.Name);
		}
	}
}
