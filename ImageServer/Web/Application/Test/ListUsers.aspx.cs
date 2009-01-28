using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Security.Principal;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Web.Common.Security;
using IUserAdminService=ClearCanvas.ImageServer.Common.Services.Admin.IUserAdminService;

namespace ClearCanvas.ImageServer.Web.Application.Test
{
    public partial class ListUsers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
        }

        public override void DataBind()
        {
            LoginCredentials credential = SessionManager.Current.Credentials;
            CurrentUser.Text = String.Format("{0}: {1} expire at: {2}",
                Thread.CurrentPrincipal,
                    credential.DisplayName,
                    credential.SessionToken.ExpiryTime
                );


            Platform.GetService<IUserAdminService>(
                delegate(IUserAdminService services)
                    {
                        List<UserSummary> users = services.ListUsers(new ListUsersRequest());

                        List<UserRowData> rows = CollectionUtils.Map<UserSummary, UserRowData>(
                            users, delegate(UserSummary summary)
                                       {
                                           UserRowData row = new UserRowData(summary);
                                           return row;
                                       });

                        UserGridView.DataSource = rows;
                    });
            base.DataBind();
        }

        protected void LogoutClicked(object sender, EventArgs e)
        {
            SessionManager.TerminiateSession();
        }
    }

    class UserRowData
    {
        private string _userName;
        private string _displayName;
        private bool _enabled;
        private DateTime? _lastLoginTime;
        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public DateTime? LastLoginTime
        {
            get { return _lastLoginTime; }
            set { _lastLoginTime = value; }
        }

        public UserRowData(UserSummary summary)
        {
            UserName = summary.UserName;
            DisplayName = summary.DisplayName;
            Enabled = summary.Enabled;
            LastLoginTime = summary.LastLoginTime;
        }
    }
}
