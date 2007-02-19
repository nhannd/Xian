using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;

namespace ClearCanvas.ImageViewer.Graphics
{
	public class StandardColorImageGraphic : ImageGraphic
	{
		private ImageSop _imageSop;
		private bool _ybrConvertedToRgb = false;

		public StandardColorImageGraphic(ImageSop imageSop) :
			base(
			imageSop.Rows,
			imageSop.Columns,
			imageSop.BitsAllocated,
			imageSop.BitsStored,
			imageSop.HighBit,
			imageSop.SamplesPerPixel,
			imageSop.PixelRepresentation,
			imageSop.PlanarConfiguration,
			imageSop.PhotometricInterpretation,
			null)
		{
			_imageSop = imageSop;
		}

		public bool YbrConvertedToRgb
		{
			get { return _ybrConvertedToRgb; }
		}

		public override PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
				if (this.IsYbr)
					return PhotometricInterpretation.Rgb;
				else
					return base.PhotometricInterpretation;
			}
		}

		private bool IsYbr
		{
			get
			{
				return base.PhotometricInterpretation == PhotometricInterpretation.YbrFull ||
				base.PhotometricInterpretation == PhotometricInterpretation.YbrFull422 ||
				base.PhotometricInterpretation == PhotometricInterpretation.YbrPartial422 ||
				base.PhotometricInterpretation == PhotometricInterpretation.YbrIct ||
				base.PhotometricInterpretation == PhotometricInterpretation.YbrRct;
			}
		}

		protected override byte[] PixelDataRaw
		{
			get
			{
				// If it's a YBR image, convert it to RGB in place.  Having 
				// all non-indexed (i.e., non-palette) colour images in one format
				// makes image processing much easier.
				if (!_ybrConvertedToRgb && this.IsYbr)
				{
					_ybrConvertedToRgb = true;
					ColorSpaceConverter.YbrToRgb(this);
				}

				return _imageSop.PixelData;
			}
		}
	}
}
