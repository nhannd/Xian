#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Net;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    /// <summary>
    /// Represents a factory that creates handler for streaming objects from the server to web clients.
    /// </summary>
    class ObjectStreamingHandlerFactory
    {
        /// <summary>
        /// Instantiates a handler based on the specified context.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IObjectStreamingHandler CreateHandler(HttpListenerRequest request)
        {
            // TODO: other type of objects (such as text, report) may be supported in the future
            return new ImageStreamingHandler();
        }
    }
}