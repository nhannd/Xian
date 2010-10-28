#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Net;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    /// <summary>
    /// Defines the interface of handler for different WADO request types.
    /// </summary>
    /// <remarks>
    /// Dicom standard says "WADO" is the only valid request type value currently defined, but additional ones may be added in the future.
    /// </remarks>
    interface IWADORequestTypeHandler : IDisposable
    {
        /// <summary>
        /// Gets the request type that can be handled by the handler.
        /// </summary>
        string RequestType { get;}

        /// <summary>
        /// Processes a WADO request.
        /// </summary>
        /// <param name="context"></param>
        WADOResponse Process(WADORequestTypeHandlerContext context);
    }

    class WADORequestTypeHandlerContext
    {
        public String ServerAE;
        public HttpListenerContext HttpContext;
    }

    /// <summary>
    /// Extension point to allow adding plugins for handling requests with different RequestType parameter
    /// </summary>
    [ExtensionPoint()]
    class WADORequestTypeExtensionPoint : ExtensionPoint<IWADORequestTypeHandler>
    {
    }
}
