using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.Imaging;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class ColorPixelData : PixelData
	{
		public ColorPixelData(
			int rows,
			int columns,
			byte[] pixelData)
			: base(rows, columns, 32, pixelData)
		{
		}

		public ColorPixelData(
			int rows,
			int columns,
			PixelDataGetter pixelDataGetter)
			: base(rows, columns, 32, pixelDataGetter)
		{
		}

		#region Public methods

		public override PixelData Clone()
		{
			return new ColorPixelData(_rows, _columns, (byte[])_pixelData.Clone());
		}

		/// <summary>
		/// Gets the RGB pixel value at the specified location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		/// <remarks>
		/// <see cref="GetPixelRGB"/> only works with images whose
		/// photometric interpretation is RGB.
		/// </remarks>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds.</exception>
		/// <exception cref="InvalidOperationException">The photometric
		/// interpretation is not RGB.</exception>
		public Color GetPixelAsColor(int x, int y)
		{
			return Color.FromArgb(GetPixel(x, y));
		}

		/// <summary>
		/// Sets the RGB pixel value at the specified location.
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

		public void SetPixel(int pixelIndex, Color color)
		{
			SetPixel(pixelIndex, color.A, color.R, color.G, color.B);
		}

		/// <summary>
		/// Sets the RGB pixel value at the specified location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="a"></param>
		/// <param name="r"></param>
		/// <param name="g"></param>
		/// <param name="b"></param>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds.</exception>
		public void SetPixel(int x, int y, byte a, byte r, byte g, byte b)
		{
			int i = GetIndex(x, y);
			SetPixelInternal(i, a, r, g, b);
		}

		public void SetPixel(int pixelIndex, byte a, byte r, byte g, byte b)
		{
			int i = pixelIndex * _bytesPerPixel;
			SetPixelInternal(i, a, r, g, b);
		}

		#endregion

		#region Overrides

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
