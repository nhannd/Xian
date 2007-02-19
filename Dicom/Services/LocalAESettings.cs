using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Dicom.Services
{
	[SettingsGroupDescription("Defines the Local AE Settings for the application")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class LocalAESettings
	{
		private LocalAESettings()
		{
			ApplicationSettingsRegister.Instance.RegisterInstance(this);
		}

		~LocalAESettings()
		{
			ApplicationSettingsRegister.Instance.UnregisterInstance(this);
		}
	}
}
