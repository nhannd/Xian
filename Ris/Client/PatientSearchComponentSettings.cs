using System;
using System.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{

	[SettingsGroupDescription("Configures behaviour of the Patient Search component.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class PatientSearchComponentSettings
	{
		private PatientSearchComponentSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}
