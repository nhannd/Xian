using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Web.Common
{
    /// <summary>
    /// Used to handle web application error. 
    /// </summary>
    /// <remarks>
    /// This module allows capture error and write to the log file. To use it, specify this in the <httpModules> section of web.config:
    /// 
    ///     <add  name="ErrorModule" type="ClearCanvas.ImageServer.Web.Common.ErrorModule, ClearCanvas.ImageServer.Web.Common" />
    /// 
    /// </remarks>
    public class ErrorModule : IHttpModule
    {
        #region IHttpModule Members
        public void Init(HttpApplication application)
        {
            application.Error += new EventHandler(application_Error);
            
        }
        public void Dispose() { }
        #endregion

        public void application_Error(object sender, EventArgs e)
        {
            
            HttpContext ctx = HttpContext.Current;
            Exception theException;

            for (theException = ctx.Server.GetLastError(); theException.InnerException != null; theException = theException.InnerException)
            {
                
            }

            if (theException is HttpException)
            {
                HttpException exception = theException as HttpException;
                Platform.Log(LogLevel.Error, "HTTP Error {0}: {0}", exception.ErrorCode, exception);
            }
            else
            {
                Platform.Log(LogLevel.Error, "Unhandled exception: {0}", theException);
            }
        }
    }

}
