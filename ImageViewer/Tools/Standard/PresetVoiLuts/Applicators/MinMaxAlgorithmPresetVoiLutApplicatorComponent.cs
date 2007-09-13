using System;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Imaging;

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
			return (presentationImage is IIndexedPixelDataProvider && presentationImage is IVoiLutProvider);
		}

		public override void Apply(IPresentationImage presentationImage)
		{
			if (!AppliesTo(presentationImage))
				throw new InvalidOperationException(SR.ExceptionInputPresentationImageNotSupported);

			IVoiLutManager manager = ((IVoiLutProvider)presentationImage).VoiLutManager;
			ILut currentLut = manager.GetLut();

			if (currentLut is MinMaxPixelCalculatedLinearLut)
				return;

			manager.InstallLut(new MinMaxPixelCalculatedLinearLut(((IIndexedPixelDataProvider)presentationImage).PixelData));
		}
	}
}