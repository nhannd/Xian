#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Security.Principal;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.Web.Enterprise.Authentication
{
    /// <summary>
    /// Custom principal
    /// </summary>
    public class CustomPrincipal : IPrincipal, IUserCredentialsProvider
    {
        private IIdentity _identity;
        private LoginCredentials _credentials;

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
            set { _credentials = value; }
        }

        public bool IsInRole(string role)
        {
            // check that the user was granted this token
            return CollectionUtils.Contains(_credentials.Authorities,
                                            delegate(string token) { return token == role; });
        }

        #endregion

        public string DisplayName
        {
            get { return _credentials.DisplayName; }
        }

        #region IUserCredentialsProvider

        public string UserName
        {
            get { return _credentials.UserName; }
        }

        public string SessionTokenId
        {
            get { return _credentials.SessionToken.Id; }
        }

        #endregion
    }
}