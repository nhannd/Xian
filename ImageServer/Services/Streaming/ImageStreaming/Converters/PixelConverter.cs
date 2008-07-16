using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Converters
{
    /// <summary>
    /// Generates pixel data for an image streaming response.
    /// </summary>
    [ExtensionOf(typeof(ImageMimeTypeConverterExtensionPoint))]
    public class CCPixelConverter : IImageMimeTypeConverter
    {
        public string OutputMimeType
        {
            get { return "application/clearcanvas"; }
        }

        public ImageConverterOutput Convert(ImageStreamingContext context)
        {
            ImageConverterOutput output = new ImageConverterOutput();
            
            if (context.Request.QueryString["frameNumber"] == null)
            {
                output.ContentType = OutputMimeType;
                output.Stream = new MemoryStream(context.PixelData.GetFrame(0));
                output.HasMoreFrame = (context.PixelData.NumberOfFrames > 1);
            
            }
            else
            {
                int frame = int.Parse(context.Request.QueryString["frameNumber"]);
                output.ContentType = OutputMimeType;
                byte[] buffer = context.PixelData.GetFrame(frame);
                output.Stream = new MemoryStream(buffer);
                output.HasMoreFrame = (context.PixelData.NumberOfFrames > frame + 1);
            }


            #region Special Code
            // Note: this block of code inject a special header field to assist the client during streaming of frames
            // Clients which has no aprior knowledge of the number of frames can use this field to load next frame.
            if (output.HasMoreFrame)
                context.Response.Headers.Add("HasMoreFrame", "true");
            #endregion
            

            return output;
        }
    }
}
