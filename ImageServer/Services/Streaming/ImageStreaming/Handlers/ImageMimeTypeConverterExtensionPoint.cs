using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    [ExtensionPoint]
    class ImageMimeTypeProcessorExtensionPoint : ExtensionPoint<IImageMimeTypeProcessor>
    {
    }
}