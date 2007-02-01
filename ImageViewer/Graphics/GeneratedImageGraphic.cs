using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class GeneratedImageGraphic : ImageGraphic
	{
		private int _rows;
		private int _columns;
		private int _bitsAllocated;
		private int _bitsStored;
		private int _highBit;
		private int _samplesPerPixel;
		private int _pixelRepresentation;
		private int _planarConfiguration;
		private PhotometricInterpretation _photometricInterpretation;
		private byte[] _pixelData;
		private GrayscaleLUTPipeline _grayscaleLUTPipeline;

		public GeneratedImageGraphic(ImageSop image)
			: this(
			image.Rows, 
			image.Columns,
			image.BitsAllocated,
			image.BitsStored,
			image.HighBit,
			image.SamplesPerPixel,
			image.PixelRepresentation,
			image.PlanarConfiguration,
			image.PhotometricInterpretation)
		{

		}

		public GeneratedImageGraphic(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			int samplesPerPixel,
			int pixelRepresentation,
			int planarConfiguration,
			PhotometricInterpretation photometricInterpretation)
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
		}

		public override int Rows
		{
			get { return _rows; }
		}

		public override int Columns
		{
			get { return _columns; }
		}

		public override int BitsAllocated
		{
			get { return _bitsAllocated; }
		}

		public override int BitsStored
		{
			get { return _bitsStored; }
		}

		public override int HighBit
		{
			get { return _highBit; }
		}

		public override int SamplesPerPixel
		{
			get { return _samplesPerPixel; }
		}

		public override int PixelRepresentation
		{
			get { return _pixelRepresentation; }
		}

		public override int PlanarConfiguration
		{
			get { return _planarConfiguration; }
		}

		public override PhotometricInterpretation PhotometricInterpretation
		{
			get { return _photometricInterpretation; }
		}

		public GrayscaleLUTPipeline GrayscaleLUTPipeline
		{
			get
			{
				if (this.IsColor)
					return null;

				if (_grayscaleLUTPipeline == null)
					_grayscaleLUTPipeline = new GrayscaleLUTPipeline();

				return _grayscaleLUTPipeline;
			}
		}

		public override byte[] GetPixelData()
		{
			if (_pixelData == null)
				_pixelData = new byte[this.SizeInBytes];

			return _pixelData;
		}

		public override byte[] GetGrayscaleLUT()
		{
			if (this.GrayscaleLUTPipeline == null)
				throw new Exception(SR.ExceptionImageIsNotGrayscale);

			return this.GrayscaleLUTPipeline.OutputLUT;
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
