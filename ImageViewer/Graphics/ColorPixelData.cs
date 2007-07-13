using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
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
	public class ColorPixelData : PixelData
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ColorPixelData"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="pixelData">The pixel data to be wrapped.</param>
		public ColorPixelData(
			int rows,
			int columns,
			byte[] pixelData)
			: base(rows, columns, 32, pixelData)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ColorPixelData"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
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
		/// <returns></returns>
		public new ColorPixelData Clone()
		{
			return base.Clone() as ColorPixelData;
		}

		/// <summary>
		/// Gets the ARGB pixel value at the specified location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds.</exception>
		public Color GetPixelAsColor(int x, int y)
		{
			return Color.FromArgb(GetPixel(x, y));
		}

		/// <summary>
		/// Sets the ARGB pixel value at the specified location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="color"></param>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds.</exception>
		public void SetPixel(int x, int y, Color color)
		{
			SetPixel(x, y, color.A, color.R, color.G, color.B);
		}

		/// <summary>
		/// Sets the ARGB pixel value at a specific pixel index.
		/// </summary>
		/// <param name="pixelIndex"></param>
		/// <param name="color"></param>
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
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="a">Alpha</param>
		/// <param name="r">Red</param>
		/// <param name="g">Green</param>
		/// <param name="b">Blue</param>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds.</exception>
		public void SetPixel(int x, int y, byte a, byte r, byte g, byte b)
		{
			int i = GetIndex(x, y);
			SetPixelInternal(i, a, r, g, b);
		}

		/// <summary>
		/// Sets the ARGB pixel value at a particular pixel index.
		/// </summary>
		/// <param name="pixelIndex"></param>
		/// <param name="a">Alpha</param>
		/// <param name="r">Red</param>
		/// <param name="g">Green</param>
		/// <param name="b">Blue</param>
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

		protected override PixelData CloneInternal()
		{
			return new ColorPixelData(_rows, _columns, (byte[])_pixelData.Clone());
		}

		protected override int GetPixelInternal(int i)
		{
			int b = (int)_pixelData[i];
			int g = (int)_pixelData[i + 1];
			int r = (int)_pixelData[i + 2];
			int a = (int)_pixelData[i + 3];

			int argb = a << 24 | r << 16 | g << 8 | b;
			return argb;
		}

		protected override void SetPixelInternal(int i, int value)
		{
			_pixelData[i]     = (byte)(value & 0x000000ff);
			_pixelData[i + 1] = (byte)((value & 0x0000ff00) >> 8);
			_pixelData[i + 2] = (byte)((value & 0x00ff0000) >> 16);
			_pixelData[i + 3] = (byte)((value & 0xff000000) >> 24);
		}

		#endregion

		#region Private methods

		private void SetPixelInternal(int i, byte a, byte r, byte g, byte b)
		{
			_pixelData[i] = b;
			_pixelData[i + 1] = g;
			_pixelData[i + 2] = r;
			_pixelData[i + 3] = a;
		}

		#endregion
	}
}
