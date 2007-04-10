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
	public class ImageGraphic : Graphic
	{
		#region Private fields

		private int _rows;
		private int _columns;
		private int _bitsAllocated;
		private int _bitsStored;
		private int _highBit;
		private int _samplesPerPixel;
		private int _pixelRepresentation;
		private int _planarConfiguration;
		private PhotometricInterpretation _photometricInterpretation;
		private byte[] _pixelDataRaw;
		private PixelData _pixelData;

		private int _sizeInBytes = -1;
		private int _sizeInPixels = -1;
		private int _doubleWordAlignedColumns = -1;
		private RectangleF _imageRectangle;

		private InterpolationMode _interpolationMode = InterpolationMode.Bilinear;

		#endregion

		/// <summary>
		/// Initializes a new instance of <see cref="ImageGraphic"/>
		/// with the specified <see cref="ImageSop"/>.
		/// </summary>
		/// <param name="imageSop"></param>
		/// <remarks>
		/// This constructor is provided for convenience in the case where
		/// the properties of <see cref="ImageGraphic"/> are the
		/// same as that of an existing <see cref="ImageSop"/>.
		/// Note that a reference to <paramref name="imageSop"/> is <i>not</i> held
		/// by <see cref="ImageGraphic"/>.
		/// </remarks>
		public ImageGraphic(ImageSop imageSop)
			: this(
			imageSop.Rows, 
			imageSop.Columns,
			imageSop.BitsAllocated,
			imageSop.BitsStored,
			imageSop.HighBit,
			imageSop.SamplesPerPixel,
			imageSop.PixelRepresentation,
			imageSop.PlanarConfiguration,
			imageSop.PhotometricInterpretation,
			imageSop.PixelData)
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageGraphic"/>
		/// with the specified image parameters.
		/// </summary>
		/// <param name="rows"></param>
		/// <param name="columns"></param>
		/// <param name="bitsAllocated"></param>
		/// <param name="bitsStored"></param>
		/// <param name="highBit"></param>
		/// <param name="samplesPerPixel"></param>
		/// <param name="pixelRepresentation"></param>
		/// <param name="planarConfiguration"></param>
		/// <param name="photometricInterpretation"></param>
		/// <param name="pixelData"></param>
		public ImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			int samplesPerPixel,
			int pixelRepresentation,
			int planarConfiguration,
			PhotometricInterpretation photometricInterpretation,
			byte[] pixelData)
		{
			ImageValidator.ValidateRows(rows);
			ImageValidator.ValidateColumns(columns);
			ImageValidator.ValidateBitsAllocated(bitsAllocated);
			ImageValidator.ValidateBitsStored(bitsStored);
			ImageValidator.ValidateHighBit(highBit);
			ImageValidator.ValidateSamplesPerPixel(samplesPerPixel);
			ImageValidator.ValidatePixelRepresentation(pixelRepresentation);
			ImageValidator.ValidatePhotometricInterpretation(photometricInterpretation);

			_rows = rows;
			_columns = columns;
			_bitsAllocated = bitsAllocated;
			_bitsStored = bitsStored;
			_highBit = highBit;
			_samplesPerPixel = samplesPerPixel;
			_pixelRepresentation = pixelRepresentation;
			_planarConfiguration = planarConfiguration;
			_photometricInterpretation = photometricInterpretation;
			_pixelDataRaw = pixelData;

			_imageRectangle = new RectangleF(0, 0, _columns - 1, _rows - 1);
		}

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
		/// <remarks>Theoretically, the high bit does not necessarily have to equal
		/// Bits Stored - 1.  But in almost all cases this assumption is true; we
		/// too make this assumption.</remarks>
		public int HighBit 
		{
			get { return _highBit; }
		}

		/// <summary>
		/// Gets the number of samples per pixel in the image.
		/// </summary>
		/// <remarks>For MONOCHROME1, MONOCHROME2 and PALETTE_COLOR images, this 
		/// property can be ignored.  For other colour (e.g. RGB, YBR_FULL, etc.)
		/// images, the value of this property is typically 3 or 4 depending
		/// on the particular <see cref="PhotometricInterpretation"/>.</remarks>
		public int SamplesPerPixel 
		{
			get { return _samplesPerPixel; } 
		}

		/// <summary>
		/// Gets the pixel representation of the image.
		/// </summary>
		/// <value>0 if the pixel data is unsigned or non-zero if the pixel data
		/// is signed.</value>
		public int PixelRepresentation 
		{
			get { return _pixelRepresentation; }
		}

		/// <summary>
		/// Gets the planar configuration of the image.
		/// </summary>
		/// <remarks>When pixel colour components are interleaved (e.g., RGBRGBRGB)
		/// the value of this property is 0.  When they organized in colour planes
		/// (e.g., RRRGGGBBB), the value is 1.</remarks>
		public int PlanarConfiguration 
		{
			get { return _planarConfiguration; }
		}
		
		/// <summary>
		/// Gets the photometric interpretation of the image.
		/// </summary>
		public virtual PhotometricInterpretation PhotometricInterpretation 
		{
			get { return _photometricInterpretation; }
		}

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

		/// <summary>
		/// Gets the <see cref="PixelData"/>.
		/// </summary>
		public PixelData PixelData
		{
			get
			{
				if (_pixelData == null)
				{
					_pixelData = new PixelData(
						this.Rows,
						this.Columns,
						this.BitsAllocated,
						this.BitsStored,
						this.HighBit,
						this.SamplesPerPixel,
						this.PixelRepresentation,
						this.PlanarConfiguration,
						this.PhotometricInterpretation,
						this.PixelDataRaw);
				}

				return _pixelData;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the image is grayscale.
		/// </summary>
		public bool IsGrayscale
		{
			get
			{
				return (this.PhotometricInterpretation == PhotometricInterpretation.Monochrome1 ||
						this.PhotometricInterpretation == PhotometricInterpretation.Monochrome2);
			}
		}

		/// <summary>
		/// Gets a value indicating whether the image is colour.
		/// </summary>
		public bool IsColor
		{
			get { return !this.IsGrayscale && this.PhotometricInterpretation != PhotometricInterpretation.Unknown; }
		}

		/// <summary>
		/// Gets a value indicating whether the image's <see cref="PlanarConfiguration"/> is 1.
		/// </summary>
		public bool IsPlanar
		{
			get { return this.PlanarConfiguration == 1; }
		}

		/// <summary>
		/// Gets a value indicating whether the image's pixel data is signed.
		/// </summary>
        public bool IsSigned
        {
            get { return this.PixelRepresentation != 0; }
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
					_sizeInBytes = this.SizeInPixels * this.SamplesPerPixel * this.BitsAllocated / 8;

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
	}
}
