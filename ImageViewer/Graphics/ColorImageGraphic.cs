#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An image where pixel values are ARGB.
	/// </summary>
	/// <remarks>
	/// A typical usage of this class is overlaying
	/// colour regions on a grayscale image to highlight areas of interest.
	/// Note that you can control not just the colour, but also the 
	/// opacity (i.e. alpha) of each pixel.
	/// </remarks>
	[Cloneable]
	public class ColorImageGraphic : ImageGraphic
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ColorImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <remarks>
		/// Creates an empty colour image of a specific size.
		/// By default, all pixels are set to ARGB=(0,0,0,0) (i.e.,
		/// transparent). 
		/// </remarks>
		public ColorImageGraphic(int rows, int columns)
			: base(rows, columns, 32)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ColorImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelData">The pixel data in ARGB format.</param>
		/// <remarks>
		/// Creates a colour image using existing pixel data.
		/// </remarks>
		public ColorImageGraphic(int rows, int columns, byte[] pixelData)
			: base(rows, columns, 32, pixelData)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ColorImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelDataGetter"></param>
		/// <remarks>
		/// Creates a grayscale image using existing pixel data but does so
		/// without ever storing a reference to the pixel data. This is necessary
		/// to ensure that pixel data can be properly garbage collected in
		/// any future memory management schemes.
		/// </remarks>
		public ColorImageGraphic(int rows, int columns, PixelDataGetter pixelDataGetter)
			: base(rows, columns, 32, pixelDataGetter)
		{
		}

		/// <summary>
		/// Cloning constructor.
		/// </summary>
		protected ColorImageGraphic(ColorImageGraphic source, ICloningContext context)
			: base(source, context)
		{
			context.CloneFields(source, this);
		}

		/// <summary>
		/// Gets an object that encapsulates the pixel data.
		/// </summary>
		public new ColorPixelData PixelData
		{
			get
			{
				return base.PixelData as ColorPixelData;
			}
		}

		/// <summary>
		/// Creates an object that encapsulates the pixel data.
		/// </summary>
		/// <returns></returns>
		protected override PixelData CreatePixelDataWrapper()
		{
			if (this.PixelDataRaw != null)
				return new ColorPixelData(this.Rows, this.Columns, this.PixelDataRaw);
			else
				return new ColorPixelData(this.Rows, this.Columns, this.PixelDataGetter);
		}
	}
}
