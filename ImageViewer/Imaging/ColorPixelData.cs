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

using System;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Imaging
{
	/// <summary>
	/// A colour pixel wrapper.
	/// </summary>
	/// <remarks>
	/// <see cref="ColorPixelData"/> provides a number of convenience methods
	/// to make accessing and changing colour pixel data easier.  Use these methods
	/// judiciously, as the convenience comes at the expense of performance.
	/// For example, if you're doing complex image processing, using methods
	/// such as <see cref="PixelData.SetPixel(int, int, int)"/> is not recommended if you want
	/// good performance.  Instead, use the <see cref="PixelData.Raw"/> property 
	/// to get the raw byte array, then use unsafe code to do your processing.
	/// </remarks>
	/// <seealso cref="PixelData"/>
	public class ColorPixelData : PixelData
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ColorPixelData"/> with the specified image parameters.
		/// </summary>
		/// <param name="rows">The number of rows.</param>
		/// <param name="columns">The number of columns.</param>
		/// <param name="pixelData">The pixel data to be wrapped.</param>
		public ColorPixelData(
			int rows,
			int columns,
			byte[] pixelData)
			: base(rows, columns, 32, pixelData)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ColorPixelData"/> with the specified image parameters.
		/// </summary>
		/// <param name="rows">The number of rows.</param>
		/// <param name="columns">The number of columns.</param>
		/// <param name="pixelDataGetter">A delegate that returns the pixel data.</param>
		public ColorPixelData(
			int rows,
			int columns,
			PixelDataGetter pixelDataGetter)
			: base(rows, columns, 32, pixelDataGetter)
		{
		}

		#region Public methods

		/// <summary>
		/// Returns a copy of the object, including the pixel data.
		/// </summary>
		public new ColorPixelData Clone()
		{
			return base.Clone() as ColorPixelData;
		}

		/// <summary>
		/// Gets the ARGB pixel value at the specified location.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <exception cref="ArgumentException">Thrown when <paramref name="x"/> and/or <paramref name="y"/> are out of bounds.</exception>
		public Color GetPixelAsColor(int x, int y)
		{
			return Color.FromArgb(GetPixel(x, y));
		}

		/// <summary>
		/// Sets the ARGB pixel value at the specified location.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="color">The color to set.</param>
		/// <exception cref="ArgumentException">Thrown when <paramref name="x"/> and/or <paramref name="y"/> are out of bounds.</exception>
		public void SetPixel(int x, int y, Color color)
		{
			SetPixel(x, y, color.A, color.R, color.G, color.B);
		}

		/// <summary>
		/// Sets the ARGB pixel value at a specific pixel index.
		/// </summary>
		/// <param name="pixelIndex">The pixel index.</param>
		/// <param name="color">The color to set.</param>
		/// <remarks>
		/// If the pixel data is treated as a one-dimensional array
		/// where each row of pixels is concatenated, <paramref name="pixelIndex"/>
		/// is the index into that array.  This is useful when you know the
		/// index of the pixel that you want to set and don't want to 
		/// incur the needless computational overhead associated with specifying
		/// an x and y value.
		/// </remarks>
		public void SetPixel(int pixelIndex, Color color)
		{
			SetPixel(pixelIndex, color.A, color.R, color.G, color.B);
		}

		/// <summary>
		/// Sets the ARGB pixel value at the specified location.
		/// </summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <param name="a">The alpha component.</param>
		/// <param name="r">The red component.</param>
		/// <param name="g">The green component.</param>
		/// <param name="b">The blue component.</param>
		/// <exception cref="ArgumentException">Thrown when <paramref name="x"/> and/or <paramref name="y"/> are out of bounds.</exception>
		public void SetPixel(int x, int y, byte a, byte r, byte g, byte b)
		{
			int i = GetIndex(x, y);
			SetPixelInternal(i, a, r, g, b);
		}

		/// <summary>
		/// Sets the ARGB pixel value at a particular pixel index.
		/// </summary>
		/// <param name="pixelIndex">The pixel index</param>
		/// <param name="a">The alpha component.</param>
		/// <param name="r">The red component.</param>
		/// <param name="g">The green component.</param>
		/// <param name="b">The blue component.</param>
		/// <remarks>
		/// If the pixel data is treated as a one-dimensional array
		/// where each row of pixels is concatenated, <paramref name="pixelIndex"/>
		/// is the index into that array.  This is useful when you know the
		/// index of the pixel that you want to set and don't want to 
		/// incur the needless computational overhead associated with specifying
		/// an x and y value.
		/// </remarks>
		public void SetPixel(int pixelIndex, byte a, byte r, byte g, byte b)
		{
			int i = pixelIndex * _bytesPerPixel;
			SetPixelInternal(i, a, r, g, b);
		}

		#endregion

		#region Overrides

		/// <summary>
		/// This method overrides <see cref="PixelData.CloneInternal"/>.
		/// </summary>
		protected override PixelData CloneInternal()
		{
			return new ColorPixelData(_rows, _columns, (byte[])GetPixelData().Clone());
		}

		/// <summary>
		/// This method overrides <see cref="PixelData.GetPixelInternal"/>.
		/// </summary>
		protected override int GetPixelInternal(int i)
		{
			byte[] pixelData = GetPixelData();

			int b = (int)pixelData[i];
			int g = (int)pixelData[i + 1];
			int r = (int)pixelData[i + 2];
			int a = (int)pixelData[i + 3];

			int argb = a << 24 | r << 16 | g << 8 | b;
			return argb;
		}

		/// <summary>
		/// This method overrides <see cref="PixelData.SetPixelInternal"/>.
		/// </summary>
		protected override void SetPixelInternal(int i, int value)
		{
			byte[] pixelData = GetPixelData();

			pixelData[i]     = (byte)(value & 0x000000ff);
			pixelData[i + 1] = (byte)((value & 0x0000ff00) >> 8);
			pixelData[i + 2] = (byte)((value & 0x00ff0000) >> 16);
			pixelData[i + 3] = (byte)((value & 0xff000000) >> 24);
		}

		#endregion

		#region Private methods

		private void SetPixelInternal(int i, byte a, byte r, byte g, byte b)
		{
			byte[] pixelData = GetPixelData();

			pixelData[i] = b;
			pixelData[i + 1] = g;
			pixelData[i + 2] = r;
			pixelData[i + 3] = a;
		}

		#endregion
	}
}
