using System.Net;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers;
using ClearCanvas.Dicom;
using System.IO;
using System.Globalization;
using ClearCanvas.Common.Utilities;
using System.Diagnostics;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.MimeTypes
{
	[ExtensionOf(typeof(ImageMimeTypeProcessorExtensionPoint))]
	public class ImageHeaderMimeType : IImageMimeTypeProcessor
	{
		#region IImageMimeTypeProcessor Members

		public string OutputMimeType
		{
			get { return "application/clearcanvas-header"; }
		}

		public MimeTypeProcessorOutput Process(ImageStreamingContext context)
		{
			uint stopTag;
			if (!uint.TryParse(context.Request.QueryString["stopTag"] ?? "", NumberStyles.HexNumber, null, out stopTag))
				stopTag = DicomTags.PixelData;

			if (stopTag > DicomTags.PixelData)
				throw new WADOException(HttpStatusCode.BadRequest,
										"Stop tag must be less than PixelData tag.");

			MimeTypeProcessorOutput output = new MimeTypeProcessorOutput();
			output.ContentType = OutputMimeType;
			DicomFile file = new DicomFile(context.ImagePath);
			file.Load(stopTag, DicomReadOptions.Default);

			output.ContentType = OutputMimeType;

			MemoryStream memStream = new MemoryStream();
			file.Save(memStream, DicomWriteOptions.Default);
			output.Output = memStream.ToArray();

			return output;
		}

		#endregion
	}
}
