using System;
using ClearCanvas.ImageViewer.Imaging;
using ClearCanvas.ImageViewer.Graphics;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators
{
	public sealed class MinMaxAlgorithmPresetVoiLutApplicatorComponent : DefaultPresetVoiLutApplicatorComponent
	{
		public MinMaxAlgorithmPresetVoiLutApplicatorComponent()
		{
		}

		public override string Name
		{
			get { return SR.MinMaxAlgorithmPresetVoiLutApplicatorComponentName; }
		}

		public override string Description
		{
			get { return SR.MinMaxAlgorithmPresetVoiLutApplicatorComponentDescription; }
		}

		public override bool AppliesTo(IPresentationImage presentationImage)
		{
			return (presentationImage is IVoiLutProvider &&
			        presentationImage is IImageGraphicProvider &&
			        ((IImageGraphicProvider) presentationImage).ImageGraphic.PixelData is IndexedPixelData);
		}

		public override void Apply(IPresentationImage presentationImage)
		{
			if (!AppliesTo(presentationImage))
				throw new InvalidOperationException(SR.ExceptionInputPresentationImageNotSupported);

			IVoiLutManager manager = ((IVoiLutProvider)presentationImage).VoiLutManager;
			IComposableLut currentLut = manager.GetLut();

			if (currentLut is MinMaxPixelCalculatedLinearLut)
				return;

			IndexedPixelData pixelData = (IndexedPixelData)((IImageGraphicProvider) presentationImage).ImageGraphic.PixelData;

			IModalityLutProvider modalityLutProvider = presentationImage as IModalityLutProvider;
			if (modalityLutProvider != null)
				manager.InstallLut(new MinMaxPixelCalculatedLinearLut(pixelData, modalityLutProvider.ModalityLut));
			else
				manager.InstallLut(new MinMaxPixelCalculatedLinearLut(pixelData));
		}
	}
}