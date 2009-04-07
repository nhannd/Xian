using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Common.Authentication
{
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
            using(LoginService service = new LoginService())
            {
                service.Validate(this);
                _valid = true;
            }
            
        }
    }
}