#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common.ServiceModel;
using ClearCanvas.ImageServer.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Common.Utilities
{
    public static class ServerUtility
    {
        /// <summary>
        /// Retrieves the <see cref="FilesystemInfo"/> for a specified path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static FilesystemInfo GetFilesystemInfo(string path)
        {
            FilesystemInfo fsInfo = null;
            Platform.GetService(delegate(IFilesystemService service)
            {
                fsInfo = service.GetFilesystemInfo(path);
            });

            return fsInfo;
        }
    }
}
