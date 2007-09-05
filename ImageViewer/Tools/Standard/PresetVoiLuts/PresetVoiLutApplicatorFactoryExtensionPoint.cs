using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System;
using System.ComponentModel;

namespace ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts
{
	public interface IEditPresetVoiLutApplicationComponent : IApplicationComponent, INotifyPropertyChanged
	{
		IPresetVoiLutApplicator GetApplicator();

		bool Valid { get; }
	}

	public interface IPresetVoiLutApplicatorFactory
	{
		string Name { get; }
		IPresetVoiLutApplicator Create(PresetVoiLutConfiguration configuration);
		IEditPresetVoiLutApplicationComponent GetEditComponent(PresetVoiLutConfiguration configuration);
	}

	public sealed class PresetVoiLutApplicatorFactoryExtensionPoint : ExtensionPoint<IPresetVoiLutApplicatorFactory>
	{
	}
}
