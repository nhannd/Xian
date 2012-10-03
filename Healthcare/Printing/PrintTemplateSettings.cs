#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Configuration;

namespace ClearCanvas.Healthcare.Printing
{
	/// <summary>
	/// Provides application settings for print template URLs.
	/// </summary>
	/// <remarks>
	/// This code is adapted from the Visual Studio generated template code;  the generated code has been removed from the project.  Additional 
	/// settings need to be manually added to this class.
	/// </remarks>
	[SettingsGroupDescription("Configures print template URLs.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal partial class PrintTemplateSettings : ApplicationSettingsBase
	{
		public PrintTemplateSettings()
		{
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("http://localhost/ris/print_templates/")]
		[global::System.Configuration.SettingsDescription("Specifies base URL for print templates.  Must be a localhost URL and must end with a trailing slash (e.g. http://localhost/ris/print_templates/")]
		public string BaseUrl
		{
			get { return (string) (this["BaseUrl"]); }
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("report/report.htm")]
		[global::System.Configuration.SettingsDescription("Specifies template for radiology reports.")]
		public string ReportTemplateUrl
		{
			get
			{
				return Combine(this.BaseUrl, (string)this["ReportTemplateUrl"]);
			}
		}

		[global::System.Configuration.ApplicationScopedSettingAttribute]
		[global::System.Configuration.DefaultSettingValueAttribute("downtime/downtime.htm")]
		[global::System.Configuration.SettingsDescription("Specifies template for downtime forms.")]
		public string DowntimeFormTemplateUrl
		{
			get
			{
				return Combine(this.BaseUrl, (string)this["DowntimeFormTemplateUrl"]);
			}
		}

		private static string Combine(string baseUrl, string relativeUrl)
		{
			return new Uri(new Uri(baseUrl), relativeUrl).ToString();
		}
	}
}
