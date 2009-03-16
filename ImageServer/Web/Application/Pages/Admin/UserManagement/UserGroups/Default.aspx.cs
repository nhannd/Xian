using System;
using System.Security.Permissions;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups
{
    [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.AuthorityGroup)]
    public partial class Default : BasePage
    {
    	readonly UserManagementController _controller = new UserManagementController();

        protected void Page_Load(object sender, EventArgs e)
        {
            AddEditUserGroupsDialog.OKClicked += delegate(UserGroupRowData userGroup)
                                                   {
                                                       if (AddEditUserGroupsDialog.EditMode)
                                                       {
                                                           // Commit the change into database
                                                           if (_controller.UpdateUserGroup(userGroup))
                                                           {
                                                               UserGroupsPanel.UpdateUI();
                                                               return true;
                                                           }
                                                           return false;
                                                       }
                                                       else
                                                       {
                                                           try
                                                           {
                                                               if (_controller.AddUserGroup(userGroup))
                                                               {
                                                                   UserGroupsPanel.UpdateUI();
                                                                   return true;
                                                               }
                                                               return false;
                                                           }
                                                           catch (Exception)
                                                           {
                                                               return false;
                                                           }
                                                       }
                                                   };


            DeleteConfirmation.Confirmed += delegate(object data)
                                            {
                                                // delete the device and reload the affected partition.

                                                UserGroupRowData userGroup = data as UserGroupRowData;
                                                _controller.DeleteUserGroup(userGroup);
                                                UserGroupsPanel.UpdateUI();
                                            };

            Page.Title = App_GlobalResources.Titles.UserGroupsPageTitle;
        }

        public void OnAddUserGroup()
        {
            AddEditUserGroupsDialog.EditMode = false;
            AddEditUserGroupsDialog.Show(true);
        }

        public void OnEditUserGroup(UserGroupRowData userGroupRowData)
        {
            AddEditUserGroupsDialog.EditMode = true;
            AddEditUserGroupsDialog.UserGroup = userGroupRowData;
            AddEditUserGroupsDialog.Show(true);
        }

        public void OnDeleteUserGroup(UserGroupRowData userGroupRowData)
        {
            DeleteConfirmation.Message = string.Format("Are you sure to delete user group \"{0}?\"", userGroupRowData.Name);
            DeleteConfirmation.MessageType = MessageBox.MessageTypeEnum.YESNO;
            DeleteConfirmation.Data = userGroupRowData;
            DeleteConfirmation.Show();
        }
    }
}
