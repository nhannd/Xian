
namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	//[ExtensionOf(typeof(PresetVoiLutApplicatorFactoryExtensionPoint))]
	//public sealed class AutoPresetVoiLutApplicatorFactory : PresetVoiLutApplicatorFactory<LinearPresetVoiLutApplicator>
	//{
	//    public AutoPresetVoiLutApplicatorFactory()
	//    {
	//    }

	//    public override string Name
	//    {
	//        get { return "Auto"; }
	//    }
	//}

	internal sealed class AutoPresetVoiLutApplicator : PresetVoiLutApplicator
	{
		public AutoPresetVoiLutApplicator()
		{
		}

		public override string Name
		{
			get { return "Auto"; }
		}

		public override string Description
		{
			get { return this.Name; }
		}

		public override void Validate()
		{
		}

		public override bool AppliesTo(IPresentationImage presentationImage)
		{
			return AutoVoiLutApplicator.AppliesTo(presentationImage);
		}

		public override void Apply(IPresentationImage image)
		{
			// TODO: Later, this should be implemented as a 'true applicator' provided by the factory above (commented out).
			// Then, essentially everything can be a lut applicator (auto, min/max algorithm, etc).

			AutoVoiLutApplicator.AutoApplyLut(image);
		}
	}
}
