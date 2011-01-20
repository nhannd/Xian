#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Web.Script.Services;
using System.Web.Services;
using System.ComponentModel;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Web.Common.Utilities;

namespace ClearCanvas.ImageServer.Web.Application.Services
{
    //// <summary>
    /// Summary description for FilesystemInfoService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class FilesystemInfoService : System.Web.Services.WebService
    {

        [WebMethod]
        public FilesystemInfo GetFilesystemInfo(string path)
        {
            return ServerUtility.GetFilesystemInfo(path);
        }
    }
}
