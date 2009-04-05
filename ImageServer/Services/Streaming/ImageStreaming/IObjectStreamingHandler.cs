using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    /// <summary>
    /// Defines the interface of a handler that can process WADO requests.
    /// </summary>
    interface IObjectStreamingHandler
    {
        WADOResponse Process(WADORequestTypeHandlerContext context);
    }

    
}
