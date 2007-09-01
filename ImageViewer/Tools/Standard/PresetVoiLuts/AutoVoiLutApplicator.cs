
using System;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	internal static class AutoVoiLutApplicator
	{
		public static bool AppliesTo(IPresentationImage image)
		{
			if (!(image is IVoiLutProvider))
				return false;

			IImageSopProvider sopProvider = image as IImageSopProvider;
			if (sopProvider != null && sopProvider.ImageSop.WindowCenterAndWidth.Length > 0)
				return true;

			IIndexedPixelDataProvider pixelDataProvider = image as IIndexedPixelDataProvider;
			return pixelDataProvider != null;
		}

		public static void AutoApplyLut(IPresentationImage image)
		{
			if (!AppliesTo(image))
				throw new InvalidOperationException("The input presentation image is not supported");

			IVoiLutManager manager = ((IVoiLutProvider)image).VoiLutManager;
			ILut currentLut = manager.GetLut();

			IImageSopProvider sopProvider = image as IImageSopProvider;
			if (sopProvider != null)
			{
				if (sopProvider.ImageSop.WindowCenterAndWidth.Length > 0)
				{
					if (currentLut is AutoVoiLutLinear)
						((AutoVoiLutLinear)currentLut).ApplyNext();
					else
						manager.InstallLut(new AutoVoiLutLinear(sopProvider.ImageSop));

					return;
				}
			}

			if (currentLut is MinMaxPixelCalculatedLinearLut)
				return;

			IIndexedPixelDataProvider pixelDataProvider = image as IIndexedPixelDataProvider;
			if (pixelDataProvider != null)
			{
				IModalityLutProvider modalityLutProvider = image as IModalityLutProvider;
				if (modalityLutProvider != null)
					manager.InstallLut(new MinMaxPixelCalculatedLinearLut(pixelDataProvider.PixelData, modalityLutProvider.ModalityLut));
				else
					manager.InstallLut(new MinMaxPixelCalculatedLinearLut(pixelDataProvider.PixelData));
			}
		}

		public static ILut GetInitialLut(IPresentationImage image)
		{
			if (!AppliesTo(image))
				return null;

			IImageSopProvider sopProvider = image as IImageSopProvider;
			if (sopProvider != null)
			{
				if (sopProvider.ImageSop.WindowCenterAndWidth.Length > 0)
					return new AutoVoiLutLinear(sopProvider.ImageSop);
			}

			IIndexedPixelDataProvider pixelDataProvider = image as IIndexedPixelDataProvider;
			if (pixelDataProvider != null)
			{
				IModalityLutProvider modalityLutProvider = image as IModalityLutProvider;
				if (modalityLutProvider != null)
					return new MinMaxPixelCalculatedLinearLut(pixelDataProvider.PixelData, modalityLutProvider.ModalityLut);
				else
					return new MinMaxPixelCalculatedLinearLut(pixelDataProvider.PixelData);
			}

			return null;
		}
	}
}
