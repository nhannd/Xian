using System;
using System.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client.Workflow
{

	// TODO add a description of the purpose of the settings group here
	[SettingsGroupDescription("")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class ReassignComponentSettings
	{
		private ReassignComponentSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}
