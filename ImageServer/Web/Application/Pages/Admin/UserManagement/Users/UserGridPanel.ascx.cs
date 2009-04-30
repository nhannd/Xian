#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using GridView=ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users
{
    public partial class UserGridPanel : System.Web.UI.UserControl
    {
        #region Delegates
        public delegate void UserDataSourceCreated(UserDataSource theSource);
        public event UserDataSourceCreated DataSourceCreated;
        public delegate void UserSelectedEventHandler(object sender, UserRowData selectedUser);
        public event UserSelectedEventHandler OnUserSelectionChanged;
        #endregion

        #region Private members
        // list of studies to display
        private UserDataSource _dataSource;
        private IList<UserRowData> _userRows;
        #endregion Private members
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UserGridView.SelectedIndexChanged += UserGridView_SelectedIndexChanged;

            UserGridView.DataSource = UserDataSourceObject;
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

        private void CustomizeUserGroupsColumn(GridViewRowEventArgs e)
        {
            TextBox textBox = ((TextBox)e.Row.FindControl("UserGroupTextBox"));
            UserRowData rowData = e.Row.DataItem as UserRowData;
            string groupList = string.Empty;
            foreach (UserGroup userGroup in rowData.UserGroups)
            {
                groupList += userGroup.Name + "\n";
            }
            textBox.Text = groupList;
        }

        public UserRowData SelectedUser
        {
            get
            {
                if(!UserGrid.IsDataBound) UserGrid.DataBind();

                if (Users.Count == 0 || UserGrid.SelectedIndex < 0)
                    return null;

                // SelectedIndex is for the current page. Must convert to the index of the entire list
                int index = UserGrid.PageIndex * UserGrid.PageSize + UserGrid.SelectedIndex;

                if (index < 0 || index > Users.Count - 1)
                    return null;

                return Users[index];
            }
            set
            {                
                UserGrid.SelectedIndex = Users.IndexOf(value);
                if (OnUserSelectionChanged != null)
                    OnUserSelectionChanged(this, value);
            }
        }

        public IList<UserRowData> Users
        {
            get { return _userRows; }
            set
            {
                _userRows = value;
                UserGrid.DataSource = _userRows; // must manually call DataBind() later
            }
        }

        protected void UserGridView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UserRowData userRow = SelectedUser;
            if (userRow != null)
                if (OnUserSelectionChanged != null)
                    OnUserSelectionChanged(this, userRow);

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
            if (UserGridView.EditIndex != e.Row.RowIndex)
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    CustomizeUserGroupsColumn(e);
                }
            }
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
