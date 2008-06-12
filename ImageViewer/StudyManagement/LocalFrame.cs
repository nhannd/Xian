#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// A local, file-based implementation of <see cref="Frame"/>.
	/// </summary>
	public class LocalFrame : Frame
	{
		private readonly object _syncLock = new object();
		private volatile byte[] _pixelData;

		/// <summary>
		/// Initializes a new instance of <see cref="LocalFrame"/>.
		/// </summary>
		/// <param name="parentImageSop"></param>
		/// <param name="frameNumber"></param>
		protected internal LocalFrame(LocalImageSop parentImageSop, int frameNumber)
			: base(parentImageSop, frameNumber)
		{

		}

		private LocalImageSop ParentLocalImageSop
		{
			get { return this.ParentImageSop as LocalImageSop; }
		}


		/// <summary>
		/// Gets pixel data in normalized form.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// <i>Normalized</i> pixel data means that:
		/// <list type="Bullet">
		/// <item>
		/// <description>Grayscale pixel data is unchanged.</description>
		/// </item>
		/// <item>
		/// <description>Colour pixel data is always converted
		/// into ARGB format.</description>
		/// </item>
		/// <item>
		/// <description>Pixel data is always uncompressed.</description>
		/// </item>
		/// </list>
		/// Ensuring that the pixel data always meets the above criteria
		/// allows clients to easily consume pixel data without having
		/// to worry about the the multitude of DICOM photometric interpretations
		/// and transfer syntaxes.
		/// </remarks>
		/// <seealso cref="Frame.ToArgb"/>
		public override byte[] GetNormalizedPixelData()
		{
			if (_pixelData == null)
			{
				lock (_syncLock)
				{
					if (_pixelData == null)
					{
						this.ParentLocalImageSop.Load();

						DicomMessageBase message = this.ParentLocalImageSop.NativeDicomObject;

						PhotometricInterpretation photometricInterpretation;

						byte[] temp;
						if (!message.TransferSyntax.Encapsulated)
						{
							DicomUncompressedPixelData pixelData = new DicomUncompressedPixelData(message);
							temp = pixelData.GetFrame(this.FrameNumber - 1);
							photometricInterpretation = this.PhotometricInterpretation;
						}
						else if (DicomCodecRegistry.GetCodec(message.TransferSyntax) != null)
						{
							DicomCompressedPixelData pixelData = new DicomCompressedPixelData(message);
							string pi;
							temp = pixelData.GetFrame(this.FrameNumber - 1, out pi);
							photometricInterpretation = PhotometricInterpretationHelper.FromString(pi);
						}
						else
							throw new DicomCodecException("Unsupported transfer syntax");

						// DICOM library uses zero-based frame numbers

						if (this.IsColor)
							temp = this.ToArgb(temp, photometricInterpretation);

						_pixelData = temp;
					}
				}
			}

			return _pixelData;
		}

		/// <summary>
		/// Unloads the pixel data.
		/// </summary>
		/// <remarks>
		/// It is sometimes necessary to manage the memory used by unloading the pixel data. 
		/// Calling this method will not necessarily result in an immediate decrease in memory
		/// usage, since it merely releases the reference to the pixel data; it is up to the
		/// garbage collector to free the memory.  Calling <see cref="GetNormalizedPixelData"/>
		/// will reload the pixel data.
		/// </remarks>
		public override void UnloadPixelData()
		{
			lock (_syncLock)
			{
				_pixelData = null;
			}
		}
	}
}
