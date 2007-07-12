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

	public delegate byte[] PixelDataGetter();

	/// <summary>
	/// An image <see cref="Graphic"/>.
	/// </summary>
	/// <remarks>
	/// The derived classes <see cref="IndexedImageGraphic"/> and 
	/// <see cref="ColorImageGraphic"/> represent the two basic types of
	/// 2D images in the framework.
	/// 
	/// An <see cref="ImageGraphic"/> is always a leaf in the scene graph.
	/// </remarks>
	public abstract class ImageGraphic : Graphic
	{
		#region Private fields

		private int _rows;
		private int _columns;
		private int _bitsPerPixel;

		private byte[] _pixelDataRaw;
		private PixelDataGetter _pixelDataGetter;
		private PixelData _pixelDataWrapper;

		private int _sizeInBytes = -1;
		private int _sizeInPixels = -1;
		private int _doubleWordAlignedColumns = -1;

		private InterpolationMode _interpolationMode = InterpolationMode.Bilinear;

		#endregion

		#region Protected constructor

		protected ImageGraphic(int rows, int columns, int bitsPerPixel) :
			this(rows, columns, bitsPerPixel, new byte[rows * columns * bitsPerPixel / 8])
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsPerPixel"></param>
		/// <param name="pixelData">If <b>null</b>, a byte buffer of size
		/// <i>rows</i> x <i>columns</i> x <i>bitsPerPixel</i> / 8
		/// will be allocated.</param>
		/// <exception cref="ArgumentException"></exception>
		protected ImageGraphic(int rows, int columns, int bitsPerPixel, byte[] pixelData)
		{
			Platform.CheckForNullReference(pixelData, "pixelData");
			ImageValidator.ValidatePixelData(pixelData, rows, columns, bitsPerPixel);
			_pixelDataRaw = pixelData;
			Initialize(rows, columns, bitsPerPixel);
		}

		protected ImageGraphic(int rows, int columns, int bitsPerPixel, PixelDataGetter pixelDataGetter)
		{
			Platform.CheckForNullReference(pixelDataGetter, "pixelDataGetter");
			_pixelDataGetter = pixelDataGetter;
			Initialize(rows, columns, bitsPerPixel);
		}

		private void Initialize(int rows, int columns, int bitsPerPixel)
		{
			ImageValidator.ValidateRows(rows);
			ImageValidator.ValidateColumns(columns);

			_rows = rows;
			_columns = columns;
			_bitsPerPixel = bitsPerPixel;
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
		/// Gets the number of bits per pixel.
		/// </summary>
		/// <remarks>In the case of <see cref="IndexedImageGraphic"/>, this
		/// property will always have a value of 8 or 16, whereas in the
		/// case of <see cref="ColorImageGraphic"/>, it will always have
		/// a value of 32 (ARGB).</remarks>
		public int BitsPerPixel 
		{
			get { return _bitsPerPixel; }
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
					_sizeInBytes = this.SizeInPixels * this.BitsPerPixel / 8;

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

		public PixelData PixelData
		{
			get
			{
				if (_pixelDataWrapper == null)
					_pixelDataWrapper = CreatePixelDataWrapper();

				return _pixelDataWrapper; 
			}
		}

		#endregion

		#region Protected properties/methods

		protected byte[] PixelDataRaw
		{
			get { return _pixelDataRaw; }
		}

		protected PixelDataGetter PixelDataGetter
		{
			get { return _pixelDataGetter; }
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
