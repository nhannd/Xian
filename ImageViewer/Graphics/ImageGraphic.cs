using System;
using System.Drawing;
using System.Collections;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// An <see cref="InterpolationMode"/> enumeration specifies the
	/// interpolation algorithm to use when rendering the image.
	/// </summary>
	public enum InterpolationMode 
	{ 
		/// <summary>
		/// Specifies nearest neighbour interpolation.
		/// </summary>
		//NearestNeighbour,

		/// <summary>
		/// Specifies bilinear interpolation using fixed-point arithmetic.
		/// </summary>
		Bilinear 
	};


	/// <summary>
	/// An image <see cref="Graphic"/>.
	/// </summary>
	public abstract class ImageGraphic : Graphic
	{
		#region Private fields

		private int _rows;
		private int _columns;
		private int _bitsAllocated;
		private int _samplesPerPixel;
		private byte[] _pixelDataRaw;
		protected PixelData _pixelDataWrapper;

		private int _sizeInBytes = -1;
		private int _sizeInPixels = -1;
		private int _doubleWordAlignedColumns = -1;
		private RectangleF _imageRectangle;

		private InterpolationMode _interpolationMode = InterpolationMode.Bilinear;

		#endregion

		#region Protected constructor

		/// <summary>
		/// Initializes a new instance of <see cref="ImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated"></param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <param name="pixelData"></param>
		protected ImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			byte[] pixelData)
		{
			ImageValidator.ValidateRows(rows);
			ImageValidator.ValidateColumns(columns);

			_rows = rows;
			_columns = columns;
			_bitsAllocated = bitsAllocated;
			_pixelDataRaw = pixelData;

			_imageRectangle = new RectangleF(0, 0, _columns - 1, _rows - 1);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Gets the number of rows in the image.
		/// </summary>
		public int Rows 
		{ 
			get { return _rows; } 
		}

		/// <summary>
		/// Gets the number of columns in the image.
		/// </summary>
		public int Columns 
		{
			get { return _columns; } 
		}

		/// <summary>
		/// Gets the number of bits allocated in the image.
		/// </summary>
		/// <remarks>The number of bits allocated will always either be 8 or 16.</remarks>
		public int BitsAllocated 
		{
			get { return _bitsAllocated; }
		}

		/// <summary>
		/// Gets the <see cref="PixelData"/>.
		/// </summary>
		public PixelData PixelData
		{
			get
			{
				if (_pixelDataWrapper == null)
					_pixelDataWrapper = CreatePixelDataWrapper();

				return _pixelDataWrapper;
			}
		}

		/// <summary>
		/// Gets a value indicating whether image is aligned on a 4-byte boundary
		/// </summary>
		/// <remarks>Bitmaps in Windows need to be aligned on a 4-byte boundary.  
		/// That is, the width of an image must be divisible by 4.</remarks>
		public bool IsDoubleWordAligned
		{
			get
			{
				return (this.Columns % 4) == 0;
			}
		}

		/// <summary>
		/// Gets the size of the image in pixels.
		/// </summary>
		public int SizeInPixels
		{
			get
			{
				if (_sizeInPixels == -1)
					_sizeInPixels = this.Rows * this.Columns;

				return _sizeInPixels;
			}
		}

		/// <summary>
		/// Gets the size of the image in bytes.
		/// </summary>
		public int SizeInBytes
		{
			get
			{
				// Only calculate this once
				if (_sizeInBytes == -1)
					_sizeInBytes = this.SizeInPixels * this.BitsAllocated / 8;

				return _sizeInBytes;
			}
		}

		/// <summary>
		/// Gets the number of columns when the image has been aligned on a 4-byte boundary.
		/// </summary>
		public int DoubleWordAlignedColumns
		{
			get
			{
				// Only calculate this once
				if (_doubleWordAlignedColumns == -1)
				{
					// If we're not on a 4-byte boundary, round up to the next multiple of 4
					// using integer division
					if (this.Columns % 4 != 0)
						_doubleWordAlignedColumns = this.Columns / 4 * 4 + 4;
					else
						_doubleWordAlignedColumns = this.Columns;
				}

				return _doubleWordAlignedColumns;
			}
		}

		/// <summary>
		/// Gets the current interpolation method.
		/// </summary>
		public virtual InterpolationMode InterpolationMode
		{
			get { return _interpolationMode; }
		}

		#endregion

		#region Protected properties/methods

		/// <summary>
		/// Gets the pixel data of the image.
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// By default, <see cref="PixelDataRaw"/> returns an empty array of bytes
		/// of size <see cref="SizeInBytes"/>.  Override this property if you want
		/// the pixel data to be otherwise.  Note that this is what is returned
		/// by <see cref="ClearCanvas.ImageViewer.Graphics.PixelData.Raw"/>.
		/// </remarks>
		protected virtual byte[] PixelDataRaw
		{
			get
			{
				if (_pixelDataRaw == null)
					_pixelDataRaw = new byte[this.SizeInBytes];

				return _pixelDataRaw;
			}
		}

		protected abstract PixelData CreatePixelDataWrapper();

		#endregion

		#region Public methods

		/// <summary>
		/// Performs a hit test on the <see cref="ImageGraphic"/> at a given point.
		/// </summary>
		/// <param name="point">The mouse position in destination coordinates.</param>
		/// <returns><b>True</b> if <paramref name="point"/> is within the boundaries
		/// of the image, <b>false</b> otherwise.</returns>
		public override bool HitTest(Point point)
		{
			PointF srcPoint = this.SpatialTransform.ConvertToSource(point);

			if (srcPoint.X >= 0.0 &&
				srcPoint.X < _columns &&
				srcPoint.Y >= 0.0 &&
				srcPoint.Y < _rows)
				return true;
			else
				return false;
		}

		public override void Move(SizeF delta)
		{
			//this.SpatialTransform.
		}

		#endregion
	}
}
