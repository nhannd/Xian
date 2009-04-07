using System.Threading;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Enterprise
{
    /// <summary>
    /// User credentials
    /// </summary>
    public class LoginCredentials
    {
        public string UserName;
        public string DisplayName;
        public SessionToken SessionToken;
        public string[] Authorities;

        /// <summary>
        /// Gets the credentials for the current user
        /// </summary>
        public static LoginCredentials Current
        {
            get
            {
                if (Thread.CurrentPrincipal is CustomPrincipal)
                {
                    CustomPrincipal p = Thread.CurrentPrincipal as CustomPrincipal;
                    return p.Credentials;

                }
                else
                {
                    return null;
                }
            }
            set
            {
                Thread.CurrentPrincipal = new CustomPrincipal(
                    new CustomIdentity(value.UserName, value.DisplayName), value);
            }
        }
    }
}