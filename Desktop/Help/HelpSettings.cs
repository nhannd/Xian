using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Desktop.Help {
	[SettingsGroupDescription("Stores the action model settings for each user")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	partial class HelpSettings {
		public HelpSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}
