using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Common.Admin.UserAdmin;
using ClearCanvas.Enterprise.Common.Authentication;
using ClearCanvas.ImageServer.Common.Services.Login;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Custom Identity
    /// </summary>
    public class CustomIdentity : GenericIdentity
    {
        private string _displayName;

        public CustomIdentity(string loginId, string displayName) : base(loginId)
        {
            _displayName = displayName;
        }

        /// <summary>
        /// Formatted name of the identity
        /// </summary>
        public String DisplayName
        {
            get { return _displayName; }
        }
    }

    /// <summary>
    /// Custom principal
    /// </summary>
    public class CustomPrincipal : IPrincipal
    {
        private IIdentity _identity;
        private readonly LoginCredentials _credentials;
        public CustomPrincipal(CustomIdentity identity, SessionToken token, String[] authorities)
        {
            _identity = identity;
            _credentials = new LoginCredentials();
            _credentials.SessionToken = token;
            _credentials.UserName = _identity.Name;
            _credentials.DisplayName = identity.DisplayName;
            _credentials.Authorities = authorities;
        }

        #region IPrincipal Members

        public IIdentity Identity
        {
            get { return _identity; }
            set { _identity = value;  }
        }
        public LoginCredentials Credentials
        {
            get { return _credentials; }
        }

        public bool IsInRole(string role)
        {
            //TODO: Refresh the credentials instead

            // check that the user was granted this token
            return CollectionUtils.Contains(_credentials.Authorities,
                delegate(string token) { return token == role; });
        }

        #endregion
    }
    public class LoginCredentials
    {
        public string UserName;
        public string DisplayName;
        public ClearCanvas.Enterprise.Common.SessionToken SessionToken;
        public string[] Authorities;

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
                    new CustomIdentity(value.UserName, value.DisplayName), value.SessionToken, value.Authorities);
            }
        }
        
    }

    public class SessionInfo
    {
        private CustomPrincipal _user;

        public SessionInfo(CustomPrincipal user)
        {
            _user = user;

        }

        public CustomPrincipal User
        {
            get { return _user; }
        }

        public LoginCredentials Credentials
        {
            get { return _user.Credentials; }
        }

        public void Validate()
        {
            Platform.GetService<ILoginService>(
                delegate(ILoginService service)
                    {
                        service.Validate(this);
                    });
            
        }
    }

}
