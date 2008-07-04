using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    public interface IWADOMimeTypeHandler
    {
        string[] SupportedRequestedMimeTypes { get; }
        WADOResponse GetResponseContent(HttpListenerRequest request);
    }

    [ExtensionPoint]
    public class WADOMimeTypeExtensionPoint : ExtensionPoint<IWADOMimeTypeHandler>
    {

    }
}
