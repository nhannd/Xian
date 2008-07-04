using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{

    /// <summary>
    /// Defines the interface of handler for different WADO request types.
    /// </summary>
    public interface IWADORequestTypeHandler : IDisposable
    {
        /// <summary>
        /// Gets the request type that can be handled by the handler.
        /// </summary>
        string RequestType { get;}

        /// <summary>
        /// Processes a WADO request.
        /// </summary>
        /// <param name="request"></param>
        WADOResponse Process(HttpListenerRequest request);
    }

    
}
