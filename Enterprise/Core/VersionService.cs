#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common.ServerVersion;

namespace ClearCanvas.Enterprise.Core
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IVersionService))]
    public class VersionService : CoreServiceLayer, IVersionService
    {
        public GetVersionResponse GetVersion(GetVersionRequest request)
        {
            var response = new GetVersionResponse()
                               {
                                   Component = ProductInformation.Component,
                                   Edition = ProductInformation.Edition,
                                   VersionMajor = ProductInformation.Version.Major,
                                   VersionMinor = ProductInformation.Version.Minor,
                                   VersionBuild = ProductInformation.Version.Build,
                                   VersionRevision  = ProductInformation.Version.Revision
                               };

            return response;
        }
    }
}
