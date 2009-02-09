using System;
using System.Collections.Generic;
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
    public partial class ChangePasswordDialog : UserControl
    {
        public void Show(bool updateUI)
        {
            ChangePasswordUsername.Text = "";
            LoginPasswordChange.Checked = true;
            
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
                            SessionInfo session;

                            session = service.Login(ChangePasswordUsername.Text, OriginalPassword.Text);

                            if (!NewPassword.Text.Equals(ConfirmNewPassword.Text) || NewPassword.Text.Equals(string.Empty))
                            {
                                ErrorMessage.Text =
                                    "The new password provided is invalid.<br/>Please retype your passwords and try again.";
                                ErrorMessagePanel.Visible = true;
                            }
                            else
                            {
                                service.ChangePassword(ChangePasswordUsername.Text, OriginalPassword.Text,
                                                       NewPassword.Text);
                                session = service.Login(ChangePasswordUsername.Text, NewPassword.Text);
                                SessionManager.InitializeSession(session);

                                if (LoginPasswordChange.Checked)
                                {
                                    Response.Redirect(
                                        FormsAuthentication.GetRedirectUrl(ChangePasswordUsername.Text, false));
                                } else
                                {
                                    ModalDialog1.Hide();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorMessage.Text = "Invalid Username/Password combination.";
                            ErrorMessagePanel.Visible = true;
                        }
                    });
        }
    }
}