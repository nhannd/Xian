using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer
{
	[ExtensionOf(typeof(VoiLutFactoryExtensionPoint))]
	public class MinMaxPixelCalculatedLinearLut : CalculatedVoiLutLinear
	{
		private IndexedPixelData _pixelData;
		private IModalityLut _modalityLut;

		private double _windowWidth;
		private double _windowCenter;

		public MinMaxPixelCalculatedLinearLut()
		{
			_windowWidth = double.NaN;
			_windowCenter = double.NaN;
		}

		public MinMaxPixelCalculatedLinearLut(IPresentationImage presentationImage)
			:this()
		{
			Initialize(presentationImage);
		}

		public void Initialize(IPresentationImage presentationImage)
		{
			Platform.CheckForNullReference(presentationImage, "presentationImage");
			
			IImageGraphicProvider imageGraphicProvider = presentationImage as IImageGraphicProvider;
			Platform.CheckForInvalidCast(imageGraphicProvider, "presentationImage", typeof(IImageGraphicProvider).Name);

			IndexedPixelData pixelData = imageGraphicProvider.ImageGraphic.PixelData as IndexedPixelData;
			Platform.CheckForInvalidCast(pixelData, "imageGraphicProvider.ImageGraphic.PixelData", typeof(IndexedPixelData).Name);
			
			IModalityLutProvider modalityLutProvider = presentationImage as IModalityLutProvider;
			if (modalityLutProvider != null)
				_modalityLut = modalityLutProvider.ModalityLut;

			_pixelData = pixelData;
		}

		public override double  WindowWidth
		{
			get
			{
				if (double.IsNaN(_windowWidth))
					Calculate();

				return _windowWidth;
			}
		}

		public override double  WindowCenter
		{
			get
			{
				if (double.IsNaN(_windowCenter))
					Calculate();

				return _windowCenter;
			}
		}

		private void Calculate()
		{
			int minPixelValue, maxPixelValue;
			_pixelData.CalculateMinMaxPixelValue(out minPixelValue, out maxPixelValue);

			if (_modalityLut != null)
			{
				minPixelValue = _modalityLut[minPixelValue];
				maxPixelValue = _modalityLut[maxPixelValue];
			}

			_windowWidth = (maxPixelValue - minPixelValue);
			_windowCenter = _windowWidth / 2;
		}
	}
}
