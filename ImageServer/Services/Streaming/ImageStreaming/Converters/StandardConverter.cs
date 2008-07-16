using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Converters
{
    [ExtensionOf(typeof(ImageMimeTypeConverterExtensionPoint))]
    public class StandardConverter : IImageMimeTypeConverter
    {
        public string OutputMimeType
        {
            get { return "application/dicom"; }
        }

        public ImageConverterOutput Convert(ImageStreamingContext context)
        {
            
            if (context.Request.QueryString["frameNumber"] != null)
            {
                // shall be ignored in the case of all objects other than multi-frame objects.
                // shall not be present if content type is application/dicom

                if (HttpUtility.HtmlDecode(context.Request.QueryString["contentType"]) == "application/dicom")
                    throw new WADOException((int)HttpStatusCode.BadRequest, "Requested object is a multi-frame image but using FrameNumber is not currrently supported");

                if (context.IsMultiFrame)
                {
                    ImageConverterOutput output = new ImageConverterOutput();
                    output.ContentType = OutputMimeType;
                    output.Stream = new FileStream(context.ImagePath, FileMode.Open, FileAccess.Read);
                    return output;    
                }
                else
                {
                    ImageConverterOutput output = new ImageConverterOutput();
                    output.ContentType = OutputMimeType;
                    output.Stream = new FileStream(context.ImagePath, FileMode.Open, FileAccess.Read);

                    return output;    
                       
                }
            }
            else
            {
            
                ImageConverterOutput output = new ImageConverterOutput();
                output.ContentType = OutputMimeType;
                output.Stream = new FileStream(context.ImagePath, FileMode.Open, FileAccess.Read);
         
                return output;    
            }
            
        }
    }

    [ExtensionOf(typeof(ImageMimeTypeConverterExtensionPoint))]
    public class JPEGConverter : IImageMimeTypeConverter
    {
        public string OutputMimeType
        {
            get { return "image/jpeg"; }
        }

        public ImageConverterOutput Convert(ImageStreamingContext context)
        {
            throw new WADOException( (int) HttpStatusCode.NotImplemented, "image/jpeg is not supported. Please use different ContentType");
        }
    }

}