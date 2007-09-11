using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators
{
	[AllowMultiplePresetVoiLutApplicators]
	[ExtensionOf(typeof(PresetVoiLutApplicatorFactoryExtensionPoint))]
	public sealed class LinearPresetVoiLutApplicatorFactory : PresetVoiLutApplicatorFactory<LinearPresetVoiLutApplicatorComponent>
	{
		internal static readonly string FactoryName = "Linear Preset";

		public LinearPresetVoiLutApplicatorFactory()
		{
		}

		public override string Name
		{
			get { return FactoryName; }
		}

		public override string Description
		{
			get { return SR.LinearPresetVoiLutApplicatorFactoryDescription; }
		}
	}
}