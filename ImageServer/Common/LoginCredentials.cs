using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
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

        public CustomPrincipal(IIdentity identity, LoginCredentials credentials)
        {
            _identity = identity;
            _credentials = credentials;
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
            // check that the user was granted this token
            return CollectionUtils.Contains(_credentials.Authorities,
                delegate(string token) { return token == role; });
        }

        #endregion
    }

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

    public class SessionInfo
    {
        private CustomPrincipal _user;
        private bool _valid = false;
        
        public SessionInfo(CustomPrincipal user)
        {
            _user = user;

            Validate(); // this would refresh the authority groups
        }

        public SessionInfo(string loginId, string name, SessionToken token)
            : this(new CustomPrincipal(new CustomIdentity(loginId, name),
                CreateLoginCredentials(loginId, name, token)))
        {
            
        }

        /// <summary>
        /// Gets a value indicating whether or not the session information is valid.
        /// </summary>
        public bool Valid
        {
            get
            {
                return _valid;
            }
        }

        public CustomPrincipal User
        {
            get { return _user; }
        }

        public LoginCredentials Credentials
        {
            get { return _user.Credentials; }
        }

        private static LoginCredentials CreateLoginCredentials(string loginId, string name, SessionToken token)

        {
            LoginCredentials credentials = new LoginCredentials();
            credentials.UserName = loginId;
            credentials.DisplayName = name;
            credentials.SessionToken = token;
            return credentials;
        }

        private void Validate()
        {
            Platform.GetService<ILoginService>(
                delegate(ILoginService service)
                    {
                        service.Validate(this);
                        _valid = true;
                    });
            
        }
    }

}
