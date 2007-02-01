using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Dicom;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class StandardImageGraphic : ImageGraphic, IImageSopProvider
	{
		private ImageSop _image;
		private bool _photometricInterpretationDefined = false;
		private PhotometricInterpretation _photometricInterpretation;
		private bool _ybrConvertedToRgb = false;
		private GrayscaleLUTPipeline _grayscaleLUTPipeline;

		public StandardImageGraphic(ImageSop image)
		{
			Platform.CheckForNullReference(image, "image");

			_image = image;

			if (this.PhotometricInterpretation == PhotometricInterpretation.Monochrome1)
				this.GrayscaleLUTPipeline.Invert = true;
		}

		public ImageSop ImageSop
		{
			get{ return _image; }
		}

		public override int Rows
		{
			get { return _image.Rows; }
		}

		public override int Columns
		{
			get { return _image.Columns; }
		}

		public override int BitsAllocated
		{
			get { return _image.BitsAllocated; }
		}

		public override int BitsStored
		{
			get { return _image.BitsStored; }
		}

		public override int HighBit
		{
			get { return _image.HighBit; }
		}

		public override int SamplesPerPixel
		{
			get { return _image.SamplesPerPixel; }
		}

		public override int PixelRepresentation
		{
			get { return _image.PixelRepresentation; }
		}

		public override int PlanarConfiguration
		{
			get { return _image.PlanarConfiguration; }
		}

		public override PhotometricInterpretation PhotometricInterpretation
		{
			get 
			{
				if (!_photometricInterpretationDefined)
				{
					_photometricInterpretation = _image.PhotometricInterpretation;
					_photometricInterpretationDefined = true;
				}

				return _photometricInterpretation;
			}
		}

		/// <summary>
		/// Gets the <see cref="GrayscaleLUTPipeline"/> of the image.
		/// </summary>
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
			// If it's a YBR image, convert it to RGB in place.  Having 
			// all non-indexed (i.e., non-palette) colour images in one format
			// makes image processing much easier.
			if (!_ybrConvertedToRgb &&
				this.PhotometricInterpretation == PhotometricInterpretation.YbrFull ||
				this.PhotometricInterpretation == PhotometricInterpretation.YbrFull422 ||
				this.PhotometricInterpretation == PhotometricInterpretation.YbrPartial422 ||
				this.PhotometricInterpretation == PhotometricInterpretation.YbrIct ||
				this.PhotometricInterpretation == PhotometricInterpretation.YbrRct)
			{
				_ybrConvertedToRgb = true;
				ColorSpaceConverter.YbrToRgb(this);
				_photometricInterpretationDefined = true;
				_photometricInterpretation = PhotometricInterpretation.Rgb;
			}

			return  _image.GetPixelData();
		}

		public override byte[] GetGrayscaleLUT()
		{
			if (this.GrayscaleLUTPipeline == null)
				throw new Exception(SR.ExceptionImageIsNotGrayscale);

			this.GrayscaleLUTPipeline.Execute();

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
