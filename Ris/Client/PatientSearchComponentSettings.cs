using System;
using System.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{

	// TODO add a description of the purpose of the settings group here
	[SettingsGroupDescription("")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class PatientSearchComponentSettings
	{
		private PatientSearchComponentSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}
