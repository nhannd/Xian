using System;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Desktop.Configuration.Standard
{

	[SettingsGroupDescription("Provides a list of custom date formats the user can select from to set their own preferred date format")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class CustomDateFormatSettings
	{
		private CustomDateFormatSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		public System.Collections.Specialized.StringCollection AvailableFormats
		{
			get
			{
				return AvailableCustomFormats;
			}
		}
	}
}
