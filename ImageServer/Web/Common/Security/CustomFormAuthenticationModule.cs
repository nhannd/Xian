using System;
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
                    FormsIdentity loginId = (FormsIdentity) HttpContext.Current.User.Identity ;
                    FormsAuthenticationTicket ticket = loginId.Ticket;

                    String[] fields = ticket.UserData.Split('|');
                    String tokenId = fields[0];
                    String userDisplayName = fields[1];
                    SessionToken token = new SessionToken(tokenId, ticket.Expiration);

                    SessionInfo  session = new SessionInfo(loginId.Name, userDisplayName, token);
                    SessionManager.InitializeSession(session);
                }
                
            }
            catch (Exception ex)
            {
                // log the exception
                ExceptionHandler.ThrowException(ex);
            }
            
           
        }

        #endregion
    }
}
