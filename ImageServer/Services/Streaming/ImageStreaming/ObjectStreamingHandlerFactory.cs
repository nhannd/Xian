using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using ClearCanvas.ImageServer.Model;
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
        /// <param name="context"></param>
        /// <returns></returns>
        public IObjectStreamingHandler CreateHandler(HttpListenerRequest request)
        {
            // TODO: other type of objects (such as text, report) may be supported in the future
            return new ImageStreamingHandler();
        }
    }
}