#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Configuration
{
	public static class ViewerLaunchSettings
	{
		public static WindowBehaviour WindowBehaviour
		{
			get
			{
				return (WindowBehaviour)MonitorConfigurationSettings.Default.WindowBehaviour;
			}
		}

		public static bool AllowEmptyViewer
		{
			get { return MonitorConfigurationSettings.Default.AllowEmptyViewer; }
		}
	}

	[SettingsGroupDescription("Settings related to monitor configuration and multiple windows.")]
	[SettingsProvider(typeof(LocalFileSettingsProvider))]
	internal sealed class MonitorConfigurationSettings : ApplicationSettingsBase
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
		[System.Configuration.SettingsDescription("Specifies whether viewers should be launched in the main window, or in a separate dedicated window.")]
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

		[global::System.Configuration.ApplicationScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("False")]
		[System.Configuration.SettingsDescription("Specifies whether or not to allow a viewer to open when there are no images to display.")]
		public bool AllowEmptyViewer
		{
			get
			{
				return ((bool)(this["AllowEmptyViewer"]));
			}
			set
			{
				this["AllowEmptyViewer"] = value;
			}
		}
	}
}