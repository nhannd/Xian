using System;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Graphics;
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

			IImageGraphicProvider graphicProvider = presentationImage as IImageGraphicProvider;
			if (graphicProvider != null)
				return graphicProvider.ImageGraphic.PixelData is IndexedPixelData;

			return false;
		}

		public static void AutoApplyLut(IPresentationImage presentationImage)
		{
			if (!AppliesTo(presentationImage))
				throw new InvalidOperationException(SR.ExceptionInputPresentationImageNotSupported);

			IVoiLutManager manager = ((IVoiLutProvider)presentationImage).VoiLutManager;
			IComposableLut currentLut = manager.GetLut();

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

			IndexedPixelData pixelData = (IndexedPixelData)((IImageGraphicProvider)presentationImage).ImageGraphic.PixelData;

			IModalityLutProvider modalityLutProvider = presentationImage as IModalityLutProvider;
			if (modalityLutProvider != null)
				manager.InstallLut(new MinMaxPixelCalculatedLinearLut(pixelData, modalityLutProvider.ModalityLut));
			else
				manager.InstallLut(new MinMaxPixelCalculatedLinearLut(pixelData));
		}

		public static IComposableLut GetInitialLut(IPresentationImage presentationImage)
		{
			if (!AppliesTo(presentationImage))
				return null;

			IImageSopProvider sopProvider = presentationImage as IImageSopProvider;
			if (sopProvider != null)
			{
				if (sopProvider.ImageSop.WindowCenterAndWidth.Length > 0)
					return new AutoVoiLutLinear(sopProvider.ImageSop);
			}

			IndexedPixelData pixelData = (IndexedPixelData)((IImageGraphicProvider)presentationImage).ImageGraphic.PixelData;

			IModalityLutProvider modalityLutProvider = presentationImage as IModalityLutProvider;
			if (modalityLutProvider != null)
				return new MinMaxPixelCalculatedLinearLut(pixelData, modalityLutProvider.ModalityLut);
			else
				return new MinMaxPixelCalculatedLinearLut(pixelData);
		}
	}
}
