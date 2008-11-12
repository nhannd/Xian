using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Desktop.Help {
	[SettingsGroupDescription("Configures the behaviour of application Help.")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	partial class HelpSettings {
		public HelpSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}
	}
}
