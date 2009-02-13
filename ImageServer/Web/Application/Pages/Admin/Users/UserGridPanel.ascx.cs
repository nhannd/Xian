using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Web.Application.Pages.Admin.Alerts;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Security;
using GridView=ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;
using IUserAdminService = ClearCanvas.ImageServer.Common.Services.Admin.IUserAdminService;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.Users
{
    public partial class UserGridPanel : System.Web.UI.UserControl
    {
        #region Delegates
        public delegate void UserDataSourceCreated(UserDataSource theSource);
        public event UserDataSourceCreated DataSourceCreated;
        #endregion

        #region Private members
        // list of studies to display
        private UserDataSource _dataSource;
        private IList<UserRowData> _userRows;
        #endregion Private members
        
        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
        }



        /// <summary>
        /// Retrieve reference to the grid control being used to display the devices.
        /// </summary>
        public GridView UserGrid
        {
            get { return UserGridView; }
        }

        public int ResultCount
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new UserDataSource();

                    _dataSource.UserFoundSet += delegate(IList<UserRowData> newlist)
                                            {
                                                _userRows = newlist;
                                            };
                    if (DataSourceCreated != null)
                        DataSourceCreated(_dataSource);
                    _dataSource.SelectCount();
                }
                if (_dataSource.ResultCount == 0)
                {
                    if (DataSourceCreated != null)
                        DataSourceCreated(_dataSource);

                    _dataSource.SelectCount();
                }
                return _dataSource.ResultCount;
            }
        }

        public override void DataBind()
        {
            LoginCredentials credential = SessionManager.Current.Credentials;
/*            CurrentUser.Text = String.Format("{0}: {1} expire at: {2}",
                Thread.CurrentPrincipal,
                    credential.DisplayName,
                    credential.SessionToken.ExpiryTime
                );
            */

            base.DataBind();

        }

        protected void UserGridView_DataBound(object sender, EventArgs e)
        {
            // reselect the row based on the new order
//            if (SelectedAlertKey != null)
//            {
//                UserGridView.SelectedIndex = _userRows.RowIndexOf(SelectedAlertKey, AlertGridView);
//            }
        }

        protected void AlertGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
//            if (AlertGridView.SelectedDataKey != null)
                //SelectedAlertKey = AlertGridView.SelectedDataKey.Value as ServerEntityKey;

            DataBind();
        }

        protected void UserGridView_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void UserGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            UserGridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected void UserGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void UserGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
//            if (UserGridView.SelectedDataKey != null)
                //SelectedAlertKey = AlertGridView.SelectedDataKey.Value as ServerEntityKey;

            DataBind();
        }

        protected void DisposeUserDataSource(object sender, ObjectDataSourceDisposingEventArgs e)
        {
            e.Cancel = true;
        }

        protected void GetUserDataSource(object sender, ObjectDataSourceEventArgs e)
        {
            if (_dataSource == null)
            {
                _dataSource = new UserDataSource();

                _dataSource.UserFoundSet += delegate(IList<UserRowData> newlist)
                                        {
                                            _userRows = newlist;
                                        };
            }

            e.ObjectInstance = _dataSource;

            if (DataSourceCreated != null)
                DataSourceCreated(_dataSource);

        }
    }
}
