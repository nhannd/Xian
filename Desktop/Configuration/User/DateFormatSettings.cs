using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.Configuration;

namespace ClearCanvas.Desktop.Configuration.User
{
	public static class DateFormatSettings
	{
		private static DateFormatUserSettings _userSettings;
		private static DateFormatConfiguration _configuration;

		private static void Initialize()
		{
			if (_userSettings == null)
				_userSettings = new DateFormatUserSettings();

			if (_configuration == null)
				_configuration = new DateFormatConfiguration();
		}

		public static DateFormatConfiguration Configuration
		{
			get
			{
				Initialize();

				_configuration.SelectedCustomDateFormat = _userSettings.SelectedCustomDateFormat;
				_configuration.DateFormatSelection = _userSettings.DateFormatSelection;

				return _configuration;
			}
			set
			{
				Initialize();
				
				_userSettings.SelectedCustomDateFormat = value.SelectedCustomDateFormat;
				_userSettings.DateFormatSelection = value.DateFormatSelection;

				_userSettings.Save();
			}
		}

		public static string DateFormat
		{ 
			get 
			{
				Initialize();
				return _configuration.DateFormat;
			}
		}
	}

	internal class DateFormatUserSettings : ExtensionSettings
	{
		public DateFormatUserSettings()
		{
		}

		[UserScopedSettingAttribute()]
		[DefaultSettingValueAttribute("SYSTEMSHORT")]
		public DateFormatConfiguration.DateFormatSelections DateFormatSelection
		{
			get { return (DateFormatConfiguration.DateFormatSelections)this["DateFormatSelection"]; }
			set { this["DateFormatSelection"] = value; }
		}

		[UserScopedSettingAttribute()]
		[DefaultSettingValueAttribute("")]
		public string SelectedCustomDateFormat
		{
			get { return (string)this["SelectedCustomDateFormat"]; }
			set { this["SelectedCustomDateFormat"] = value; }
		}
	}
}
