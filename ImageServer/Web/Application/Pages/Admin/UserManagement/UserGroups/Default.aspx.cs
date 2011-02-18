#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Security.Permissions;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Application.App_GlobalResources;

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
												try
												{
													_controller.DeleteUserGroup(userGroup);
												}
												catch (Exception)
												{
													DeleteErrorMessage.Message = string.Format(ErrorMessages.AdminUserGroups_DeleteUserGroupFailed,userGroup.Name);
													DeleteErrorMessage.MessageStyle = "color: red; font-weight: bold";
													DeleteErrorMessage.MessageType = MessageBox.MessageTypeEnum.ERROR;
													DeleteErrorMessage.Show();
												}
                                            	UserGroupsPanel.UpdateUI();
                                            };

            SetPageTitle(App_GlobalResources.Titles.UserGroupsPageTitle);
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
            DeleteConfirmation.Message = string.Format(SR.AdminUserGroups_DeleteDialog_AreYouSure, userGroupRowData.Name);
            DeleteConfirmation.MessageType = MessageBox.MessageTypeEnum.YESNO;
            DeleteConfirmation.Data = userGroupRowData;
            DeleteConfirmation.Show();
        }
    }
}
