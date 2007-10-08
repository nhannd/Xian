using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Codecs;
using ClearCanvas.Dicom;
using ClearCanvas.Common;
using System.Threading;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class ImageSopHelper
	{
		private static object _imageCodecLock = new object();
		private static ImageCodecMap _imageCodecMap;

		private ImageSopHelper()
		{ 
		}

		private static ImageCodecMap ImageCodecMap
		{
			get
			{
				lock (_imageCodecLock)
				{
					if (_imageCodecMap == null)
						_imageCodecMap = new ImageCodecMap();

					return _imageCodecMap;
				}
			}
		}

		/// <summary>
		/// Gets the pixel spacing appropriate to the modality.
		/// </summary>
		/// <param name="imageSop"></param>
		/// <param name="pixelSpacingX"></param>
		/// <param name="pixelSpacingY"></param>
		/// <remarks>
		/// For projection based modalities (i.e., CR, DX and MG), Imager Pixel Spacing is
		/// returned as the pixel spacing.  For all other modalities, the standard
		/// Pixel Spacing is returned.
		/// </remarks>
		public static void GetModalityPixelSpacing(ImageSop imageSop, out double pixelSpacingX, out double pixelSpacingY)
		{
			pixelSpacingX = 0;
			pixelSpacingY = 0;

			if (String.Compare(imageSop.Modality, "CR", true) == 0 ||
				String.Compare(imageSop.Modality, "DX", true) == 0 ||
				String.Compare(imageSop.Modality, "MG", true) == 0)
			{
				bool tagExists;
				imageSop.GetTag(DicomTags.ImagerPixelSpacing, out pixelSpacingY, 0, out tagExists);

				if (!tagExists)
					return;

				imageSop.GetTag(DicomTags.ImagerPixelSpacing, out pixelSpacingX, 1, out tagExists);
			}
			else
			{
				pixelSpacingX = imageSop.PixelSpacing.Row;
				pixelSpacingY = imageSop.PixelSpacing.Column;
			}
		}

		/// <summary>
		/// Decompresses/Decodes pixel data, if necessary.
		/// </summary>
		/// <param name="imageSop"></param>
		/// <param name="compressedPixelData">The pixel data.</param>
		/// <remarks>
		/// This method should be called by subclasses of <see cref="ImageSop"/>'s <see cref="PixelData"/>  property.
		/// </remarks>
		public static byte[] DecompressPixelData(ImageSop imageSop, byte[] compressedPixelData)
		{
			if (!ImageCodecMap.IsTransferSyntaxSupported(imageSop.TransferSyntaxUID))
				throw new Exception("Transfer syntax not supported.");

			byte[] uncompressedPixelData;

			try
			{
				uncompressedPixelData = ImageCodecMap[imageSop.TransferSyntaxUID].Decode(
					compressedPixelData,
					imageSop.Rows,
					imageSop.Columns,
					imageSop.BitsAllocated,
					imageSop.BitsStored,
					imageSop.PixelRepresentation,
					PhotometricInterpretationHelper.GetString(imageSop.PhotometricInterpretation),
					imageSop.SamplesPerPixel,
					imageSop.PlanarConfiguration,
					null);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
				throw new Exception("Unable to decode pixel data.", e);
			}

			return uncompressedPixelData;
		}
	}
}
