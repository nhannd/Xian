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
using ClearCanvas.Common;
using ClearCanvas.Dicom.Audit;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Enterprise.Authentication;

namespace ClearCanvas.ImageServer.Web.Common.Security
{
    public static class SessionManager
    {
        #region Private Fields
        private static TimeSpan _sessionTimeOut;
        #endregion

        /// <summary>
        /// Returns the current session information
        /// </summary>
        /// <remarks>
        /// The session information is set by calling <see cref="InitializeSession(SessionInfo)"/>. It is null 
        /// if the <see cref="InitializeSession(SessionInfo)"/> hasn't been called.
        /// </remarks>
        public static SessionInfo Current
        {
            get
            {
                if (Thread.CurrentPrincipal is CustomPrincipal)
                {
                    CustomPrincipal p = Thread.CurrentPrincipal as CustomPrincipal;
                    return new SessionInfo(p);
                   
                }
                else
                {
                    return null;
                }
            }
            set
            {
                Thread.CurrentPrincipal = value.User;
                HttpContext.Current.User = value.User;
               
            }
        
        }

        /// <summary>
        /// Sets up the principal for the thread and save the authentiction ticket.
        /// </summary>
        /// <param name="session"></param>
        public static void InitializeSession(SessionInfo session)
        {
            if (!session.Valid)
            {
                throw new SessionValidationException();
            }
            else
            {
                // this should throw exception if the session is no longer valid. It also loads the authority tokens}
                Current = session;

                string loginId = session.User.Identity.Name;
                CustomIdentity identity = session.User.Identity as CustomIdentity;
                Platform.CheckForNullReference(identity, "identity");
                
                string displayName = identity.DisplayName;
                SessionToken token = session.Credentials.SessionToken;
                string[] authorities = session.Credentials.Authorities;

                String data = String.Format("{0}|{1}|{2}", token.Id, displayName, authorities);
                FormsAuthenticationTicket authTicket = new
                    FormsAuthenticationTicket(1,  // version
                                              loginId,         // user name
                                              Platform.Time,    // creation
                                              token.ExpiryTime,// Expiration
                                              false,           // Persistent
                                              data);           // User data

                // Now encrypt the ticket.
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                // Create a cookie with the encrypted data
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                //Create an unencrypted cookie that contains the userid and the expiry time so the browser
                //can check for session timeout.
                //HttpCookie loginIdCookie = new HttpCookie("loginid", loginId);
                HttpCookie expiryCookie = new HttpCookie("ImageServer_" + loginId, token.ExpiryTime.ToUniversalTime().ToString());
                
                HttpContext.Current.Response.Cookies.Add(authCookie);
                HttpContext.Current.Response.Cookies.Add(expiryCookie);
                
                SessionTimeout = token.ExpiryTime - Platform.Time;
            }

        }

        /// <summary>
        /// Logs in and intializes the session using the given username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static SessionInfo InitializeSession(string username, string password)
        {
            using (LoginService service = new LoginService())
            {
                SessionInfo session = service.Login(username, password);
                InitializeSession(session);
                Platform.Log(LogLevel.Info, "{0} has successfully logged in.", username);                                
                
                HttpContext.Current.Response.Redirect(FormsAuthentication.GetRedirectUrl(username, false), false);
                return session;
            }
        }


        /// <summary>
        /// Terminates the current session and redirects users to the login page and displays the given message on the screen.
        /// </summary>
        public static void TerminateSession(string reason)
        {
            SignOut();// force to signout by removing the authentication ticket
            String queryString = String.Format("error={0}", reason);
            if (!String.IsNullOrEmpty(reason))
            {
                Platform.Log(LogLevel.Info, "Terminate session because {0}", reason);
            }
            FormsAuthentication.RedirectToLoginPage(queryString);
        }

        /// <summary>
        /// Signs out the current users without redirection to the login screen.
        /// </summary>
        public static void SignOut()
        {
            SessionInfo session = Current;
            
            FormsAuthentication.SignOut();
            
            if (session!=null)
            {
                UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(
                    ServerPlatform.AuditSource,
                    EventIdentificationTypeEventOutcomeIndicator.Success,
                    UserAuthenticationEventType.Logout);
                audit.AddUserParticipant(new AuditPersonActiveParticipant(
                                             session.Credentials.UserName,
                                             null,
                                             session.Credentials.DisplayName));
                ServerPlatform.LogAuditMessage("UserAuthentication", audit);
            }
            

        
        }


        /// <summary>
        /// Gets or sets the session time out in minutes.
        /// </summary>
        public static TimeSpan SessionTimeout
        {
            get
            {
                return _sessionTimeOut;
            }
            set
            {
                // no thread-safety check here, assuming it's almost the same even if it's changed.
                _sessionTimeOut = value;
            }
        }

        public static string LoginUrl
        {
            get
            {
                return FormsAuthentication.LoginUrl;
            }
        }
    }
}