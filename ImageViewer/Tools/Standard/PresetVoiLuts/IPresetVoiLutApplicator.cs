
namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public interface IPresetVoiLutApplicator
	{
		string Name { get; }
		string Description { get; }

		IPresetVoiLutApplicatorFactory SourceFactory { get; }

		bool AppliesTo(IPresentationImage presentationImage);
		void Apply(IPresentationImage image);

		PresetVoiLutConfiguration GetConfiguration();
	}
}
