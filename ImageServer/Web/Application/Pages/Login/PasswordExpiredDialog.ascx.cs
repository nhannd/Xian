using System;
using System.Web.Security;
using System.Web.UI;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Login
{
    public partial class PasswordExpiredDialog : UserControl
    {
        public void Show(string username, string password)
        {
            Username.Text = username;
            OriginalPassword.Value = password;
            ErrorMessagePanel.Visible = false;
            
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
                    if (!NewPassword.Text.Equals(ConfirmNewPassword.Text) || NewPassword.Text.Equals(string.Empty))
                    {
                        ErrorMessage.Text =
                            "Provided passwords don't match.<br/>Please retype your passwords and try again.";
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
                    ErrorMessage.Text = ex.Message;
                    ErrorMessagePanel.Visible = true;
					// May want to elimiate this.
					Platform.Log(LogLevel.Error, ex, "Unexpected exception changing password: {0}.", ex.Message);
				}
            }
        }
    }
}