using System.ComponentModel;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Applicators
{
	public enum EditContext
	{
		Add = 0,
		Edit
	}

	public interface IPresetVoiLutApplicatorComponent : IApplicationComponent, INotifyPropertyChanged, IDataErrorInfo
	{
		IPresetVoiLutApplicator GetApplicator();

		EditContext EditContext { get; set; }
		bool Valid { get; }
	}

	public interface IPresetVoiLutApplicator
	{
		string Name { get; }
		string Description { get; }

		IPresetVoiLutApplicatorFactory SourceFactory { get; }

		bool AppliesTo(IPresentationImage presentationImage);
		void Apply(IPresentationImage image);

		PresetVoiLutConfiguration GetConfiguration();
	}
	
	public interface IPresetVoiLutApplicatorFactory
	{
		string Name { get; }
		string Description { get; }
		
		bool CanCreateMultiple { get; }

		IPresetVoiLutApplicator Create(PresetVoiLutConfiguration configuration);
		IPresetVoiLutApplicatorComponent GetEditComponent(PresetVoiLutConfiguration configuration);
	}

	public sealed class PresetVoiLutApplicatorFactoryExtensionPoint : ExtensionPoint<IPresetVoiLutApplicatorFactory>
	{
	}
}
