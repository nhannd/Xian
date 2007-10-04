using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{

	// TODO add a description of the purpose of the settings group here
	[SettingsGroupDescription("")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class DicomExplorerConfigurationSettings
	{
		private DicomExplorerConfigurationSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}
