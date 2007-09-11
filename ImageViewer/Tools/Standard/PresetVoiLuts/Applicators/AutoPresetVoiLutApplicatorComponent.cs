namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators
{
	public sealed class AutoPresetVoiLutApplicatorComponent : DefaultPresetVoiLutApplicatorComponent
	{
		public AutoPresetVoiLutApplicatorComponent()
		{
		}

		public override string Name
		{
			get { return SR.AutoPresetVoiLutApplicatorName; }
		}

		public override string Description
		{
			get { return SR.AutoPresetVoiLutApplicatorDescription; }
		}

		public override bool AppliesTo(IPresentationImage presentationImage)
		{
			return AutoPresetVoiLutApplicatorHelper.AppliesTo(presentationImage);
		}

		public override void Apply(IPresentationImage presentationImage)
		{
			// TODO: Later, when we've enabled all the factories, we need to change this functionality so it 
			// is purely 'auto'; no min/max algorithm, as it is currently.

			AutoPresetVoiLutApplicatorHelper.AutoApplyLut(presentationImage);
		}
	}
}
