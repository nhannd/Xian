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
using System.Web.UI;
using AjaxControlToolkit;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.WebControls.UI;

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups
{

    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel", ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups.UserGroupsPanel.js")]
    public partial class UserGroupsPanel : AJAXScriptControl
    {
        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteUserGroupButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string EditButtonClientID
        {
            get { return EditUserGroupButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("UserGroupsListClientID")]
        public string UserGroupsListClientID
        {
            get { return UserGroupsGridPanel.UserGroupGrid.ClientID; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GridPagerTop.InitializeGridPager(App_GlobalResources.SR.GridPagerUserGroupsSingleItemFound,
                                             App_GlobalResources.SR.GridPagerUserGroupsMultipleItemsFound,
                                             UserGroupsGridPanel.UserGroupGrid, 
                                             delegate
                                                 {
                                                     return UserGroupsGridPanel.ResultCount;
                                                 },
                                             ImageServerConstants.GridViewPagerPosition.top);

            UserGroupsGridPanel.DataSourceCreated += delegate(UserGroupDataSource source)
                            {
                                source.GroupName = GroupName.Text;
                            };

        }

        public void UpdateUI()
        {
            UserGroupRowData userGroupRow = UserGroupsGridPanel.SelectedUserGroup;

            if (userGroupRow == null)
            {
                // no device being selected
                EditUserGroupButton.Enabled = false;
                DeleteUserGroupButton.Enabled = false;
            }
            else
            {
                EditUserGroupButton.Enabled = true;
                DeleteUserGroupButton.Enabled = true;
            }

            // UpdatePanel UpdateMode must be set to "conditional"
            // Calling UpdatePanel.Update() will force the client to refresh the screen
            SearchUpdatePanel.Update();
            UserGroupsGridPanel.DataBind();
        }

        protected void AddUserGroupButton_Click(object sender, ImageClickEventArgs e)
        {
            ((Default)Page).OnAddUserGroup();
        }

        protected void EditUserGroupButton_Click(object sender, ImageClickEventArgs e)
        {
            UserGroupRowData userGroup = UserGroupsGridPanel.SelectedUserGroup;
            if (userGroup != null) ((Default)Page).OnEditUserGroup(userGroup);
        }

        protected void DeleteUserGroupButton_Click(object sender, ImageClickEventArgs e)
        {
            UserGroupRowData userGroup = UserGroupsGridPanel.SelectedUserGroup;
            if (userGroup != null) ((Default)Page).OnDeleteUserGroup(userGroup);
        }

    	protected void SearchButton_Click(object sender, ImageClickEventArgs e)
    	{
    		UserGroupsGridPanel.Refresh();
    	}
    }
}