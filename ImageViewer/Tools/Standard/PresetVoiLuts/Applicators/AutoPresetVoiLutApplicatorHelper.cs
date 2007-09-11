using System;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Luts;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators
{
	internal static class AutoPresetVoiLutApplicatorHelper
	{
		public static bool AppliesTo(IPresentationImage presentationImage)
		{
			if (!(presentationImage is IVoiLutProvider))
				return false;

			IImageSopProvider sopProvider = presentationImage as IImageSopProvider;
			if (sopProvider != null && sopProvider.ImageSop.WindowCenterAndWidth.Length > 0)
				return true;

			IIndexedPixelDataProvider pixelDataProvider = presentationImage as IIndexedPixelDataProvider;
			return pixelDataProvider != null;
		}

		public static void AutoApplyLut(IPresentationImage presentationImage)
		{
			if (!AppliesTo(presentationImage))
				throw new InvalidOperationException("The input presentation image is not supported");

			IVoiLutManager manager = ((IVoiLutProvider)presentationImage).VoiLutManager;
			ILut currentLut = manager.GetLut();

			IImageSopProvider sopProvider = presentationImage as IImageSopProvider;
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

			IIndexedPixelDataProvider pixelDataProvider = presentationImage as IIndexedPixelDataProvider;
			if (pixelDataProvider != null)
			{
				IModalityLutProvider modalityLutProvider = presentationImage as IModalityLutProvider;
				if (modalityLutProvider != null)
					manager.InstallLut(new MinMaxPixelCalculatedLinearLut(pixelDataProvider.PixelData, modalityLutProvider.ModalityLut));
				else
					manager.InstallLut(new MinMaxPixelCalculatedLinearLut(pixelDataProvider.PixelData));
			}
		}

		public static ILut GetInitialLut(IPresentationImage image)
		{
			if (AppliesTo(image))
			{
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
			}

			return null;
		}
	}
}
