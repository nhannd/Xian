using System;
using System.IO;
using System.Net;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    /// <summary>
    /// Represents a handler that can stream dicom images to web clients.
    /// </summary>
    internal class ImageStreamingHandler : IObjectStreamingHandler
    {

        public WADOResponse Process(string serverAE, HttpListenerContext httpContext)
        {
            Platform.CheckForNullReference(serverAE, "serverAE");
            Platform.CheckForNullReference(httpContext, "httpContext");
            
            ServerPartitionMonitor partitionMonitor = ServerPartitionMonitor.Instance;
            partitionMonitor.LoadPartitions();
            ServerPartition partition = partitionMonitor.GetPartition(serverAE);
            if (partition== null)
                throw new WADOException(HttpStatusCode.NotFound, String.Format("Server {0} does not exist", serverAE));

            if (!partition.Enabled)
            {
                throw new WADOException(HttpStatusCode.Forbidden, String.Format("Server {0} has been disabled", serverAE));
            }
            
            ImageStreamingContext context = new ImageStreamingContext();
            context.ServerAE = serverAE;
            context.Request = httpContext.Request;
            context.Response = httpContext.Response;
            context.StudyInstanceUid = httpContext.Request.QueryString["studyuid"];
            context.SeriesInstanceUid = httpContext.Request.QueryString["seriesuid"];
            context.ObjectUid = httpContext.Request.QueryString["objectuid"];
            context.StorageLocation = StudyStorageUtility.GetStudyStorageLocation(context.ServerAE, httpContext.Request);

            if (!File.Exists(context.ImagePath))
            {
                throw new WADOException(HttpStatusCode.NotFound, "The requested object does not exist on the specified server");
            }

            FilesystemMonitor monitor = new FilesystemMonitor("ImageStreaming");
            monitor.Load();
            ServerFilesystemInfo fs = monitor.GetFilesystemInfo(context.StorageLocation.FilesystemKey);
            if (!fs.Readable)
            {
                throw new WADOException(HttpStatusCode.Forbidden, "The requested object is not located on readable filesystem");
            }

            if (context.StorageLocation.Lock)
            {
                throw new WADOException(HttpStatusCode.Forbidden, "The requested object is being used by another process. Please try again later.");
            }
                        
            
            // convert the dicom image into the appropriate mime type
            WADOResponse response = new WADOResponse();
            IImageMimeTypeProcessor processor = GetMimeTypeProcessor(context);

            MimeTypeProcessorOutput output = processor.Process(context);   
            response.Output = output.Output;
            response.ContentType = output.ContentType;
            response.IsLast = output.IsLast;
            
            return response;

        }


        protected bool ClientAcceptable(ImageStreamingContext context, string contentType)
        {
            if (context.Request.AcceptTypes == null)
                return false;
            
            foreach(string rawmime in context.Request.AcceptTypes)
            {
                string mime = rawmime;
                if (rawmime.Contains(";"))
                    mime = rawmime.Substring(0, rawmime.IndexOf(";"));

                if (mime=="*/*" || mime==contentType)
                    return true;
            }

            return false;
        }

        protected virtual IImageMimeTypeProcessor GetMimeTypeProcessor(ImageStreamingContext context)
        {
            string responseContentType = HttpUtility.HtmlDecode(context.Request.QueryString["contentType"]);
            if (String.IsNullOrEmpty(responseContentType))
            {
                if (context.IsMultiFrame)
                    responseContentType = "application/dicom";
                else
                {
                    responseContentType = "image/jpeg";
                    if (!ClientAcceptable(context, responseContentType))
                    {
                        responseContentType = "application/dicom";
                    }
                }
            }

            
            ImageMimeTypeProcessorExtensionPoint xp = new ImageMimeTypeProcessorExtensionPoint();
            object[] plugins = xp.CreateExtensions();

            bool found = false;
            foreach (IImageMimeTypeProcessor mimeTypeConverter in plugins)
            {

                if (mimeTypeConverter.OutputMimeType == responseContentType)
                {
                    found = true;

                    if (ClientAcceptable(context, mimeTypeConverter.OutputMimeType))
                        return mimeTypeConverter;
                }

            }

            if (found)
            {
                throw new WADOException(HttpStatusCode.NotAcceptable,
                                    String.Format("{0} is supported but is not acceptable by the client", responseContentType));
            }
            else
            {
                throw new WADOException(HttpStatusCode.BadRequest,
                                    String.Format("The specified contentType '{0}' is not supported", responseContentType));
            }
        }

    }
}