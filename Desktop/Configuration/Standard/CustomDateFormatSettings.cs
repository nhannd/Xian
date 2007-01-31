using System;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace ClearCanvas.Desktop.Configuration.Standard
{

	[SettingsGroupDescription("Provides a list of custom date formats the user can select from to set their own preferred date format")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class CustomDateFormatSettings
	{
		public CustomDateFormatSettings()
		{
		}

		public System.Collections.Specialized.StringCollection AvailableCustomFormats
		{
			get
			{
				//we have to parse the xml each time this property is accessed because we don't want to cache the setting anywhere.
				StringReader stringReader = new StringReader(this.AvailableCustomFormatsXml);
				XmlTextReader reader = new XmlTextReader(stringReader);

				XmlSerializer serializer = new XmlSerializer(typeof(System.Collections.Specialized.StringCollection));
				System.Collections.Specialized.StringCollection collection = (System.Collections.Specialized.StringCollection)serializer.Deserialize(reader);
				return collection;
			}
		}
	}
}
