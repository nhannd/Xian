#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Web.Common.Exceptions;

namespace ClearCanvas.ImageServer.Web.Common.Modules
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

        public void Dispose()
        {
        }

        #endregion

        public void OnError(object obj, EventArgs args)
        {
            HttpContext context = HttpContext.Current;

            Exception baseException = context.Server.GetLastError();
            Platform.Log(LogLevel.Error, context.Error);

            if (baseException != null)
            {
                baseException = baseException.GetBaseException();

                context.Server.ClearError();

                string logMessage = string.Format("Message: {0}\nSource:{1}\nStack Trace:{2}", baseException.Message, baseException.Source, baseException.StackTrace);
                Platform.Log(LogLevel.Error, logMessage);

                ExceptionHandler.ThrowException(baseException);
            }
        }
        
        public void application_Error(object sender, EventArgs e)
        {
            HttpContext ctx = HttpContext.Current;
            Exception theException;
            Platform.Log(LogLevel.Error, ctx.Error);

            for (theException = ctx.Server.GetLastError();
                 theException != null && theException.InnerException != null;
                 theException = theException.InnerException)
            {
            }


            if(theException != null && (theException.Message.Equals("Access is denied.") || theException.Message.Equals("Request for principal permission failed.")))
            {
                ExceptionHandler.ThrowException(new AuthorizationException());
            } 
            else if (theException is HttpException)
            {
                HttpException exception = theException as HttpException;
                Platform.Log(LogLevel.Error, "HTTP Error {0}: {0}", exception.ErrorCode, exception);
            }
            else
            {
                Platform.Log(LogLevel.Error, "Unhandled exception: {0}", theException);
            }

            ExceptionHandler.ThrowException(theException);
        }
    }
}
