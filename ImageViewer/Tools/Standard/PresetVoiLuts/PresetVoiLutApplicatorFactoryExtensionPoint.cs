using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public interface IPresetVoiLutApplicatorFactory
	{
		string Name { get; }
		IPresetVoiLutApplicator Create(PresetVoiLutConfiguration configuration);
	}

	public sealed class PresetVoiLutApplicatorFactoryExtensionPoint : ExtensionPoint<IPresetVoiLutApplicatorFactory>
	{
	}
}
