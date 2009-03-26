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
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application.Controls
{
    public partial class SessionTimeout : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Set the interval of the timer to match the length of the timeout set for the session and
            //convert it to milliseconds.
            //if(!IsPostBack) SessionTimer.Interval = (Session.Timeout) * 60000;
        }
/*
        protected void TimeoutTimer_Tick(object sender, EventArgs e)
        {
            if (HttpContext.Current == null)
            {
                Page.Response.Redirect("~/Pages/Error/TimeoutErrorPage.aspx");
            }
            else if (Context.Session != null)
            {
                if (Context.Session.IsNewSession)
                {
                    string sCookieHeader = Page.Request.Headers["Cookie"];
                    if ((null != sCookieHeader) && (sCookieHeader.IndexOf("ASP.NET_SessionId") >= 0))
                    {
                        if (Page.Request.IsAuthenticated)
                        {
                            SessionManager.TerminateSession(false);
                        }
                        Page.Response.Redirect("~/Pages/Error/TimeoutErrorPage.aspx");
                    }
                }
            }

        }
        */
  }

}