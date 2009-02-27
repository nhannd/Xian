using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Security;
using GridView=ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups
{
    public partial class UserGroupsGridPanel : System.Web.UI.UserControl
    {
        #region Delegates
        public delegate void UserGroupDataSourceCreated(UserGroupDataSource theSource);
        public event UserGroupDataSourceCreated DataSourceCreated;

        /// <summary>
        /// Defines the handler for <seealso cref="OnUserGroupSelectionChanged"/> event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="selectedUserGroup"></param>
        public delegate void UserGroupSelectedEventHandler(object sender, UserGroupRowData selectedUserGroup);

        /// <summary>
        /// Occurs when the selected device in the list is changed.
        /// </summary>
        /// <remarks>
        /// The selected device can change programmatically or by users selecting the device in the list.
        /// </remarks>
        public event UserGroupSelectedEventHandler OnUserGroupSelectionChanged;

        #endregion

        #region Private members
        // list of studies to display
        private UserGroupDataSource _dataSource;
        private IList<UserGroupRowData> _userGroupRows;
        #endregion Private members

        private void CustomizeTokensColumn(GridViewRowEventArgs e)
        {
            TextBox textBox = ((TextBox) e.Row.FindControl("TokensTextBox"));
            Label tokenCount = ((Label)e.Row.FindControl("TokenCount"));
            UserGroupRowData rowData = e.Row.DataItem as UserGroupRowData;

            if (rowData != null)
            {
                string tokenList = string.Empty;
                foreach (TokenSummary token in rowData.Tokens)
                {
                    tokenList += token.Description + "\n";
                }
                textBox.Text = tokenList;
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
        }

        /// <summary>
        /// Retrieve reference to the grid control being used to display the devices.
        /// </summary>
        public GridView UserGroupGrid
        {
            get { return UserGroupsGridView; }
        }

        public int ResultCount
        {
            get
            {
                if (_dataSource == null)
                {
                    _dataSource = new UserGroupDataSource();

                    _dataSource.UserGroupFoundSet += delegate(IList<UserGroupRowData> newlist)
                                            {
                                                _userGroupRows = newlist;
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

        /// <summary>
        /// Gets/Sets the list of users rendered on the screen.
        /// </summary>
        public IList<UserGroupRowData> UserGroups
        {
            get { return _userGroupRows; }
            set
            {
                _userGroupRows = value;
                UserGroupGrid.DataSource = _userGroupRows; // must manually call DataBind() later
            }
        }

        /// <summary>
        /// Gets/Sets the current selected device.
        /// </summary>
        public UserGroupRowData SelectedUserGroup
        {
            get
            {
                if (UserGroups.Count == 0 || UserGroupGrid.SelectedIndex < 0)
                    return null;

                // SelectedIndex is for the current page. Must convert to the index of the entire list
                int index = UserGroupGrid.PageIndex * UserGroupGrid.PageSize + UserGroupGrid.SelectedIndex;

                if (index < 0 || index > UserGroups.Count - 1)
                    return null;

                return UserGroups[index];
            }
            set
            {
                UserGroupGrid.SelectedIndex = UserGroups.IndexOf(value);
                if (OnUserGroupSelectionChanged != null)
                    OnUserGroupSelectionChanged(this, value);
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

        protected void UserGroupsGridView_DataBound(object sender, EventArgs e)
        {
            // reselect the row based on the new order
//            if (SelectedAlertKey != null)
//            {
//                UserGridView.SelectedIndex = _userRows.RowIndexOf(SelectedAlertKey, AlertGridView);
//            }
        }

        protected void UserGroupsGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
//            if (AlertGridView.SelectedDataKey != null)
                //SelectedAlertKey = AlertGridView.SelectedDataKey.Value as ServerEntityKey;

            DataBind();
        }

        protected void UserGroupsGridView_PageIndexChanged(object sender, EventArgs e)
        {
            DataBind();
        }

        protected void UserGroupsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            UserGroupsGridView.PageIndex = e.NewPageIndex;
            DataBind();
        }

        protected void UserGroupsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (UserGroupGrid.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    CustomizeTokensColumn(e);
                }
            }

        }

        protected void DisposeUserGroupsDataSource(object sender, ObjectDataSourceDisposingEventArgs e)
        {
            e.Cancel = true;
        }

        protected void GetUserGroupDataSource(object sender, ObjectDataSourceEventArgs e)
        {
            if (_dataSource == null)
            {
                _dataSource = new UserGroupDataSource();

                _dataSource.UserGroupFoundSet += delegate(IList<UserGroupRowData> newlist)
                                        {
                                            _userGroupRows = newlist;
                                        };
            }

            e.ObjectInstance = _dataSource;

            if (DataSourceCreated != null)
                DataSourceCreated(_dataSource);

        }
    }
}
