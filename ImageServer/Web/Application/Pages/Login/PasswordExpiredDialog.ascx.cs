using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Security;
using System.Web.UI;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Services.Login;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Web.Common.Data;
using ClearCanvas.ImageServer.Web.Common.Security;
using MessageBox=ClearCanvas.ImageServer.Web.Application.Controls.MessageBox;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Login
{
    public partial class PasswordExpiredDialog : UserControl
    {
        public void Show(string username, string password)
        {
            Username.Text = username;
            OriginalPassword.Value = password;
            
            ModalDialog1.Show();
        }

        public void Cancel_Click(object sender, EventArgs e)
        {
            ModalDialog1.Hide();
        }

        public void ChangePassword_Click(object sender, EventArgs e)
        {
            Platform.GetService<ILoginService>(
                delegate(ILoginService service)
                    {
                        try
                        {
                            if (!NewPassword.Text.Equals(ConfirmNewPassword.Text) || NewPassword.Text.Equals(string.Empty))
                            {
                                ErrorMessage.Text =
                                    "The new password provided is invalid.<br/>Please retype your passwords and try again.";
                                ErrorMessagePanel.Visible = true;
                            }
                            else
                            {
                                service.ChangePassword(Username.Text, OriginalPassword.Value, NewPassword.Text);
                                SessionInfo session = service.Login(Username.Text, NewPassword.Text);
                                SessionManager.InitializeSession(session);
                                Response.Redirect(FormsAuthentication.GetRedirectUrl(Username.Text, false));
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorMessage.Text = "Invalid Password. Password must be 8 - 16 characters long.";
                            ErrorMessagePanel.Visible = true;
                        }
                    });
        }
    }
}