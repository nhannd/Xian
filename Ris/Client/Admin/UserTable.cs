using System;

using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.Admin.AuthenticationAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    public class UserTable : Table<UserSummary>
    {
        public UserTable()
        {
            this.Columns.Add(new TableColumn<UserSummary, string>(SR.ColumnUserId,
                delegate(UserSummary user) { return user.UserId; },
                1.0f));

            this.Columns.Add(new TableColumn<UserSummary, string>(SR.ColumnUserName,
                delegate(UserSummary user) { return string.Format("{0}, {1}", user.UserName.FamilyName, user.UserName.GivenName); },
                1.0f));
        }
    }
}
