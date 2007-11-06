using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Configuration
{
	[SettingsGroupDescription("")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	public sealed class MonitorConfigurationSettings : ApplicationSettingsBase
	{
		private MonitorConfigurationSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		private static MonitorConfigurationSettings defaultInstance = ((MonitorConfigurationSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new MonitorConfigurationSettings())));

		public static MonitorConfigurationSettings Default
		{
			get
			{
				return defaultInstance;
			}
		}

		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("1")]
		public int WindowBehaviour
		{
			get
			{
				return ((int)(this["WindowBehaviour"]));
			}
			set
			{
				this["WindowBehaviour"] = value;
			}
		}
	}
}