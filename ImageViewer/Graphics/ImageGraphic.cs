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
	/// Describes an image.
	/// </summary>
	public class ImageGraphic : Graphic
	{
		public enum InterpolationMethods { NearestNeighbour, FastBilinear, Bilinear };

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

		private InterpolationMethods _normalInterpolationMethod = InterpolationMethods.Bilinear;
		//private InterpolationMethods _fastInterpolationMethod = InterpolationMethods.NearestNeighbour;
		private InterpolationMethods _fastInterpolationMethod = InterpolationMethods.FastBilinear;
		private bool _fastRender = false;

		#endregion

		public ImageGraphic(ImageSop image)
			: this(
			image.Rows, 
			image.Columns,
			image.BitsAllocated,
			image.BitsStored,
			image.HighBit,
			image.SamplesPerPixel,
			image.PixelRepresentation,
			image.PlanarConfiguration,
			image.PhotometricInterpretation,
			image.PixelData)
		{
		}

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
		/// bit stored - 1.  But in almost all cases this assumption is true; we
		/// too make this assumption.</remarks>
		public int HighBit 
		{
			get { return _highBit; }
		}

		/// <summary>
		/// Gets the number of samples per pixel in the image.
		/// </summary>
		/// <remarks>For monochrome images, this property can be ignored.  For colour
		/// images, the value of this property is typically 3 or 4 depending
		/// on the particular <see cref="PhotometricInterpretation"/>.</remarks>
		public int SamplesPerPixel 
		{
			get { return _samplesPerPixel; } 
		}

		/// <summary>
		/// Gets the pixel representation of the image.
		/// </summary>
		/// <remarks>When the pixel data is unsigned the value of this property
		/// is 0.  When it is signed, the value is 1.</remarks>
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
		protected virtual byte[] PixelDataRaw
		{
			get
			{
				if (_pixelDataRaw == null)
					_pixelDataRaw = new byte[this.SizeInBytes];

				return _pixelDataRaw;
			}
		}

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


		public bool FastRender
		{
			get 
			{
				return _fastRender; 
			}
			set
			{
				_fastRender = value;
			}
		}

		public InterpolationMethods FastInterpolationMethod
		{
			get { return _fastInterpolationMethod; }
			set { _fastInterpolationMethod = value; }
		}

		public InterpolationMethods NormalInterpolationMethod
		{
			get { return _normalInterpolationMethod; }
			set { _normalInterpolationMethod = value; }
		}

		public virtual InterpolationMethods InterpolationMethod
		{
			get
			{
				if (_fastRender)
					return _fastInterpolationMethod;

				return _normalInterpolationMethod;
			}
		}

		public override bool HitTest(Point point)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public override void Move(SizeF delta)
		{
			throw new Exception("The method or operation is not implemented.");
		}
	}
}
