#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Enterprise.Authentication
{

    public class SessionInfo
    {
        private readonly CustomPrincipal _user;
        private bool _valid;

        public SessionInfo(CustomPrincipal user)
        {
            _user = user;
        }

        public SessionInfo(string loginId, string name, SessionToken token)
            : this(new CustomPrincipal(new CustomIdentity(loginId, name),
                                       CreateLoginCredentials(loginId, name, token)))
        {

        }

        /// <summary>
        /// Gets a value indicating whether or not the session information is valid.
        /// </summary>
        /// <remarks>
        /// Exception will be thrown if session cannot be validated in the process.
        /// </remarks>
        public bool Valid
        {
            get
            {
                Validate();
                return _valid;
            }
        }

        public CustomPrincipal User
        {
            get { return _user; }
        }

        public LoginCredentials Credentials
        {
            get
            {
                return _user.Credentials;
            }
        }

        private static LoginCredentials CreateLoginCredentials(string loginId, string name, SessionToken token)
        {
            LoginCredentials credentials = new LoginCredentials();
            credentials.UserName = loginId;
            credentials.DisplayName = name;
            credentials.SessionToken = token;
            return credentials;
        }

        public void Validate()
        {
            _valid = false;

            using(LoginService service = new LoginService())
            {
                SessionInfo sessionInfo = service.Query(this.Credentials.SessionToken.Id);

                if (sessionInfo == null)
                {
                    throw new SessionValidationException();
                }

                _user.Credentials = sessionInfo.Credentials;
                SessionToken newToken = service.Renew(this.Credentials.SessionToken.Id);
                _user.Credentials.SessionToken = newToken;
                _valid = true;
            }   
        }

    }
}