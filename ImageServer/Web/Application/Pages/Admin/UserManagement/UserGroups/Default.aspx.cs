using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups
{
    public partial class Default : BasePage
    {
        UserManagementController _controller = new UserManagementController();

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
                                                           }
                                                       }
                                                       else
                                                       {
                                                           if (_controller.AddUserGroup(userGroup))
                                                           {
                                                               UserGroupsPanel.UpdateUI();
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
