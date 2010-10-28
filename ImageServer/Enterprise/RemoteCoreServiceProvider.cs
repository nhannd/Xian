#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.ImageServer.Enterprise
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint), Enabled = true)]
    class RemoteServiceProvider : RemoteCoreServiceProvider
    {
        protected override string UserName
        {
            get
            {
                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                Platform.CheckForNullReference(principal, "principal");
                Platform.CheckMemberIsSet(principal.Credentials, "principal.Credentials");
                return principal.Credentials.UserName;
            }
        }

        protected override string Password
        {
            get
            {
                CustomPrincipal principal = Thread.CurrentPrincipal as CustomPrincipal;
                Platform.CheckForNullReference(principal, "principal");
                Platform.CheckMemberIsSet(principal.Credentials, "principal.Credentials");
                Platform.CheckMemberIsSet(principal.Credentials.SessionToken, "principal.Credentials.SessionToken");
                return principal.Credentials.SessionToken.Id;
            }
        }
    }
}