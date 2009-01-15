using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.Services.Login;

namespace ClearCanvas.ImageServer.Web.Application
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void LoginClicked(object sender, EventArgs e)
        {
            ILoginService service = Platform.GetService<ILoginService>();
            LoginResult result = service.SignIn(UserName.Text, Password.Text);
            if (result.Successful)
            {
                Result.Text = "Login successfully. You are part of ";
                foreach(string group in result.Groups)
                {
                    Result.Text += group + ",";
                }
            }
            else
            {
                Result.Text = "Login failed";
            }
        }
    }
}
