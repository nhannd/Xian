using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer
{
	public class GrayscalePresentationImage 
		: BasicPresentationImage, 
		IVoiLutProvider, 
		IVoiLutLinearProvider,
		IPresentationLutProvider
	{
		private IVoiLutManager _voiLutManager;
		private IPresentationLutManager _presentationLutManager;

		public GrayscalePresentationImage(int rows, int columns)
			: base(new GrayscaleImageGraphic(rows, columns))
		{
		}

		public GrayscalePresentationImage(
			int rows,
			int columns,
			int bitsAllocated,
			int bitsStored,
			int highBit,
			bool isSigned,
			bool inverted,
			double rescaleSlope,
			double rescaleIntercept,
			double pixelSpacingX,
			double pixelSpacingY,
			double pixelAspectRatioX,
			double pixelAspectRatioY,
			PixelDataGetter pixelDataGetter)
			: base(new GrayscaleImageGraphic(
			       	rows,
			       	columns,
			       	bitsAllocated,
			       	bitsStored,
			       	highBit,
			       	isSigned,
			       	inverted,
			       	rescaleSlope,
			       	rescaleIntercept,
			       	pixelDataGetter),
			       pixelSpacingX,
			       pixelSpacingY,
			       pixelAspectRatioX,
			       pixelAspectRatioY)
		{

		}


		/// <summary>
		/// Creates a clone of the <see cref="StandardPresentationImage"/>.
		/// </summary>
		/// <returns></returns>
		public override IPresentationImage Clone()
		{

			// TODO:
			return null;

			//IPresentationImage image = new BasicPresentationImage();
			//image.Uid = this.Uid;

			//return image;
		}

		#region IVoiLutProvider Members

		public IVoiLutManager VoiLutManager
		{
			get 
			{
				if (_voiLutManager == null)
					_voiLutManager = new VoiLutManager(this.ImageGraphic as GrayscaleImageGraphic);

				return _voiLutManager;
			}
		}

		#endregion

		#region IPresentationLutProvider Members

		public IPresentationLutManager PresentationLutManager
		{
			get
			{
				if (_presentationLutManager == null)
					_presentationLutManager = new PresentationLutManager(this.ImageGraphic as GrayscaleImageGraphic);

				return _presentationLutManager;
			}
		}

		#endregion


		#region IVoiLutLinearProvider Members

		public IVoiLutLinear VoiLutLinear
		{
			get { return (this.ImageGraphic as GrayscaleImageGraphic).VoiLutLinear ; }
		}

		#endregion
	}
}
