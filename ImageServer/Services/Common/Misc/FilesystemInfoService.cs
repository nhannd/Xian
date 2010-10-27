#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.ServiceModel;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;

namespace ClearCanvas.ImageServer.Services.Common.Misc
{
    [ServiceImplementsContract(typeof(IFilesystemService))]
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class FilesystemInfoService : IApplicationServiceLayer, IFilesystemService
    {
        #region IFilesystemService Members

        public FilesystemInfo GetFilesystemInfo(string path)
        {
            return FilesystemUtils.GetDirectoryInfo(path);
        }

        #endregion
    }
}