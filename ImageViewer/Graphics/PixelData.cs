using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using ClearCanvas.Common.Utilities;
using System.Diagnostics;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A pixel data wrapper.
	/// </summary>
	/// <remarks>
	/// <see cref="PixelData"/> provides a number of convenience methods
	/// to make accessing and changing pixel data easier.  Use these methods
	/// judiciously, as the convenience comes at the expense of performance.
	/// For example, if you're doing complex image processing, using methods
	/// such as <see cref="SetPixel"/> is not recommended if you want
	/// good performance.  Instead, use the <see cref="Raw"/> property 
	/// to get the raw byte array, then use unsafe code to do your processing.
	/// </remarks>
	public abstract class PixelData
	{
		#region Private fields

		protected int _rows;
		protected int _columns;
		protected int _bitsAllocated;
		protected byte[] _pixelData;
		protected PixelDataGetter _pixelDataGetter;

		protected int _bytesPerPixel;
		private int _stride;

		#endregion

		#region Protected constructor

		/// <summary>
		/// Initializes a new instance of <see cref="PixelData"/> with the
		/// specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated"></param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <param name="pixelData"></param>
		protected PixelData(
			int rows,
			int columns,
			int bitsAllocated,
			byte[] pixelData)
		{
			Platform.CheckForNullReference(pixelData, "pixelData");
			_pixelData = pixelData;

			Initialize(rows, columns, bitsAllocated);
		}

		protected PixelData(
			int rows,
			int columns,
			int bitsAllocated,
			PixelDataGetter pixelDataGetter)
		{
			Platform.CheckForNullReference(pixelDataGetter, "pixelDataGetter");
			_pixelDataGetter = pixelDataGetter;

			Initialize(rows, columns, bitsAllocated);
		}

		private void Initialize(int rows, int columns, int bitsAllocated)
		{
			ImageValidator.ValidateRows(rows);
			ImageValidator.ValidateColumns(columns);
			_rows = rows;
			_columns = columns;
			_bitsAllocated = bitsAllocated;

			_bytesPerPixel = bitsAllocated / 8;
			_stride = _columns * _bytesPerPixel;
		}
		
		#endregion

		#region Public properties

		/// <summary>
		/// Gets the raw pixel data.
		/// </summary>
		public byte[] Raw
		{
			get 
			{
				if (_pixelData != null)
					return _pixelData;
				else
					return _pixelDataGetter();
			}
		}

		#endregion

		#region Public methods

		public abstract PixelData Clone();

		/// <summary>
		/// Gets the pixel value at the specified location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>
		/// The value of the pixel.  In the case where the pixel data is colour,
		/// an ARGB value is returned.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds.</exception>
		public int GetPixel(int x, int y)
		{
			int i = GetIndex(x, y);
			return GetPixelInternal(i);
		}

		public int GetPixel(int pixelIndex)
		{
			int i = pixelIndex * _bytesPerPixel;
			return GetPixelInternal(i);
		}

		/// <summary>
		/// Sets the pixel value at the specified location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="value"></param>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds, or <paramref name="value"/>
		/// is out of range.</exception>
		public void SetPixel(int x, int y, int value)
		{
			int i = GetIndex(x, y);
			SetPixelInternal(i, value);
		}

		public void SetPixel(int pixelIndex, int value)
		{
			int i = pixelIndex * _bytesPerPixel;
			SetPixelInternal(i, value);
		}

		public delegate void PixelProcessor(int i, int x, int y, int pixelIndex);

		public void ForEachPixel(PixelProcessor processor)
		{
			ForEachPixel(0, 0, _columns - 1, _rows - 1, processor);
		}

		public void ForEachPixel(
			int left, int top, int right, int bottom, 
			PixelProcessor processor)
		{
			int i = 0;
			int temp;

			if (top > bottom)
			{
				temp = top;
				top = bottom;
				bottom = temp;
			}

			if (left > right)
			{
				temp = left;
				left = right;
				right = temp;
			}

			int pixelIndex = top * _columns + left;

			for (int y = top; y <= bottom; y++)
			{
				for (int x = left; x <= right; x++)
				{
					processor(i, x, y, pixelIndex);
					pixelIndex++;
					i++;
				}

				pixelIndex += (_columns - right) + left - 1;
			}
		}

		#endregion

		#region Protected methods

		protected abstract int GetPixelInternal(int i);

		protected abstract void SetPixelInternal(int i, int value);

		#endregion

		#region Private methods

		protected int GetIndex(int x, int y)
		{
			if (x < 0 ||
				x >= _columns ||
				y < 0 ||
				y >= _rows)
				throw new ArgumentException("x and/or y are out of bounds");

			int i = (y * _stride) + (x * _bytesPerPixel);
			return i;
		}

		#endregion
	}
}
