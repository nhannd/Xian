#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Reflection;
using System.Xml;
using SystemConfiguration = System.Configuration.Configuration;

namespace ClearCanvas.Common.Configuration
{
	/// <summary>
    /// Helper class that allows settings values for <see cref="ApplicationSettingsBase"/>-derived classes
    /// to be easily read/written to/from a <see cref="SystemConfiguration"/> object.
    /// </summary>
    public static class SystemConfigurationHelper
	{
		internal static SettingsSerializeAs GetSerializeAs(string serializeAs)
		{
			var converter = new EnumConverter(typeof(SettingsSerializeAs));
			if (!String.IsNullOrEmpty(serializeAs))
				return (SettingsSerializeAs)converter.ConvertFromInvariantString(serializeAs);

			return default(SettingsSerializeAs);
		}

		internal static string GetElementValue(XmlNode xmlNode, SettingsSerializeAs serializeAs)
		{
			return serializeAs == SettingsSerializeAs.Xml ? xmlNode.InnerXml : xmlNode.InnerText;
		}

		private static string GetElementValue(SettingElement element)
		{
			if (element.Value == null || element.Value.ValueXml == null)
				return null;

			return GetElementValue(element.Value.ValueXml, element.SerializeAs);
		}

		private static void SetElementValue(SettingElement element, string value)
		{
			XmlDocument temp = new XmlDocument();
			XmlNode valueXml = temp.CreateElement("value");

			if (element.SerializeAs == SettingsSerializeAs.String)
			{
				XmlElement escaper = temp.CreateElement("temp");
				escaper.InnerText = value ?? "";
				valueXml.InnerXml = escaper.InnerXml;
			}
			else
			{
				if (!String.IsNullOrEmpty(value))
				{
					XmlElement tempElement = temp.CreateElement("temp");
					tempElement.InnerXml = value;
					RemoveXmlDeclaration(tempElement);
					valueXml.InnerXml = tempElement.InnerXml;
				}
			}

			element.Value.ValueXml = valueXml;
		}

		private static void RemoveXmlDeclaration(XmlElement element)
		{
			XmlNode declaration = element.FirstChild;
			while (declaration != null && declaration.NodeType != XmlNodeType.XmlDeclaration)
				declaration = declaration.NextSibling;

			if (declaration != null)
				element.RemoveChild(declaration);
		}

		private static ClientSettingsSection CastToClientSection(ConfigurationSection section)
		{
			if (section is ClientSettingsSection)
				return (ClientSettingsSection)section;

			throw new NotSupportedException(String.Format(
				"The specified ConfigurationSection must be of Type ClientSettingsSection: {0}.", section.GetType().FullName));
		}

		private static SettingElement GetSettingElement(ClientSettingsSection clientSection, PropertyInfo property, bool create)
		{
			SettingElement element = clientSection.Settings.Get(property.Name);
			if (element == null && create)
			{
				element = new SettingElement(property.Name, SettingsClassMetaDataReader.GetSerializeAs(property));
				clientSection.Settings.Add(element);
			}

			return element;
		}

		//TODO (CR Sept 2010): instead of automatically storing all the defaults, should we 
		//just store what we're given and also delete stuff when we're given null?  Then
		//we could just return what's there in the "get" method rather than removing the defaults.
		private static ClientSettingsSection CreateDefaultSection(IEnumerable<PropertyInfo> properties)
		{
			var section = new ClientSettingsSection();
			section.SectionInformation.RequirePermission = false;

			foreach (PropertyInfo property in properties)
			{
				var element = new SettingElement(property.Name, SettingsClassMetaDataReader.GetSerializeAs(property));
				var valueElement = new SettingValueElement();
				element.Value = valueElement;
				
				string value = SettingsClassMetaDataReader.GetDefaultValue(property, false);
				SetElementValue(element, value);
				section.Settings.Add(element);
			}

			return section;
		}

		private static ClientSettingsSection AddDefaultSection(ConfigurationSectionGroup sectionGroup, string sectionName, IEnumerable<PropertyInfo> properties)
		{
			ClientSettingsSection newClientSection = CreateDefaultSection(properties);
			sectionGroup.Sections.Add(sectionName, newClientSection);
			if (sectionGroup.Name == ConfigurationSectionGroupPath.UserSettings.ToString())
				newClientSection.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
			return newClientSection;
		}

		private static bool UpdateSection(ClientSettingsSection clientSection, IEnumerable<PropertyInfo> properties, IDictionary<string, string> newValues)
		{
			bool modified = false;
			
			foreach (PropertyInfo property in properties)
			{
				string newValue;
				if (!newValues.TryGetValue(property.Name, out newValue))
					continue;

				SettingElement element = GetSettingElement(clientSection, property, false);
				string currentValue = element == null ? null : GetElementValue(element);

				string defaultValue = SettingsClassMetaDataReader.GetDefaultValue(property, false);

				bool newValueIsDefault = newValue == null || Equals(newValue, defaultValue);
				bool currentValueIsDefault = currentValue == null || Equals(currentValue, defaultValue);
				if (currentValueIsDefault && newValueIsDefault || Equals(currentValue, newValue))
					continue;

				element = GetSettingElement(clientSection, property, true);
				if (newValueIsDefault)
					newValue = defaultValue; //store defaults because it's convenient for editing.

				SetElementValue(element, newValue);
				modified = true;
			}

			return modified;
		}

