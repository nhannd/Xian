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

using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An image where pixel values are indices into a LUT.
	/// </summary>
	public abstract class IndexedImageGraphic : ImageGraphic
	{
		#region Private fields

		private int _bitsStored;
		private int _highBit;
		private bool _isSigned;
		private bool _invert;

		#endregion

		#region Protected constructors

		/// <summary>
		/// Initializes a new instance of <see cref="IndexedImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <remarks>
		/// Creates an empty indexed image of a specific size and bit depth.
		/// All all entries in the byte buffer are set to zero. Useful as
		/// a canvas on which pixels can be set by the client.
		/// </remarks>
		protected IndexedImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			bool invert)
			: base(
				rows,
				columns,
				bitsAllocated)
		{
			Initialize(bitsStored, highBit, isSigned, invert);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="IndexedImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <remarks>
		/// Creates an indexed image using existing pixel data.
		/// </remarks>
		protected IndexedImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			bool invert,
			byte[] pixelData)
			: base(
				rows,
				columns,
				bitsAllocated,
				pixelData)
		{
			Initialize(bitsStored, highBit, isSigned, invert);
		}

		/// <summary>
		/// Initializes a new instance of <see cref="IndexedImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated">Can be either 8 or 16.</param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <param name="isSigned"></param>
		/// <param name="invert"></param>
		/// <param name="pixelDataGetter"></param>
		/// <remarks>
		/// Creates an indexed image using existing pixel data but does so
		/// without ever storing a reference to the pixel data. This is necessary
		/// to ensure that pixel data can be properly garbage collected in
		/// any future memory management schemes.
		/// </remarks>
		protected IndexedImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			bool invert,
			PixelDataGetter pixelDataGetter)
			: base(
				rows,
				columns,
				bitsAllocated,
				pixelDataGetter)
		{
			Initialize(bitsStored, highBit, isSigned, invert);
		}

		private void Initialize(int bitsStored, int highBit, bool isSigned, bool invert)
		{
			DicomValidator.ValidateBitsStored(bitsStored);
			DicomValidator.ValidateHighBit(highBit);

			_bitsStored = bitsStored;
			_highBit = highBit;
			_isSigned = isSigned;
			_invert = invert;
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Gets the number of bits stored in the image.
		/// </summary>
		/// <remarks>The number of bits stored does not necessarily equal the number
		/// of bits allocated. Values of 8, 10, 12 and 16 are typical.</remarks>
		public int BitsStored
		{
			get { return _bitsStored; }
		}

		/// <summary>
		/// Gets the high bit in the image.
		/// </summary>
		/// <remarks>
		/// Theoretically, the high bit does not necessarily have to equal
		/// Bits Stored - 1.  But in almost all cases this assumption is true; we
		/// too make this assumption.
		/// </remarks>
		public int HighBit
		{
			get { return _highBit; }
		}

		/// <summary>
		/// Gets a value indicating whether the image's pixel data is signed.
		/// </summary>
		public bool IsSigned
		{
			get { return _isSigned; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the image should
		/// be inverted.
		/// </summary>
		/// <remarks>
		/// Inversion is equivalent to polarity.
		/// </remarks>
		public bool Invert
		{
			get { return _invert; }
			set { _invert = value; }
		}

		/// <summary>
		/// Gets an object that encapsulates the pixel data.
		/// </summary>
		public new IndexedPixelData PixelData
		{
			get
			{
				return base.PixelData as IndexedPixelData;
			}
		}

		/// <summary>
		/// The output of the LUT pipeline.
		/// </summary>
		public abstract IComposedLut OutputLut { get; }

		/// <summary>
		/// The color map for the image, if applicable.
		/// </summary>
		/// <remarks>
		/// Each entry in the <see cref="ColorMap"/> array is 32-bit ARGB value.
		/// When an <see cref="IRenderer"/> renders an <see cref="IndexedImageGraphic"/>, it should
		/// use <see cref="ColorMap"/> to determine the ARGB value to display for a given pixel value.
		/// </remarks>
		public abstract IColorMap ColorMap { get; }

		#endregion

		/// <summary>
		/// Creates an object that encapsulates the pixel data.
		/// </summary>
		/// <returns></returns>
		protected override PixelData CreatePixelDataWrapper()
		{
			if (this.PixelDataRaw != null)
			{
				return new IndexedPixelData(
									this.Rows,
									this.Columns,
									this.BitsPerPixel,
									this.BitsStored,
									this.HighBit,
									this.IsSigned,
									this.PixelDataRaw);
			}
			else
			{
				return new IndexedPixelData(
									this.Rows,
									this.Columns,
									this.BitsPerPixel,
									this.BitsStored,
									this.HighBit,
									this.IsSigned,
									this.PixelDataGetter);
			}
		}
	}
}
