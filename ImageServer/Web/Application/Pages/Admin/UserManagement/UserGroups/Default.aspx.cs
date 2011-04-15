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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Helpers;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using Resources;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.UserGroups
{
    [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.AuthorityGroup)]
    public partial class Default : BasePage
    {
    	readonly UserManagementController _controller = new UserManagementController();

        protected void Page_Load(object sender, EventArgs e)
        {
            AddEditUserGroupsDialog.OKClicked += AddEditUserGroupsDialog_OKClicked;
            DeleteConfirmation.Confirmed += DeleteConfirmationDialog_Confirmed;
            DeleteNonEmptyGroupConfirmation.Confirmed += DeleteNonEmptyGroupConfirmationDialog_Confirmed;
            SetPageTitle(Titles.UserGroupsPageTitle);
        }

        public void OnAddUserGroup()
        {
            //TODO: Rewrite this. This method is being called by the child control.
            AddEditUserGroupsDialog.EditMode = false;
            AddEditUserGroupsDialog.Show(true);
        }

        public void OnEditUserGroup(UserGroupRowData userGroupRowData)
        {
            //TODO: Rewrite this. This method is being called by the child control.
            
            AddEditUserGroupsDialog.EditMode = true;
            AddEditUserGroupsDialog.UserGroup = userGroupRowData;
            AddEditUserGroupsDialog.Show(true);
        }

        public void OnDeleteUserGroup(UserGroupRowData userGroupRowData)
        {
            //TODO: Rewrite this. This method is being called by the child control.

            DeleteConfirmation.Message = string.Format(SR.AdminUserGroups_DeleteDialog_AreYouSure, userGroupRowData.Name);
            DeleteConfirmation.MessageType = MessageBox.MessageTypeEnum.YESNO;
            DeleteConfirmation.Data = userGroupRowData;
            DeleteConfirmation.Show();
        }

        private bool AddEditUserGroupsDialog_OKClicked(UserGroupRowData userGroup)
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
        }

        private void DeleteConfirmationDialog_Confirmed(object data)
        {
            // delete the device and reload the affected partition.
            UserGroupRowData userGroup = data as UserGroupRowData;
            try
            {
                _controller.DeleteUserGroup(userGroup, true);
            }
            catch (AuthorityGroupIsNotEmptyException ex)
            {
                DeleteNonEmptyGroupConfirmation.Data = data;
                DeleteNonEmptyGroupConfirmation.Message =String.Format(SR.AdminUserGroup_DeleteNonEmptyGroupDialog_AreYouSure, ExceptionTranslator.Translate(ex));
                DeleteNonEmptyGroupConfirmation.MessageType = MessageBox.MessageTypeEnum.YESNO;
                DeleteNonEmptyGroupConfirmation.MessageStyle = "color: red; font-weight: bold";
                DeleteNonEmptyGroupConfirmation.Show();

            }
            catch (Exception ex)
            {
                DeleteErrorMessage.Message = ExceptionTranslator.Translate(ex);
                DeleteErrorMessage.MessageStyle = "color: red; font-weight: bold";
                DeleteErrorMessage.MessageType = MessageBox.MessageTypeEnum.ERROR;
                DeleteErrorMessage.Show();
            }
            UserGroupsPanel.UpdateUI();
        }


        private void DeleteNonEmptyGroupConfirmationDialog_Confirmed(object data)
        {
            UserGroupRowData userGroup = data as UserGroupRowData;
            try
            {
                _controller.DeleteUserGroup(userGroup, false /* don't check if the group is empty. User has given the go head*/);
            }
            catch (Exception ex)
            {
                DeleteErrorMessage.Message = ExceptionTranslator.Translate(ex);
                DeleteErrorMessage.MessageStyle = "color: red; font-weight: bold";
                DeleteErrorMessage.MessageType = MessageBox.MessageTypeEnum.ERROR;
                DeleteErrorMessage.Show();
            }
            UserGroupsPanel.UpdateUI();
        }


    }
}
