#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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