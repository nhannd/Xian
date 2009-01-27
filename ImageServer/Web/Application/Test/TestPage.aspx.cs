using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Security.Permissions;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Services.Admin;
using ClearCanvas.ImageServer.Common.Services.Login;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Test
{
    public partial class TestPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CustomIdentity id = SessionManager.Current.User.Identity as CustomIdentity;
            Message.Text = "Welcome <B>"+id.DisplayName + "</B>. Your session will expire on " +
                           SessionManager.Current.Credentials.SessionToken.ExpiryTime;
            
        }

        protected void Logout(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }

        protected void ChangePasswordClicked(object sender, EventArgs e)
        {
            LoginCredentials credential = SessionManager.Current.Credentials;
            Platform.GetService<ILoginService>(
                delegate(ILoginService service)
                    {
                        try
                        {
                            service.ChangePassword(credential.UserName, OldPassword.Text, NewPassword.Text);
                            ChangePasswordMessage.Text = "Password changed";

                        }
                        catch (Exception ex)
                        {
                            ChangePasswordMessage.Text = ex.Message;
                        }
                    });
            
            
        }

        protected void DeleteUserClicked(object sender, EventArgs e)
        {
            Platform.GetService<IAdminServices>(
                delegate(IAdminServices service)
                {
                    try
                    {
                        service.DeleteUser(DeleteUserName.Text);
                        DeleteUserMessage.Text = "User deleted";

                    }
                    catch (Exception ex)
                    {
                        DeleteUserMessage.Text = ex.Message;
                    }
                });
        }

        protected void ResetPasswordClicked(object sender, EventArgs e)
        {
            Platform.GetService<IAdminServices>(
                delegate(IAdminServices service)
                {
                    try
                    {
                        service.ResetPassword(ResetPasswordUserName.Text);
                        ResetPasswordMessage.Text = "Password reset";

                    }
                    catch (Exception ex)
                    {
                        ResetPasswordMessage.Text = ex.Message;
                    }
                });
        }
    }
}
