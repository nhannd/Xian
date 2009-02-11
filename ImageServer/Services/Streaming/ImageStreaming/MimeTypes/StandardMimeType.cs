using System.IO;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.MimeTypes
{
    [ExtensionOf(typeof(ImageMimeTypeProcessorExtensionPoint))]
    public class StandardMimeType : IImageMimeTypeProcessor
    {
        public string OutputMimeType
        {
            get { return "application/dicom"; }
        }

        public MimeTypeProcessorOutput Process(ImageStreamingContext context)
        {
            
            if (context.Request.QueryString["frameNumber"] != null)
            {
                // "Shall be ignored in the case of all objects other than multi-frame objects...
                // shall not be present if content type is application/dicom"
                throw new WADOException(HttpStatusCode.BadRequest,
                                        "FrameNumber shall not be present when application/dicom mime type is used. Please use a different ContentType.");
                
            }
            else
            {

                MimeTypeProcessorOutput output = new MimeTypeProcessorOutput();
                output.ContentType = OutputMimeType;
                using (FileStream stream = FileStreamOpener.OpenForRead(context.ImagePath, FileMode.Open))
                {
                    output.ContentType = OutputMimeType;
                    byte[] buffer = new byte[stream.Length];
                    int offset = 0;
                    int readBytes = 0;
                    do
                    {
                        readBytes = stream.Read(buffer, offset, buffer.Length - offset);
                        if (readBytes > 0)
                        {
                            offset += readBytes;
                        }
                    } while (readBytes > 0);
                    output.Output = buffer;
					stream.Close();
                }
                return output;    
            }
            
        }
    }

    [ExtensionOf(typeof(ImageMimeTypeProcessorExtensionPoint))]
    public class JPEGMimeType : IImageMimeTypeProcessor
    {
        public string OutputMimeType
        {
            get { return "image/jpeg"; }
        }

        public MimeTypeProcessorOutput Process(ImageStreamingContext context)
        {
            throw new WADOException( HttpStatusCode.NotImplemented, "image/jpeg is not supported. Please use a different ContentType");
        }
    }
}