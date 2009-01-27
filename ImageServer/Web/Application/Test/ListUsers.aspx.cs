using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Services.Admin;
using ClearCanvas.ImageServer.Services.Common.Admin;
using ClearCanvas.ImageServer.Web.Common.Security;

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
            Login.Text = String.Format("{0} expire at: {1}",
                    credential.DisplayName,
                    credential.SessionToken.ExpiryTime
                );


            Platform.GetService<IAdminServices>(
                delegate(IAdminServices services)
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
