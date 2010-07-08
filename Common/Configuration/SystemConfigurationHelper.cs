#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
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
		internal static void RemoveXmlDeclaration(XmlElement element)
		{
			XmlNode declaration = element.FirstChild;
			while (declaration != null && declaration.NodeType != XmlNodeType.XmlDeclaration)
				declaration = declaration.NextSibling;

			if (declaration != null)
				element.RemoveChild(declaration);
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

        private static string GetElementValue(SettingElement element)
        {
			if (element.Value == null || element.Value.ValueXml == null)
				return null;

        	return element.SerializeAs == SettingsSerializeAs.Xml ? 
				element.Value.ValueXml.InnerXml : element.Value.ValueXml.InnerText;
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

		private static ClientSettingsSection CreateDefaultSection(IEnumerable<PropertyInfo> properties)
		{
			var section = new ClientSettingsSection();
			section.SectionInformation.RequirePermission = false;

			foreach (PropertyInfo property in properties)
			{
				var element = new SettingElement(property.Name, SettingsClassMetaDataReader.GetSerializeAs(property));
				var valueElement = new SettingValueElement();
				element.Value = valueElement;
				
				string value = SettingsClassMetaDataReader.GetDefaultValue(property);
				SetElementValue(element, value);
				section.Settings.Add(element);
			}

			return section;
		}

		private static ClientSettingsSection AddDefaultSection(ConfigurationSectionGroup sectionGroup, string sectionName, IEnumerable<PropertyInfo> properties)
		{
			ClientSettingsSection newClientSection = CreateDefaultSection(properties);
			sectionGroup.Sections.Add(sectionName, newClientSection);
			if (sectionGroup.Name == ConfigurationSectionGroupPath.UserSettings)
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

				string defaultValueUntranslated = SettingsClassMetaDataReader.GetDefaultValue(property, false);
				string defaultValueTranslated = SettingsClassMetaDataReader.GetDefaultValue(property);

				bool newValueIsDefault = newValue == null || Equals(newValue, defaultValueTranslated) || Equals(newValue, defaultValueUntranslated);
				bool currentValueIsDefault = currentValue == null || Equals(currentValue, defaultValueTranslated) || Equals(currentValue, defaultValueUntranslated);
				if (currentValueIsDefault && newValueIsDefault || Equals(currentValue, newValue))
					continue;

				element = GetSettingElement(clientSection, property, true);
				if (newValueIsDefault)
					newValue = defaultValueTranslated; //store defaults because it's convenient for editing.

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

		private static void GetNonDefaultSettingsValues
			(
				ConfigurationSectionGroup sectionGroup,
				string sectionName,
				IEnumerable<PropertyInfo> properties,
				IDictionary<string, string> values
			)	
		{
			if (sectionGroup == null)
				return; //the values are the same as the defaults.

			ConfigurationSection section = sectionGroup.Sections[sectionName];
			if (section == null)
				return; //the values are the same as the defaults.

			var clientSection = CastToClientSection(section);

			foreach (PropertyInfo property in properties)
			{
				SettingElement element = GetSettingElement(clientSection, property, false);
				if (element == null)
					continue;

				string currentValue = GetElementValue(element);
				if (currentValue == null) //not there means it's the default.
					continue;

				//translated or untranslated, it's still the default.
				string defaultValueUntranslated = SettingsClassMetaDataReader.GetDefaultValue(property, false);
				string defaultValueTranslated = SettingsClassMetaDataReader.GetDefaultValue(property);

				bool isDefaultValue = Equals(currentValue, defaultValueTranslated) || Equals(currentValue, defaultValueUntranslated);
				if (!isDefaultValue)
					values[property.Name] = currentValue;
			}
		}

		private static ConfigurationSectionGroup GetSectionGroup(SystemConfiguration configuration, ConfigurationSectionGroupPath sectionGroupPath, bool create)
		{
			if (configuration.GetSectionGroup(sectionGroupPath) == null && create)
				configuration.SectionGroups.Add(sectionGroupPath, sectionGroupPath.CreateSectionGroup());

			return configuration.GetSectionGroup(sectionGroupPath);
		}

        private static Dictionary<string, string> GetSettingsValues(SystemConfiguration configuration, ConfigurationSectionPath sectionPath, ICollection<PropertyInfo> properties)
        {
            var values = new Dictionary<string, string>();
			if (properties.Count > 0)
				GetNonDefaultSettingsValues(GetSectionGroup(configuration, sectionPath.GroupPath, false), sectionPath.SectionName, properties, values);

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
				var group = GetSectionGroup(configuration, sectionPath.GroupPath, true);
				modified = StoreSettings(group, sectionPath.SectionName, applicationScopedProperties, dirtyValues);
			}
			if (userScopedProperties.Count > 0)
			{
				var sectionPath = new ConfigurationSectionPath(settingsClass, SettingScope.User);
				var group = GetSectionGroup(configuration, sectionPath.GroupPath, true);
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
