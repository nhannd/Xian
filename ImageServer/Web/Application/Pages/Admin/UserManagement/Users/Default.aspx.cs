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
                                                                   AddEditUserDialog.Close();
                                                                   return true;
                                                               }
                                                               return false;
                                                           }catch(Exception e)
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
    }
}
