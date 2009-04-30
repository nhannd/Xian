#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

namespace ClearCanvas.ImageServer.Web.Common.Exceptions
{
    public class ExceptionHandler
    {
        private static ExceptionHandler instance = new ExceptionHandler();
        private static HttpContext context;
 
        public static ExceptionHandler Instance()
        {
            return instance;
        }
 
        //instance members
        private ExceptionHandler()
        {
            context = HttpContext.Current;
        }

        public static void ThrowException(Exception e)
        {
            context = HttpContext.Current;
            Platform.Log(LogLevel.Error, e);
            context.Items.Add(ImageServerConstants.ContextKeys.ErrorMessage, e.Message);
            context.Items.Add(ImageServerConstants.ContextKeys.StackTrace, e.StackTrace);
            context.Server.Transfer(ImageServerConstants.PageURLs.ErrorPage);   
        }

        public static void ThrowException(BaseWebException e)
        {
            if (e != null)
            {                
                context = HttpContext.Current;

                context.Server.ClearError();

                string logMessage = string.Format("<Error>\n\t<CustomMessage>{0}</CustomMessage>\n\t<SystemMessage>{1}</SystemMessage>\n\t<Source>{2}</Source>\n\t<StackTrace>{3}</StackTrace>", e.LogMessage, e.Message, e.Source, e.StackTrace);
                Platform.Log(LogLevel.Error, logMessage);
                
                context.Items.Add(ImageServerConstants.ContextKeys.StackTrace, logMessage);

                if(e.ErrorMessage != null && !e.ErrorMessage.Equals(string.Empty))
                    context.Items.Add(ImageServerConstants.ContextKeys.ErrorMessage, e.ErrorMessage);
                if (e.ErrorDescription != null && !e.ErrorDescription.Equals(string.Empty))
                    context.Items.Add(ImageServerConstants.ContextKeys.ErrorDescription, e.ErrorDescription);
                
                context.Server.Transfer(ImageServerConstants.PageURLs.ErrorPage);   
            }
        }

        public static void ThrowException(AuthorizationException e)
        {
                context = HttpContext.Current;

                context.Server.ClearError();

                if (e.ErrorMessage != null && !e.ErrorMessage.Equals(string.Empty))
                    context.Items.Add(ImageServerConstants.ContextKeys.ErrorMessage, e.ErrorMessage);
                if (e.ErrorDescription != null && !e.ErrorDescription.Equals(string.Empty))
                    context.Items.Add(ImageServerConstants.ContextKeys.ErrorDescription, e.ErrorDescription);

                context.Server.Transfer(ImageServerConstants.PageURLs.AuthorizationErrorPage);
        }


        public static string ThrowAJAXException(Exception e)
        {
            Exception baseException = e.GetBaseException();

            string message = baseException.Message;
            string source = baseException.Source;
            string stackTrace = baseException.StackTrace;

            if (e.Data["ExtraInfo"] != null)
            {
                message += "\nExtra Info: " + e.Data["ExtraInfo"].ToString();
            }
            else
            {
                message += "\nExtra Info: " + "An unspecified error occurred.";
            }

            string logMessage = string.Format("Message: {0}\nSource:{1}\nStack Trace:{2}", message, source, stackTrace);
            Platform.Log(LogLevel.Error, logMessage);

            return logMessage;
        }
    }
}
