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

namespace ClearCanvas.Ris.Client.Admin
{
	public class FacilityTable : Table<FacilitySummary>
	{
		public FacilityTable()
		{
			this.Columns.Add(new TableColumn<FacilitySummary, string>(SR.ColumnCode, f => f.Code, 0.5f));
			this.Columns.Add(new TableColumn<FacilitySummary, string>(SR.ColumnName, f => f.Name, 1.0f));
			this.Columns.Add(new TableColumn<FacilitySummary, string>(SR.ColumnDescription, f => f.Description, 1.0f));
			this.Columns.Add(new TableColumn<FacilitySummary, string>(SR.ColumnInformationAuthority, f => f.InformationAuthority.Value, 1.0f));
		}
	}
}
