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
using System.Threading;
using System.Web;
using System.Web.Security;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Enterprise.Authentication;
using ClearCanvas.ImageServer.Web.Common.Exceptions;

namespace ClearCanvas.ImageServer.Web.Common.Security
{
    class CustomFormAuthenticationModule : IHttpModule
    {

        #region IHttpModule Members

        private HttpApplication _context;
        public void Dispose()
        {
            _context.AuthenticateRequest -= AuthorizeRequest;
            
        }

        public void Init(HttpApplication context)
        {
            _context = context;
            context.AuthorizeRequest += AuthorizeRequest;
            
        }

        static void AuthorizeRequest(object sender, EventArgs e)
        {
            try
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated && HttpContext.Current.User.Identity is FormsIdentity)
                {
                    // Note: If user signed out in another window, the ticket would have been 
                    // removed from the browser and this code shoudn't be executed.
                    
                    // resemble the SessionInfo from the ticket.
                    FormsIdentity loginId = (FormsIdentity) HttpContext.Current.User.Identity ;
                    FormsAuthenticationTicket ticket = loginId.Ticket;

                    String[] fields = ticket.UserData.Split('|');
                    String tokenId = fields[0];
                    String userDisplayName = fields[1];
                    SessionToken token = new SessionToken(tokenId, ticket.Expiration);
                    SessionInfo  session = new SessionInfo(loginId.Name, userDisplayName, token);

                    // Initialize the session. This will throw exception if the session is no longer
                    // valid. For eg, time-out.
                    SessionManager.InitializeSession(session);
                }

                if (String.IsNullOrEmpty(Thread.CurrentThread.Name))
                {
                    String user = SessionManager.Current != null ? SessionManager.Current.User.Identity.Name : "Unknown";

                    Thread.CurrentThread.Name =
                        String.Format(SR.WebGUILogHeader, 
                            HttpContext.Current.Request.UserHostAddress,
                            HttpContext.Current.Request.Browser.Browser,
                            HttpContext.Current.Request.Browser.Version,
                            user);
                }
                
            }
            catch (SessionValidationException)
            {
                // redirect to login screen
                String error = String.Format("The current session is no longer valid.");
                SessionManager.TerminateSession(error);
            }
            catch(Exception ex)
            {
                // log the exception
                ExceptionHandler.ThrowException(ex);
            }
            
           
        }

        #endregion
    }
}