		private static bool StoreSettings
			(
				ConfigurationSectionGroup sectionGroup,
				string sectionName,
				IEnumerable<PropertyInfo> properties,
				IDictionary<string, string> newValues
			)
		{
			bool newSection = false;
			ConfigurationSection section = sectionGroup.Sections[sectionName];
			if (section == null)
			{
				newSection = true;
				section = AddDefaultSection(sectionGroup, sectionName, properties);
			}

			bool modified = UpdateSection(CastToClientSection(section), properties, newValues);
			if (newSection && !modified)
				sectionGroup.Sections.Remove(sectionName);

			return modified;
		}

        private static Dictionary<string, string> GetSettingsValues(
			SystemConfiguration configuration, 
			ConfigurationSectionPath sectionPath, 
			ICollection<PropertyInfo> properties)
        {
            var values = new Dictionary<string, string>();
			if (properties.Count > 0)
			{
				var section = sectionPath.GetSection(configuration);
				if (section != null)
				{
					var clientSection = CastToClientSection(section);
					if (clientSection != null)
					{
						foreach (PropertyInfo property in properties)
						{
							SettingElement element = GetSettingElement(clientSection, property, false);
							if (element == null)
								continue;

							string currentValue = GetElementValue(element);
							if (currentValue == null) //not there means it's the default.
								continue;

							var defaultValue = SettingsClassMetaDataReader.GetDefaultValue(property, false);
							if (!Equals(currentValue, defaultValue))
								values[property.Name] = currentValue;
						}
					}
				}
			}

            return values;
        }

		private static ICollection<PropertyInfo> GetProperties(Type settingsClass, SettingScope scope)
		{
			return SettingsClassMetaDataReader.GetSettingsProperties(settingsClass, scope);
		}

		/// <summary>
		/// Gets only those settings values that are different from the defaults for the given settings group.
		/// </summary>
		/// <param name="configuration">the configuration where the values will be taken from</param>
		/// <param name="settingsClass">the settings class for which to get the values</param>
		/// <param name="settingScope">the scope of the settings for which to get the values</param>
		public static Dictionary<string, string> GetSettingsValues(SystemConfiguration configuration, Type settingsClass, SettingScope settingScope)
		{
			var properties = GetProperties(settingsClass, settingScope);
			var sectionPath = new ConfigurationSectionPath(settingsClass, settingScope);
			return GetSettingsValues(configuration, sectionPath, properties);
		}

		/// <summary>
        /// Gets only those settings values that are different from the defaults for the given settings group.
        /// </summary>
        /// <param name="configuration">the configuration where the values will be taken from</param>
		/// <param name="settingsClass">the settings class for which to get the values</param>
		public static Dictionary<string, string> GetSettingsValues(SystemConfiguration configuration, Type settingsClass)
		{
			var applicationScopedValues = GetSettingsValues(configuration, settingsClass, SettingScope.Application);
			var userScopedValues = GetSettingsValues(configuration, settingsClass, SettingScope.User);

			foreach (KeyValuePair<string, string> userScopedValue in userScopedValues)
                applicationScopedValues[userScopedValue.Key] = userScopedValue.Value;

            return applicationScopedValues;
		}

		/// <summary>
		/// Stores the settings values for a given settings class.
		/// </summary>
        /// <param name="configuration">the configuration where the values will be stored</param>
		/// <param name="settingsClass">the settings class for which to store the values</param>
		/// <param name="dirtyValues">contains the values to be stored</param>
        public static void PutSettingsValues(SystemConfiguration configuration, Type settingsClass, Dictionary<string, string> dirtyValues)
		{
			var applicationScopedProperties = GetProperties(settingsClass, SettingScope.Application);
			var userScopedProperties = GetProperties(settingsClass, SettingScope.User);

			bool modified = false;
			if (applicationScopedProperties.Count > 0)
			{
				var sectionPath = new ConfigurationSectionPath(settingsClass, SettingScope.Application);
				var group = sectionPath.GroupPath.GetSectionGroup(configuration, true);
				modified = StoreSettings(group, sectionPath.SectionName, applicationScopedProperties, dirtyValues);
			}
			if (userScopedProperties.Count > 0)
			{
				var sectionPath = new ConfigurationSectionPath(settingsClass, SettingScope.User);
				var group = sectionPath.GroupPath.GetSectionGroup(configuration, true);
				if (StoreSettings(group, sectionPath.SectionName, userScopedProperties, dirtyValues))
					modified = true;
			}

			if (modified)
				configuration.Save(ConfigurationSaveMode.Minimal, true);
		}

		public static void RemoveSettingsValues(SystemConfiguration configuration, Type settingsClass)
		{
			var sectionPath = new ConfigurationSectionPath(settingsClass, SettingScope.Application);
			ConfigurationSectionGroup group = configuration.GetSectionGroup(sectionPath.GroupPath);
			if (group != null)
				group.Sections.Remove(sectionPath.SectionName);

			sectionPath = new ConfigurationSectionPath(settingsClass, SettingScope.User);
			group = configuration.GetSectionGroup(sectionPath.GroupPath);
			if (group != null)
				group.Sections.Remove(sectionPath.SectionName);

			configuration.Save(ConfigurationSaveMode.Minimal, true);
		}

		public static SystemConfiguration GetExeConfiguration()
		{
			return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
		}

		public static SystemConfiguration GetExeConfiguration(string fileName)
		{
			var fileMap = new ExeConfigurationFileMap {ExeConfigFilename = fileName };
			return ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
		}
	}
}
