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
    }
}