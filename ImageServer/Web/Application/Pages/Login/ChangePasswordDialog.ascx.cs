using System;
using System.ServiceModel;
using System.Web.Security;
using System.Web.UI;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Web.Application.App_GlobalResources;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Login
{
    public partial class ChangePasswordDialog : UserControl
    {
        public void Show(bool updateUI)
        {
            ChangePasswordUsername.Text = ((System.Web.UI.WebControls.TextBox)Page.FindControl("UserName")).Text;
            LoginPasswordChange.Checked = true;
            ErrorMessage.Text = String.Empty;
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
                    SessionInfo session = service.Login(ChangePasswordUsername.Text, OriginalPassword.Text);

                    if (!NewPassword.Text.Equals(ConfirmNewPassword.Text) || NewPassword.Text.Equals(string.Empty))
                    {
                        ErrorMessage.Text = ErrorMessages.PasswordsDoNotMatch;
                        ErrorMessagePanel.Visible = true;
                    }
                    else
                    {
                        service.ChangePassword(ChangePasswordUsername.Text, OriginalPassword.Text,NewPassword.Text);

                        session = service.Login(ChangePasswordUsername.Text, NewPassword.Text);
                        SessionManager.InitializeSession(session);

                        if (LoginPasswordChange.Checked)
                        {
                            Response.Redirect(
                                FormsAuthentication.GetRedirectUrl(ChangePasswordUsername.Text, false), false);
                        }
                        else
                        {
                            ModalDialog1.Hide();
                        }
                    }
                }
                catch (FaultException<RequestValidationException> ex)
                {
                    // NOTE: The server is throwing FaultException<RequestValidationException> when username or password doesn't match the configured policy
                    Platform.Log(LogLevel.Error, ex, "Unable to change password for {0}", ChangePasswordUsername.Text);
                    string error = String.Format(ErrorMessages.ChangePasswordError, ex.Detail.Message);
                    ShowError(error);
                }
                catch (FaultException ex)
                {
                    // NOTE: The server is throwing FaultException when username or password is invalid. 
                    Platform.Log(LogLevel.Error, ex, "Unable to change password for {0}", ChangePasswordUsername.Text);
                    ShowError(ErrorMessages.LoginInvalidUsernameOrPassword);
                }
                catch (CommunicationException ex)
                {
                    Platform.Log(LogLevel.Error, ex, ErrorMessages.CannotContactEnterpriseServer);
                    ShowError(ErrorMessages.CannotContactEnterpriseServer);
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                    // May want to elimiate this.
                    Platform.Log(LogLevel.Error, ex, "Unexpected exception changing password: {0}.", ex.Message);
                }
            }
        }

        private void ShowError(string error)
        {
            ErrorMessage.Text = error;
            ErrorMessagePanel.Visible = true;
        }
    }
}