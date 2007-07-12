using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Rendering;
using System.Drawing;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A colour <see cref="ImageGraphic"/> with an associated <see cref="ImageSop"/>.
	/// </summary>
	public unsafe class StandardColorImageGraphic : ColorImageGraphic
	{
		#region Private fields

		private bool _converted;
		private ImageSop _imageSop;

		#endregion

		/// <summary>
		/// Instantiates a new instance of <see cref="StandardColorImageGraphic"/>
		/// with the specified <see cref="ImageSop"/>.
		/// </summary>
		/// <param name="imageSop"></param>
		public StandardColorImageGraphic(ImageSop imageSop)
			: base(imageSop.Rows,
				   imageSop.Columns,
				   new PixelDataGetter(imageSop.GetNormalizedPixelData))
		{
			if (imageSop.PhotometricInterpretation != PhotometricInterpretation.Rgb &&
				!IsYbr(imageSop.PhotometricInterpretation))
				throw new InvalidOperationException("Image must be RGB or YBR_XXX.");

			_imageSop = imageSop;
		}

		/// <summary>
		/// Gets the associated <see cref="ImageSop"/>.
		/// </summary>
		public ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		private bool IsYbr(PhotometricInterpretation photometricInterpretation)
		{
			return photometricInterpretation == PhotometricInterpretation.YbrFull ||
				photometricInterpretation == PhotometricInterpretation.YbrFull422 ||
				photometricInterpretation == PhotometricInterpretation.YbrPartial422 ||
				photometricInterpretation == PhotometricInterpretation.YbrIct ||
				photometricInterpretation == PhotometricInterpretation.YbrRct;
		}
	}
}
