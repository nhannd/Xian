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
using System.Data;
using System.Configuration;
using System.Collections;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Application.Controls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Admin.UserManagement.Users
{
    [PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Enterprise.Common.AuthorityTokens.Admin.Security.User)]
    public partial class Default : BasePage
    {
        UserManagementController _controller = new UserManagementController();

        protected void Page_Load(object sender, EventArgs e)
        {
            SetupEventHandlers();

            Page.Title = App_GlobalResources.Titles.UserManagementPageTitle;
        }


        /// <summary>
        /// Set up the event handlers for child controls.
        /// </summary>
        protected void SetupEventHandlers() {

            AddEditUserDialog.OKClicked += delegate(UserRowData user)
                                                   {
                                                       if (AddEditUserDialog.EditMode)
                                                       {
                                                           // Commit the change into database
                                                           if (_controller.UpdateUser(user))
                                                           {
                                                               UserPanel.UpdateUI();
                                                               return true;
                                                           }
                                                           return false;
                                                       }
                                                       else
                                                       {
                                                           try
                                                           {
                                                               if (_controller.AddUser(user))
                                                               {
                                                                   UserPanel.UpdateUI();
                                                                   return true;
                                                               }
                                                               return false;
                                                           }
														   catch(Exception)
                                                           {
                                                               return false;
                                                           }
                                                       }
                                                   };


            DeleteConfirmation.Confirmed += delegate(object data)
                                            {
                                                // delete the device and reload the affected partition.

                                                UserRowData user = data as UserRowData;
                                                _controller.DeleteUser(user);
                                                UserPanel.UpdateUI();
                                            };

 
        }

        public void OnAddUser()
        {
            AddEditUserDialog.EditMode = false;
            AddEditUserDialog.Show(true);
        }

        public void OnEditUser(UserRowData userRowData)
        {
            AddEditUserDialog.EditMode = true;
            AddEditUserDialog.User = userRowData;
            AddEditUserDialog.Show(true);
        }

        public void OnDeleteUser(UserRowData userRowData)
        {
            DeleteConfirmation.Message = string.Format("Are you sure to delete user \"{0} ({1})?\"", userRowData.DisplayName, userRowData.UserName);
            DeleteConfirmation.MessageType = MessageBox.MessageTypeEnum.YESNO;
            DeleteConfirmation.Data = userRowData;
            DeleteConfirmation.Show();
        }

        public void OnResetPassword(UserRowData userRowData)
        {
            if (_controller.ResetPassword(userRowData))
            {
                PasswordResetConfirmation.Message = string.Format("Password for {0} has been reset.", userRowData.UserName);
            } else {
                PasswordResetConfirmation.Message =
                    string.Format("An error occurred resetting the password. Password has not been changed.");
                }
            PasswordResetConfirmation.Title = "Password Reset";
            PasswordResetConfirmation.MessageType = MessageBox.MessageTypeEnum.INFORMATION;
            PasswordResetConfirmation.Show();
        }
    }
}
