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
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using GridView=ClearCanvas.ImageServer.Web.Common.WebControls.UI.GridView;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups
{
    public partial class UserGroupsGridPanel : GridViewPanel
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
                if(!UserGroupGrid.IsDataBound) UserGroupGrid.DataBind();

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

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TheGrid = UserGroupGrid;
            UserGroupGrid.DataSource = UserGroupDataSourceObject;
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
