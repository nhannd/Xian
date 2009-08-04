using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Web.Common.Security
{
    /// <summary>
    /// Class implementing <see cref="IUserCredentialsProvider"/> to provide information about
    /// the current user in the web application.
    /// </summary>
    public class WebAppUserCredentialProvider : IUserCredentialsProvider
    {
        #region IUserCredentialsProvider Members

        public string UserName
        {
            get
            {
                return SessionManager.Current.User.Identity.Name;
            }
        }

        public string SessionTokenId
        {
            get
            {
                return SessionManager.Current.Credentials.SessionToken.Id;
            }
        }

        #endregion
    }
}