using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Graphics
{
	/// <summary>
	/// A <see cref="GrayscaleImageGraphic"/> with an associated <see cref="ImageSop"/>.
	/// </summary>
	public class StandardGrayscaleImageGraphic : GrayscaleImageGraphic
	{
		private ImageSop _imageSop;

		/// <summary>
		/// Instantiates a new instance of <see cref="StandardGrayscaleImageGraphic"/>
		/// with the specified <see cref="ImageSop"/>.
		/// </summary>
		/// <param name="imageSop"></param>
		public StandardGrayscaleImageGraphic(ImageSop imageSop) : 
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
			imageSop.RescaleSlope,
			imageSop.RescaleIntercept,
			null)
		{
			Platform.CheckForNullReference(imageSop, "image");

			_imageSop = imageSop;

			SetWindowLevel();
		}

		/// <summary>
		/// Gets the associated <see cref="ImageSop"/>.
		/// </summary>
		public ImageSop ImageSop
		{
			get { return _imageSop; }
		}

		/// <summary>
		/// Gets the pixel data from the associated <see cref="ImageSop"/>.
		/// </summary>
		protected override byte[] PixelDataRaw
		{
			get
			{
				return _imageSop.PixelData;
			}
		}

		private void SetWindowLevel()
		{
			if (this.VoiLUTLinear == null)
				return;

			double windowWidth = double.NaN;
			double windowCenter = double.NaN;

			Window[] windows = _imageSop.WindowCenterAndWidth;

			if (windows != null)
			{
				windowWidth = windows[0].Width;
				windowCenter = windows[0].Center;
			}

			//Window Width must be non-zero according to DICOM.
			//Otherwise, we want to do something simple (pick 2^BitsStored).
			if (windowWidth == 0 || double.IsNaN(windowWidth))
				windowWidth = 1 << ((int)this.BitsStored);

			//If Window Center is invalid, calculate a value based on the Window Width.
			if (double.IsNaN(windowCenter))
			{
				if (this.PixelRepresentation == 0)
					windowCenter = ((int)windowWidth) >> 1;
				else
					windowCenter = 0;
			}

			this.VoiLUTLinear.WindowCenter = windowCenter;
			this.VoiLUTLinear.WindowWidth = windowWidth;
		}
	}
}
