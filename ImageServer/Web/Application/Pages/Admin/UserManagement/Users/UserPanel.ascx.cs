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

[assembly: WebResource("ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel.js", "application/x-javascript")]

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users
{
    [ClientScriptResource(ComponentType = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel", ResourcePath = "ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users.UserPanel.js")]
    public partial class UserPanel : AJAXScriptControl
    {
        [ExtenderControlProperty]
        [ClientPropertyName("DeleteButtonClientID")]
        public string DeleteButtonClientID
        {
            get { return DeleteUserButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("EditButtonClientID")]
        public string EditButtonClientID
        {
            get { return EditUserButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("ResetPasswordButtonClientID")]
        public string ResetPasswordButtonClientID
        {
            get { return ResetPasswordButton.ClientID; }
        }

        [ExtenderControlProperty]
        [ClientPropertyName("UserListClientID")]
        public string UserListClientID
        {
            get { return UserGridPanel.UserGrid.ClientID; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GridPagerTop.InitializeGridPager(App_GlobalResources.SR.GridPagerUserSingleItemFound,
                                             App_GlobalResources.SR.GridPagerUserMultipleItemsFound,
                                             UserGridPanel.UserGrid, 
                                             delegate
                                                 {
                                                     return UserGridPanel.ResultCount;
                                                 },
                                             ImageServerConstants.GridViewPagerPosition.top);

            UserGridPanel.DataSourceCreated += delegate(UserDataSource source)
                            {
                                source.UserName = UserNameTextBox.Text;
                                source.DisplayName = DisplayNameTextBox.Text;
                            };
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            UpdateUI();
        }

        public void UpdateUI()
        {
            UserRowData userRow = UserGridPanel.SelectedUser;

            if (userRow == null)
            {
                // no device being selected
                EditUserButton.Enabled = false;
                DeleteUserButton.Enabled = false;
                ResetPasswordButton.Enabled = false;
            }
            else
            {
                EditUserButton.Enabled = true;
                DeleteUserButton.Enabled = true;
                ResetPasswordButton.Enabled = true;
            }

            // UpdatePanel UpdateMode must be set to "conditional"
            // Calling UpdatePanel.Update() will force the client to refresh the screen
            SearchUpdatePanel.Update();
        }

        protected void AddUserButton_Click(object sender, ImageClickEventArgs e)
        {             
            ((Default)Page).OnAddUser();
        }

        protected void EditUserButton_Click(object sender, ImageClickEventArgs e)
        {
            UserRowData user = UserGridPanel.SelectedUser;
            if (user != null) ((Default)Page).OnEditUser(user);
        }

        protected void DeleteUserButton_Click(object sender, ImageClickEventArgs e)
        {            
            UserRowData user = UserGridPanel.SelectedUser;
            if (user != null) ((Default)Page).OnDeleteUser(user);
        }

        protected void ResetPasswordButton_Click(object sender, ImageClickEventArgs e)
        {
            UserRowData user = UserGridPanel.SelectedUser;
            if (user != null) ((Default)Page).OnResetPassword(user);
        }

    	protected void SearchButton_Click(object sender, ImageClickEventArgs e)
    	{
    		UserGridPanel.Refresh();
    	}
    }
}