#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;

namespace ClearCanvas.Enterprise.Desktop
{
    public class UserTable : Table<UserSummary>
    {
        public UserTable()
        {
            this.Columns.Add(new TableColumn<UserSummary, string>(SR.ColumnUserId,
                delegate(UserSummary user) { return user.UserName; },
                0.5f));

            this.Columns.Add(new TableColumn<UserSummary, string>(SR.ColumnUserName,
                delegate(UserSummary user) { return user.DisplayName; },
                1.0f));

            this.Columns.Add(new DateTimeTableColumn<UserSummary>("Created On",
                delegate(UserSummary user) { return user.CreationTime; },
                0.75f));

            this.Columns.Add(new TableColumn<UserSummary, bool>("Enabled",
               delegate(UserSummary user) { return user.Enabled; },
               0.25f));

			this.Columns.Add(new DateTimeTableColumn<UserSummary>("Valid From",
                delegate(UserSummary user) { return user.ValidFrom; },
                0.75f));

			this.Columns.Add(new DateTimeTableColumn<UserSummary>("Valid Until",
               delegate(UserSummary user) { return user.ValidUntil; },
               0.75f));

			this.Columns.Add(new DateTimeTableColumn<UserSummary>("Last Login Time",
               delegate(UserSummary user) { return user.LastLoginTime; },
               0.75f));
        }
    }
}
