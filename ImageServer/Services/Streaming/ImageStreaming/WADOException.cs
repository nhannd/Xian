#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Net;
using System.Web;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{

    /// <summary>
    /// Represents an exception that is caused by violation of Dicom wado specifications or failure to process the wado requests.
    /// </summary>
    public class WADOException : HttpException
    {
        public WADOException(HttpStatusCode code, string message)
            : base((int)code, message)
        {
        }
    }
}
