using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Web.Application
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception baseException = Server.GetLastError();
			if (baseException != null)
			{
				baseException = baseException.GetBaseException();

				Server.ClearError();

				string message = baseException.Message;
				string source = baseException.Source;
				string stackTrace = baseException.StackTrace;

				string logMessage = string.Format("Message: {0}\nSource:{1}\nStack Trace:{2}", message, source, stackTrace);
				Platform.Log(LogLevel.Error, logMessage);
				Context.Items.Add(ImageServerConstants.ContextKeys.StackTrace, logMessage);
			}
            Server.Transfer(ImageServerConstants.PageURLs.ErrorPage);
        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}