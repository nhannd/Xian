using System.Security.Principal;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Custom principal
    /// </summary>
    public class CustomPrincipal : IPrincipal
    {
        private IIdentity _identity;
        private readonly LoginCredentials _credentials;

        public CustomPrincipal(IIdentity identity, LoginCredentials credentials)
        {
            _identity = identity;
            _credentials = credentials;
        }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return _identity; }
            set { _identity = value; }
        }
        public LoginCredentials Credentials
        {
            get { return _credentials; }
        }

        public bool IsInRole(string role)
        {
            // check that the user was granted this token
            return CollectionUtils.Contains(_credentials.Authorities,
                                            delegate(string token) { return token == role; });
        }

        #endregion
    }
}