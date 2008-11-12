using System;
using System.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Workflow
{
	[SettingsGroupDescription("Configures behaviour of Reassign component.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class ReassignComponentSettings
	{
		private ReassignComponentSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}
