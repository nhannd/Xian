using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Rendering;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A colour <see cref="ImageGraphic"/> with an associated <see cref="ImageSop"/>.
	/// </summary>
	public class StandardColorImageGraphic : ImageGraphic
	{
		private ImageSop _imageSop;
		private bool _ybrConvertedToRgb = false;

		/// <summary>
		/// Instantiates a new instance of <see cref="StandardColorImageGraphic"/>
		/// with the specified <see cref="ImageSop"/>.
		/// </summary>
		/// <param name="imageSop"></param>
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
			if (!this.IsColor && 
				base.PhotometricInterpretation != PhotometricInterpretation.PaletteColor)
				throw new InvalidOperationException("Image must be non-indexed colour");

			_imageSop = imageSop;
		}

		/// <summary>
		/// Gets the associated <see cref="ImageSop"/>.
		/// </summary>
		public ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		/// <summary>
		/// Gets a value indicating whether the image has been converted from
		/// any YBR photometric interpretation to RGB.
		/// </summary>
		public bool YbrConvertedToRgb
		{
			get { return _ybrConvertedToRgb; }
		}

		/// <summary>
		/// Gets the photometric interpretation.
		/// </summary>
		/// <value>
		/// Always RGB.
		/// </value>
		/// <remarks>
		/// To make it easier for the implementor of <see cref="IRenderer"/>, <i>all</i>
		/// non-indexed colour images are RGB.  If an image is natively YBR_XXX, it is automatically
		/// converted to RGB and the <see cref="YbrConvertedToRgb"/> flag is set
		/// to <b>true</b>.
		/// </remarks>
		public override PhotometricInterpretation PhotometricInterpretation
		{
			get
			{
				return PhotometricInterpretation.Rgb;
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

		/// <summary>
		/// Gets the pixel data from the associated <see cref="ImageSop"/>.
		/// </summary>
		/// <remarks>
		/// If the photometric interpretation is originally YBR_XXX, accessing this
		/// property for the first time will result in a one time conversion of the
		/// pixel data to RGB.
		/// </remarks>
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
