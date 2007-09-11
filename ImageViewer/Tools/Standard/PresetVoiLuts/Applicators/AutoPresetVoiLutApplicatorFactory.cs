using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators
{
	//[ExtensionOf(typeof(PresetVoiLutApplicatorFactoryExtensionPoint))]
	public sealed class AutoPresetVoiLutApplicatorFactory : PresetVoiLutApplicatorFactory<AutoPresetVoiLutApplicatorComponent>
	{
		internal static readonly string FactoryName = "Auto";

		public AutoPresetVoiLutApplicatorFactory()
		{
		}

		public override string Name
		{
			get { return FactoryName; }
		}

		public override string Description
		{
			get { return SR.AutoPresetVoiLutApplicatorFactoryDescription; }
		}
	}
}
