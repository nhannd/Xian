#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
            Columns.Add(new TableColumn<UserSummary, string>(SR.ColumnUserId,
                                                             user => user.UserName,
                                                             0.5f));

            Columns.Add(new TableColumn<UserSummary, string>(SR.ColumnUserName,
                                                             user => user.DisplayName,
                                                             1.0f));

            Columns.Add(new TableColumn<UserSummary, string>(SR.ColumnEmailAddress,
                                                             user => user.EmailAddress,
                                                             1.0f));

            Columns.Add(new DateTimeTableColumn<UserSummary>(SR.ColumnCreatedOn,
                                                             user => user.CreationTime,
                                                             0.75f));

            Columns.Add(new TableColumn<UserSummary, bool>(SR.ColumnEnabled,
                                                           user => user.Enabled,
                                                           0.25f));

            Columns.Add(new DateTimeTableColumn<UserSummary>(SR.ColumnValidFrom,
                                                             user => user.ValidFrom,
                                                             0.75f));

            Columns.Add(new DateTimeTableColumn<UserSummary>(SR.ColumnValidUntil,
                                                             user => user.ValidUntil,
                                                             0.75f));

            Columns.Add(new DateTimeTableColumn<UserSummary>(SR.ColumnPasswordExpiry,
                                                             user => user.PasswordExpiry,
                                                             0.75f));

            Columns.Add(new TableColumn<UserSummary,int>(SR.ColumnSessionCount,
                                                             user => user.SessionCount,
                                                             0.3f));

            Columns.Add(new DateTimeTableColumn<UserSummary>(SR.ColumnLastLoginTime,
                                                             user => user.LastLoginTime,
                                                             0.75f));
        }
    }
}
