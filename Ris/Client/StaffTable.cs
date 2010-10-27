#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
    public class StaffTable : Table<StaffSummary>
    {
        public StaffTable()
        {
            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnStaffId,
              delegate(StaffSummary staff) { return staff.StaffId; },
              0.2f));
            
            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnFamilyName,
               delegate(StaffSummary staff) { return staff.Name.FamilyName; },
               0.8f));
            
            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnGivenName,
                delegate(StaffSummary staff) { return staff.Name.GivenName; },
                0.8f));

            this.Columns.Add(new TableColumn<StaffSummary, string>(SR.ColumnStaffType,
               delegate(StaffSummary staff) { return staff.StaffType.Value; },
               0.5f));
        }
    }
}
