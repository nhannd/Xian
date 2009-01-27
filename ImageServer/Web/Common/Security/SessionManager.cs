using System;
using System.Threading;
using System.Web;
using System.Web.Security;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common;
namespace ClearCanvas.ImageServer.Web.Common.Security
{
    public static class SessionManager
    {
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
        /// Sets up the principal for the thread and save the authentiction ticket if there's none.
        /// </summary>
        /// <param name="session"></param>
        public static void InitializeSession(SessionInfo session)
        {
            session.Validate(); // this should throw exception if the session is no longer valid
            Current = session;

            HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie==null)
            {
                string loginId = session.User.Identity.Name;
                string displayName = (session.User.Identity as CustomIdentity).DisplayName;
                SessionToken token = session.Credentials.SessionToken;
                string[] authorities = session.Credentials.Authorities;

                String tokens = StringUtilities.Combine(authorities, ",");
                String data = String.Format("{0}|{1}|{2}", token.Id, displayName, authorities);
                FormsAuthenticationTicket authTicket = new
                    FormsAuthenticationTicket(1,  // version
                                 loginId,           // user name
                                 DateTime.Now,               // creation
                                 token.ExpiryTime,          // Expiration
                                 false,                      // Persistent
                                 data);                    // User data

                // Now encrypt the ticket.
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                // Create a cookie and add the encrypted ticket to the 
                // cookie as data.
                authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

                // one way to prevent cross-site scripting
                authCookie.HttpOnly = false;
                //authCookie.Secure = true; // over https only

                HttpContext.Current.Response.Cookies.Add(authCookie);

            }
       }
    }
}
