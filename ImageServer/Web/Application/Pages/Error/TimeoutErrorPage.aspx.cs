using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using ClearCanvas.ImageServer.Web.Application.Pages.Common;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Pages.Error
{
    public partial class TimeoutErrorPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SessionManager.TerminateSession(false);
            TimeoutMinutes.Text = Session.Timeout.ToString();

            Page.Title = App_GlobalResources.Titles.TimeoutErrorPageTitle;
        }

        protected void Login_Click(Object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Login/Default.aspx");
        }

    }
}
