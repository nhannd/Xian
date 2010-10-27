#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Xml;
using System.Collections.Generic;

namespace ClearCanvas.Common.Configuration
{
	internal class ConfigurationFileReader
	{
		private readonly XmlDocument _document;

		public ConfigurationFileReader(string fileName)
			: this(new XmlDocument())
		{
			_document.Load(fileName);
		}

		public ConfigurationFileReader(XmlDocument document)
		{
			_document = document;
		}

		public IDictionary<string, string> GetSettingsValues(ConfigurationSectionPath path)
		{
			var values = new Dictionary<string, string>();

			if (_document.DocumentElement != null)
			{
				var element = _document.DocumentElement.SelectSingleNode(path) as XmlElement;
				if (element != null)
				{
					foreach (XmlElement setting in element.ChildNodes)
					{
						var nameAttribute = setting.Attributes["name"];
						if (nameAttribute == null)
							continue;

						var name = nameAttribute.Value;
						if (String.IsNullOrEmpty(name))
							continue;

						var valueNode = setting.SelectSingleNode("value");
						if (valueNode == null)
							continue;

						var serializeAsAttribute = setting.Attributes["serializeAs"];
						var serializeAsValue = serializeAsAttribute != null ? serializeAsAttribute.Value : String.Empty;
						var serializeAs = SystemConfigurationHelper.GetSerializeAs(serializeAsValue);
						values.Add(name, SystemConfigurationHelper.GetElementValue(valueNode, serializeAs));
					}
				}
			}

			return values;
		}

		public IDictionary<string, string> GetSettingsValues(Type settingsClass, SettingScope scope)
		{
			return GetSettingsValues(new ConfigurationSectionPath(settingsClass, scope));
		}

		public IDictionary<string, string> GetSettingsValues(Type settingsClass)
		{
			var appSettings = GetSettingsValues(settingsClass, SettingScope.Application);
			var userSettings = GetSettingsValues(settingsClass, SettingScope.User);
			foreach (var setting in userSettings)
				appSettings[setting.Key] = setting.Value;

			return appSettings;
		}
	}
}
