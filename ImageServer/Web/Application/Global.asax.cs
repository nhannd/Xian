using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Web.Common.Security;

namespace ClearCanvas.ImageServer.Web.Application
{
    public class Global : System.Web.HttpApplication
    {
        private DateTime start;

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            start = DateTime.Now;
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (ConfigurationManager.AppSettings["PerformanceLogging"].Equals("true"))
            {
                //Ignore some of the requests
                if (Request.Url.AbsoluteUri.Contains("PersistantImage.ashx") ||
                    Request.Url.AbsoluteUri.Contains("WebResource.axd") ||
                    Request.Url.AbsoluteUri.Contains("ScriptResource.axd") ||
                    Request.Url.AbsoluteUri.Contains("Pages/Login") ||
                    Request.Url.AbsoluteUri.Contains("Pages/Error") ||
                    Request.Url.AbsoluteUri.Contains("&error=")) return;
                TimeSpan elapsedTime = DateTime.Now.Subtract(start);
                string processingTime = elapsedTime.Minutes + ":" + elapsedTime.Seconds + ":" + elapsedTime.Milliseconds;

                string userName = "Not Logged In.";
                if (SessionManager.Current != null)
                {
                    userName = SessionManager.Current.User.Credentials.UserName;
                }
                Platform.Log(LogLevel.Debug,
                             string.Format("USER: {0} URL: {1} PROCESSING TIME: {2}", userName,
                                           this.Request.Url.AbsoluteUri, processingTime));
            }
        }
    }
}