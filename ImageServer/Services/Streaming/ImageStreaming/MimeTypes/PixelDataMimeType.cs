using System;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.MimeTypes
{
    /// <summary>
    /// Generates pixel data for an image streaming response.
    /// </summary>
    [ExtensionOf(typeof(ImageMimeTypeProcessorExtensionPoint))]
    class PixelDataMimeTypeProcessor : IImageMimeTypeProcessor
    {

        #region IImageMimeTypeProcessor Members


        public string OutputMimeType
        {
            get { return "application/clearcanvas"; }
        }

        public MimeTypeProcessorOutput Process(ImageStreamingContext context)
        {
            Platform.CheckForNullReference(context, "context");

            DicomPixelData pd = context.PixelData;

            MimeTypeProcessorOutput output = new MimeTypeProcessorOutput();

            if (context.Request.QueryString["frameNumber"] == null)
            {
                output.ContentType = OutputMimeType;

                // TODO: we may want to re-use last loader (for multi-frame) or instantiate additional loaders
                // in anticipation of subsequent image requests.
                PixelDataLoader loader = new PixelDataLoader(context);
                output.Output = loader.ReadFrame(0);
                output.IsLast = (pd.NumberOfFrames == 1);
            }
            else
            {
                int frame = int.Parse(context.Request.QueryString["frameNumber"]);

                if (frame < 0 || frame >= context.PixelData.NumberOfFrames)
                {
                    throw new WADOException(HttpStatusCode.BadRequest, String.Format("Requested FrameNumber {0} is in invalid range", frame));
                }

                output.ContentType = OutputMimeType;

                // TODO: we may want to re-use last loader (for multi-frame) or instantiate additional loaders
                // in anticipation of subsequent image requests.
                PixelDataLoader loader = new PixelDataLoader(context);
                output.Output = loader.ReadFrame(frame);
                output.IsLast = (pd.NumberOfFrames == frame + 1);
            }


            // note: the transfer syntax of the returned pixel data may not be the same as that in the original image.
            // In the future, the clients may specify different transfer syntaxes which may mean the compressed image must be decompressed or vice versa. 
            TransferSyntax transferSyntax = context.PixelData.TransferSyntax;
            output.IsCompressed = transferSyntax.LosslessCompressed || transferSyntax.LossyCompressed;


            #region Special Code

            // Note: this block of code inject special header fields to assist the clients handling the images
            // For eg, the

            if (output.IsLast)
                context.Response.Headers.Add("IsLast", "true");

            if (output.IsCompressed)
            {
                // Fields that can be used by the web clients to decompress the compressed images streamed by the server.

                context.Response.Headers.Add("Compressed", "true");
                context.Response.Headers.Add("TransferSyntaxUid", pd.TransferSyntax.UidString);

                context.Response.Headers.Add("BitsAllocated", pd.BitsAllocated.ToString());
                context.Response.Headers.Add("BitsStored", pd.BitsStored.ToString());
                context.Response.Headers.Add("DerivationDescription", pd.DerivationDescription);

                context.Response.Headers.Add("HighBit", pd.HighBit.ToString());
                context.Response.Headers.Add("ImageHeight", pd.ImageHeight.ToString());
                context.Response.Headers.Add("ImageWidth", pd.ImageWidth.ToString());
                context.Response.Headers.Add("LossyImageCompression", pd.LossyImageCompression);
                context.Response.Headers.Add("LossyImageCompressionMethod", pd.LossyImageCompressionMethod);
                context.Response.Headers.Add("LossyImageCompressionRatio", pd.LossyImageCompressionRatio.ToString());
                context.Response.Headers.Add("NumberOfFrames", pd.NumberOfFrames.ToString());
                context.Response.Headers.Add("PhotometricInterpretation", pd.PhotometricInterpretation);
                context.Response.Headers.Add("PixelRepresentation", pd.PixelRepresentation.ToString());
                context.Response.Headers.Add("PlanarConfiguration", pd.PlanarConfiguration.ToString());
                context.Response.Headers.Add("SamplesPerPixel", pd.SamplesPerPixel.ToString());

            }

            #endregion

            Platform.Log(LogLevel.Debug, "Streaming {0} pixel data: {1} x {2} x {3} , {4} bits  [{5} KB] ({6})",
                         output.IsCompressed ? "compressed" : "uncompressed",
                         pd.ImageHeight,
                         pd.ImageWidth,
                         pd.SamplesPerPixel,
                         pd.BitsStored,
                         output.Output.Length/1024,
                         pd.TransferSyntax.Name);


            return output;
        }
        

        #endregion
    }
}