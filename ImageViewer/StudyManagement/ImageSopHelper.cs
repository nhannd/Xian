#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using ClearCanvas.Codecs;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	// TODO (Stewart): Merge methods into ImageSop

	/// <summary>
	/// A collection of image SOP helper methods.
	/// </summary>
	public static class ImageSopHelper
	{
		private static object _imageCodecLock = new object();
		private static ImageCodecMap _imageCodecMap;

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
