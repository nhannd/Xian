#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.Server.ShredHost;

namespace ClearCanvas.ImageServer.Web.Services.Shreds.Management
{
    [ServiceContract]
    public interface IFilesystemService
    {
        [OperationContract]
        FilesystemInfo GetFilesystemInfo(string path);
    }

    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [ExtensionOf(typeof(ShredExtensionPoint))]
    public class FilesystemService : WcfShred, IFilesystemService
    {

        #region IFilesystemService Members

        public FilesystemInfo GetFilesystemInfo(string path)
        {
            return FilesystemUtils.GetDirectoryInfo(path);
        }

        #endregion

        #region WcfShred override
        public override void Start()
        {
            try
            {
                ServiceEndpointDescription sed = StartHttpHost<FilesystemService, IFilesystemService>("FilesystemService", SR.FilesystemServiceDisplayDescription);

            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, "Failed to start {0} : {1}", SR.FilesystemServiceDisplayName, e.StackTrace);
                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Error, SR.FilesystemServiceDisplayName,
                                     AlertTypeCodes.UnableToStart, null, TimeSpan.Zero, SR.AlertFilesystemUnableToStart, e.Message);
            }
        }

        public override void Stop()
        {
            StopHost("FilesystemService");
        }

        public override string GetDisplayName()
        {
            return SR.FilesystemServiceDisplayName;
        }

        public override string GetDescription()
        {
            return SR.FilesystemServiceDisplayDescription;
        }

        #endregion WcfShred override
    }

}

