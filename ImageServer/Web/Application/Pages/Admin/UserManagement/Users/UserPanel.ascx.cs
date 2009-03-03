using System;
using System.Web.UI;
using AjaxControlToolkit;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.ImageServer.Model;
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

        protected void Page_Load(object sender, EventArgs e)
        {

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
            GridPagerBottom.InitializeGridPager(App_GlobalResources.SR.GridPagerUserSingleItemFound,
                                                App_GlobalResources.SR.GridPagerUserMultipleItemsFound,
                                                UserGridPanel.UserGrid,
                                                delegate { return UserGridPanel.ResultCount; },
                                                ImageServerConstants.GridViewPagerPosition.bottom);

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
            UserGridPanel.DataBind();
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

    }
}