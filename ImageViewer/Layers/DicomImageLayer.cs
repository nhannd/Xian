using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Layers
{
	public class DicomImageLayer : ImageLayer
	{
		private ImageSop _image;
		private bool _photometricInterpretationDefined = false;
		private PhotometricInterpretations _photometricInterpretation;
		private bool _ybrConvertedToRgb = false;

		public DicomImageLayer(ImageSop image)
		{
			Platform.CheckForNullReference(image, "image");

			_image = image;

			if (this.PhotometricInterpretation == PhotometricInterpretations.Monochrome1)
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

		public override PhotometricInterpretations PhotometricInterpretation
		{
			get 
			{
				if (!_photometricInterpretationDefined)
				{
					if (_image.PhotometricInterpretation.Contains("MONOCHROME1"))
						_photometricInterpretation = PhotometricInterpretations.Monochrome1;
					else if (_image.PhotometricInterpretation.Contains("MONOCHROME2"))
						_photometricInterpretation = PhotometricInterpretations.Monochrome2;
					else if (_image.PhotometricInterpretation.Contains("PALETTE COLOR"))
						_photometricInterpretation = PhotometricInterpretations.PaletteColor;
					else if (_image.PhotometricInterpretation.Contains("RGB"))
						_photometricInterpretation = PhotometricInterpretations.Rgb;
					else if (_image.PhotometricInterpretation.Contains("YBR_FULL"))
						_photometricInterpretation = PhotometricInterpretations.YbrFull;
					else if (_image.PhotometricInterpretation.Contains("YBR_FULL_422"))
						_photometricInterpretation = PhotometricInterpretations.YbrFull422;
					else if (_image.PhotometricInterpretation.Contains("YBR_PARTIAL_422"))
						_photometricInterpretation = PhotometricInterpretations.YbrPartial422;
					else if (_image.PhotometricInterpretation.Contains("YBR_ICT"))
						_photometricInterpretation = PhotometricInterpretations.YbrIct;
					else if (_image.PhotometricInterpretation.Contains("YBR_RCT"))
						_photometricInterpretation = PhotometricInterpretations.YbrRct;
					else
						_photometricInterpretation = PhotometricInterpretations.Unknown;

					_photometricInterpretationDefined = true;
				}

				return _photometricInterpretation;
			}
		}

		public override byte[] GetPixelData()
		{
			// If it's a YBR image, convert it to RGB in place.  Having 
			// all non-indexed (i.e., non-palette) colour images in one format
			// makes image processing much easier.
			if (!_ybrConvertedToRgb &&
				this.PhotometricInterpretation == PhotometricInterpretations.YbrFull ||
				this.PhotometricInterpretation == PhotometricInterpretations.YbrFull422 ||
				this.PhotometricInterpretation == PhotometricInterpretations.YbrPartial422 ||
				this.PhotometricInterpretation == PhotometricInterpretations.YbrIct ||
				this.PhotometricInterpretation == PhotometricInterpretations.YbrRct)
			{
				_ybrConvertedToRgb = true;
				ColorSpaceConverter.YbrToRgb(this);
				_photometricInterpretationDefined = true;
				_photometricInterpretation = PhotometricInterpretations.Rgb;
			}

			return  _image.GetPixelData();
		}
	}
}
