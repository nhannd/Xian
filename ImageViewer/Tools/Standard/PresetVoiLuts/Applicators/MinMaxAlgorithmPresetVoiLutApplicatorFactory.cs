using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators
{
	//[ExtensionOf(typeof(PresetVoiLutApplicatorFactoryExtensionPoint))]
	public sealed class MinMaxAlgorithmPresetVoiLutApplicatorFactory : PresetVoiLutApplicatorFactory<MinMaxAlgorithmPresetVoiLutApplicatorComponent>
	{
		internal static readonly string FactoryName = "Min/Max Algorithm";

		public MinMaxAlgorithmPresetVoiLutApplicatorFactory()
		{
		}

		public override string Name
		{
			get { return FactoryName; }
		}

		public override string Description
		{
			get { return SR.MinMaxAlgorithmPresetVoiLutApplicatorFactoryDescription; }
		}
	}
}