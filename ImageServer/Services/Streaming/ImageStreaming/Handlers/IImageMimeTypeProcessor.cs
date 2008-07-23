    using System.IO;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    public interface IImageMimeTypeProcessor
    {
        string OutputMimeType { get; }

        MimeTypeProcessorOutput Process(ImageStreamingContext context);
    }

}