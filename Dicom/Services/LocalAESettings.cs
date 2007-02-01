using System;
using System.Configuration;

namespace ClearCanvas.Dicom.Services
{
	[SettingsGroupDescription("Defines the Local AE Settings for the application")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class LocalAESettings
	{
		public LocalAESettings()
		{
		}
	}
}
