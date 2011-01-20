#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Common.Security
{
    public static class SessionManager
    {
        #region Private Fields
        private static TimeSpan _sessionTimeOut;
        #endregion

        static SessionManager()
        {
            Platform.Log(LogLevel.Info, " Initilizing SessionManager");
        }

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
            // this should throw exception if the session is no longer valid. It also loads the authority tokens}
            if (!session.Valid)
            {
                throw new Exception("This session is no longer valid");
            }
            else
            {
                Current = session;

                string loginId = session.User.Identity.Name;
                CustomIdentity identity = session.User.Identity as CustomIdentity;
                Platform.CheckForNullReference(identity, "identity");
                
                string displayName = identity.DisplayName;
                SessionToken token = session.Credentials.SessionToken;
                string[] authorities = session.Credentials.Authorities;

                String data = String.Format("{0}|{1}|{2}", token.Id, displayName, authorities);

                // the expiry time is determined by the authentication service
                DateTime expiryTime = token.ExpiryTime; 
                
                FormsAuthenticationTicket authTicket = new
                    FormsAuthenticationTicket(1,  // version
                                              loginId,         // user name
                                              Platform.Time,   // creation
                                              expiryTime,      // Expiration
                                              false,           // Persistent
                                              data);           // User data

                // Now encrypt the ticket.
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                // Create a cookie with the encrypted data
                HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                //Create an unencrypted cookie that contains the userid and the expiry time so the browser
                //can check for session timeout.
                HttpCookie expiryCookie = new HttpCookie(GetExpiryTimeCookieName(session), DateTimeFormatter.Format(expiryTime.ToUniversalTime(), ImageServerConstants.CookieDateTimeFormat));
                
                HttpContext.Current.Response.Cookies.Add(authCookie);
                HttpContext.Current.Response.Cookies.Add(expiryCookie);
                
                SessionTimeout = expiryTime.ToUniversalTime() - Platform.Time.ToUniversalTime();
            }
        }

        /// <summary>
        /// Logs in and intializes the session using the given username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static SessionInfo InitializeSession(string username, string password)
        {
            return InitializeSession(username, password, ImageServerConstants.DefaultApplicationName, true);
        }

        /// <summary>
        /// Logs in and intializes the session using the given username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static SessionInfo InitializeSession(string username, string password, string appName)
        {
                return InitializeSession(username, password, ImageServerConstants.DefaultApplicationName, true);
        }

        /// <summary>
        /// Logs in and intializes the session using the given username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static SessionInfo InitializeSession(string username, string password, string appName, bool redirect)
        {
            using (LoginService service = new LoginService())
            {
                SessionInfo session = service.Login(username, password, appName);
                InitializeSession(session);
                Platform.Log(LogLevel.Info, "[{0}]: {1} has successfully logged in.", appName, username);

                if(redirect) HttpContext.Current.Response.Redirect(FormsAuthentication.GetRedirectUrl(username, false), false);
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
        /// Signs out the current user without redirection to the login screen.
        /// </summary>
        public static void SignOut()
        {
            SignOut(Current);
        }

        public static void SignOut(SessionInfo session)
        {

            FormsAuthentication.SignOut();
            
            if (session != null)
            {
                try
                {
                    ForceOtherPagesToLogout(session);

                    using (LoginService service = new LoginService())
                    {
                        service.Logout(session.Credentials.SessionToken.Id);
                    }
                }
                catch (NotSupportedException)
                {
                    //ignore this.
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Warn, e, "Failed to log user out.");
                }

                UserAuthenticationAuditHelper audit = new UserAuthenticationAuditHelper(
                    ServerPlatform.AuditSource,
                    EventIdentificationTypeEventOutcomeIndicator.Success,
                    UserAuthenticationEventType.Logout);
                audit.AddUserParticipant(new AuditPersonActiveParticipant(
                                             session.Credentials.UserName,
                                             null,
                                             session.Credentials.DisplayName));
                ServerPlatform.LogAuditMessage(audit);
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

        public static string GetExpiryTimeCookieName()
        {
            return GetExpiryTimeCookieName(Current);
        }
        public static string GetExpiryTimeCookieName(SessionInfo session)
        {
            if (session == null)
                return null;

            string loginId = session.User.Identity.Name;
            return "ImageServer." + loginId;
        }

        private static void ForceOtherPagesToLogout(SessionInfo session)
        {
            //NOTE: SessionTimeout.ascx must be updated if this method  is modified.

			// Ideally we want to remove the expiry time cookie on the client.
			// However, this is not possible. We can only update the cookie.
			// By removing the expiry time value in the cookie, we are implicity 
			// other pages to redirect to the login page instead. (see SessionTimeout.ascx)
            HttpCookie expiryCookie = new HttpCookie(GetExpiryTimeCookieName(session))
            {
                Expires = Platform.Time.AddMinutes(5),
                Value = string.Empty
            };

            HttpContext.Current.Response.Cookies.Set(expiryCookie);
        }

    }
}