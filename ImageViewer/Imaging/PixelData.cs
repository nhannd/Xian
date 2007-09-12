using System;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Imaging
{
	///<summary>
	/// Used by the <see cref="PixelData"/> class to (lazily) retrieve pixel data from an external source.
	///</summary>
	public delegate byte[] PixelDataGetter();

	/// <summary>
	/// A pixel data wrapper.
	/// </summary>
	/// <remarks>
	/// <see cref="PixelData"/> provides a number of convenience methods
	/// to make accessing and changing pixel data easier.  Use these methods
	/// judiciously, as the convenience comes at the expense of performance.
	/// For example, if you're doing complex image processing, using methods
	/// such as <see cref="SetPixel(int, int, int)"/> is not recommended if you want
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
		/// <param name="pixelData">The pixel data to be wrapped.</param>
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

		/// <summary>
		/// Initializes a new instance of <see cref="PixelData"/> with the
		/// specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated"></param>
		/// <param name="pixelDataGetter">A delegate that returns the pixel data.</param>
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
			DicomValidator.ValidateRows(rows);
			DicomValidator.ValidateColumns(columns);
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
		/// <remarks>
		/// <para>
		/// Use the byte array returned by this property to do unsafe processing.
		/// </para>
		/// <para>
		/// In general, you should avoid storing the byte array if at all possible.
		/// By storing it for say, the lifetime of the <see cref="ImageGraphic"/>, 
		/// future memory management schemes will be unable to release it, since
		/// a reference will have been created to it that such schemes may not
		/// be able to reach.  If you do need to store the byte array for some reason,
		/// do so only using local variables, since they have only method scope
		/// and can be garbage collected easily.
		/// </para>
		/// </remarks>
		public byte[] Raw
		{
			get 
			{
				return GetPixelData();
			}
		}

		#endregion

		#region Public methods

		/// <summary>
		/// Returns a copy of the object, including the pixel data.
		/// </summary>
		/// <returns></returns>
		public PixelData Clone()
		{
			return CloneInternal();
		}

		/// <summary>
		/// Gets the pixel value at the specified location.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns>
		/// The value of the pixel.  If the pixel data is colour,
		/// an ARGB value is returned, where A is the 
		/// most significant byte.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds.</exception>
		public int GetPixel(int x, int y)
		{
			int i = GetIndex(x, y);
			return GetPixelInternal(i);
		}

		/// <summary>
		/// Gets the pixel value at the specific pixel index.
		/// </summary>
		/// <param name="pixelIndex"></param>
		/// <returns></returns>
		/// <remarks>
		/// If the pixel data is treated as a one-dimensional array
		/// where each row of pixels is concatenated, <paramref name="pixelIndex"/>
		/// is the index into that array.  This is useful when you know the
		/// index of the pixel that you want to get and don't want to 
		/// incur the needless computational overhead associated with specifying
		/// an x and y value.
		/// </remarks>
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
		/// <param name="value">The value of the pixel.  If the pixel
		/// data is colour, the value is in ARGB form, where A is the 
		/// most significant byte.</param>
		/// <exception cref="ArgumentException"><paramref name="x"/> and/or
		/// <paramref name="y"/> are out of bounds, or <paramref name="value"/>
		/// is out of range.</exception>

		public void SetPixel(int x, int y, int value)
		{
			int i = GetIndex(x, y);
			SetPixelInternal(i, value);
		}

		/// <summary>
		/// Sets the pixel value at the specified pixel index.
		/// </summary>
		/// <param name="pixelIndex"></param>
		/// <param name="value">The value of the pixel.  If the pixel
		/// data is colour, the value is in ARGB form, where A is the 
		/// most significant byte.</param>
		/// <remarks>
		/// If the pixel data is treated as a one-dimensional array
		/// where each row of pixels is concatenated, <paramref name="pixelIndex"/>
		/// is the index into that array.  This is useful when you know the
		/// index of the pixel that you want to set and don't want to 
		/// incur the needless computational overhead associated with specifying
		/// an x and y value.
		/// </remarks>
		public void SetPixel(int pixelIndex, int value)
		{
			int i = pixelIndex * _bytesPerPixel;
			SetPixelInternal(i, value);
		}

		/// <summary>
		/// Used in conjunction with <see cref="PixelData.ForEachPixel(int,int,int,int,PixelProcessor)"/> for pixel processing.
		/// </summary>
		/// <param name="i">The ith pixel processed so far.  This is a zero based index.
		/// If iterating over the entire image, <paramref name="i"/>
		/// is the same as <paramref name="pixelIndex"/>.</param>
		/// <param name="x">The x value of the current pixel being processed.</param>
		/// <param name="y">The y value of the current pixel being processed</param>
		/// <param name="pixelIndex">The index of the pixel being processed.</param>
		/// <remarks>
		/// It is often desirable to iterate through the pixels in an image, or
		/// the pixels in a rectangular region in an image so that some kind of
		/// processing can be done.  When used in conjunction with 
		/// <see cref="ForEachPixel(PixelProcessor)"/> or
		/// <see cref="ForEachPixel(int, int, int, int, PixelProcessor)"/>,
		/// this delegate allows the client to focus on writing the processing code,
		/// not the boilerplate iteration code.
		/// </remarks>
		public delegate void PixelProcessor(int i, int x, int y, int pixelIndex);

		/// <summary>
		/// Iterates through all the pixels in the image.
		/// </summary>
		/// <param name="processor">Called for each pixel.</param>
		/// <remarks>
		/// It is often desirable to iterate through all the pixels in an image.
		/// This method encapsulates all the boilerplate code required to do that.
		/// </remarks>
		public void ForEachPixel(PixelProcessor processor)
		{
			ForEachPixel(0, 0, _columns - 1, _rows - 1, processor);
		}

		/// <summary>
		/// Iterates through all the pixels in a rectangular region of the image.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="top"></param>
		/// <param name="right"></param>
		/// <param name="bottom"></param>
		/// <param name="processor"></param>
		/// <remarks>
		/// It is often desirable to iterate through all the pixels in a rectangular
		/// region of an image. This method encapsulates all the boilerplate code 
		/// required to do that.
		/// </remarks>
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

		protected abstract PixelData CloneInternal();

		protected abstract int GetPixelInternal(int i);

		protected abstract void SetPixelInternal(int i, int value);

		protected byte[] GetPixelData()
		{
			if (_pixelData != null)
				return _pixelData;
			else
				return _pixelDataGetter();
		}

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
