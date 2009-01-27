using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Services.Login;
using ClearCanvas.ImageServer.Services.Common.Login;
using ClearCanvas.ImageServer.Web.Common;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Test
{
    public partial class Login : System.Web.UI.Page
    {
        private SessionToken _token;
        private string[] _groups;
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LoginClicked(object sender, EventArgs e)
        {
            Platform.GetService<ILoginService>(
                delegate(ILoginService service)
                {
                    try
                    {
                        SessionInfo session = service.Login(UserName.Text, Password.Text);
                        SessionManager.InitializeSession(session);
                        Response.Redirect(FormsAuthentication.GetRedirectUrl(UserName.Text, false));
                    }
                    catch (PasswordExpiredException ex)
                    {
                        service.ChangePassword(UserName.Text, Password.Text, "NewPassword123");
                        SessionInfo session = service.Login(UserName.Text, "NewPassword123");
                        SessionManager.InitializeSession(session);
                    }
                });
            
        }
    }
}