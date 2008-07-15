using System;
using System.IO;
using System.Net;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    /// <summary>
    /// Represents a handler that can stream dicom images to web clients.
    /// </summary>
    internal class ImageStreamingHandler : IObjectStreamingHandler
    {

        public WADOResponse Process(HttpListenerContext httpContext)
        {
            ImageStreamingContext context = new ImageStreamingContext();
            context.Request = httpContext.Request;
            context.Response = httpContext.Response;
            context.StudyInstanceUid = httpContext.Request.QueryString["studyuid"];
            context.SeriesInstanceUid = httpContext.Request.QueryString["seriesuid"];
            context.ObjectUid = httpContext.Request.QueryString["objectuid"];
            context.StorageLocation = StudyStorageUtility.GetStudyStorageLocation(httpContext.Request);
            
            
            // convert the dicom image into the appropriate mime type
            WADOResponse response = new WADOResponse();
            IImageMimeTypeConverter converter = GetConverter(context);

            ImageConverterOutput output = converter.Convert(context);   
            response.Stream = output.Stream;
            response.ContentType = output.ContentType;
            response.HasMoreFrame = output.HasMoreFrame;
            
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

        protected virtual IImageMimeTypeConverter GetConverter(ImageStreamingContext context)
        {
            string responseContentType = HttpUtility.HtmlDecode(context.Request.QueryString["contentType"]);
            if (String.IsNullOrEmpty(responseContentType))
            {
                if (context.IsMultiFrame)
                    responseContentType = "application/dicom";
                else
                    responseContentType = "image/jpeg";
            }

            
            ImageMimeTypeConverterExtensionPoint xp = new ImageMimeTypeConverterExtensionPoint();
            object[] plugins = xp.CreateExtensions();

            bool found = false;
            foreach (IImageMimeTypeConverter mimeTypeConverter in plugins)
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
                throw new WADOException((int)HttpStatusCode.NotAcceptable,
                                    String.Format("{0} is supported but is not acceptable by the client", responseContentType));
            }
            else
            {
                throw new WADOException((int)HttpStatusCode.BadRequest,
                                    String.Format("The specified contentType '{0}' is not supported", responseContentType));
            }
        }
    }
}