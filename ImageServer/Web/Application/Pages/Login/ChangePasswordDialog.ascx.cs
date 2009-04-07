using System;
using System.Web.Security;
using System.Web.UI;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Login
{
    public partial class ChangePasswordDialog : UserControl
    {
        public void Show(bool updateUI)
        {
            ChangePasswordUsername.Text = ((System.Web.UI.WebControls.TextBox)Page.FindControl("UserName")).Text;
            LoginPasswordChange.Checked = true;
            
            ModalDialog1.Show();
        }

        public void Cancel_Click(object sender, EventArgs e)
        {
            ModalDialog1.Hide();
        }

        public void ChangePassword_Click(object sender, EventArgs e)
        {
            using(LoginService service = new LoginService())
            {
                try
                {
                    SessionInfo session = service.Login(ChangePasswordUsername.Text, OriginalPassword.Text);

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
                        }
                        else
                        {
                            ModalDialog1.Hide();
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErrorMessage.Text = "Invalid Username/Password combination.";
                    ErrorMessagePanel.Visible = true;
                    // May want to elimiate this.
                    Platform.Log(LogLevel.Error, ex, "Unexpected exception changing password: {0}.", ex.Message);
                }   
            }
        }
    }
}