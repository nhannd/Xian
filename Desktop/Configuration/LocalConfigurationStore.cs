using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common;
using System.Configuration;
using System.Reflection;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.ComponentModel;

namespace ClearCanvas.Desktop.Configuration
{
	internal class LocalConfigurationStore : IConfigurationStore
	{
		private static string _applicationSettingsGroup = "applicationSettings";
		private static string _userSettingsGroup = "userSettings";

		public LocalConfigurationStore()
		{
		}

		private SettingsSerializeAs DetermineSerializeAs(PropertyInfo property)
		{
			object[] serializeAsAttributes = property.GetCustomAttributes(typeof(SettingsSerializeAsAttribute), false);
			if (serializeAsAttributes.Length > 0)
				return ((SettingsSerializeAsAttribute)serializeAsAttributes[0]).SerializeAs;

			TypeConverter converter = TypeDescriptor.GetConverter(property.PropertyType);
			Type stringType = typeof(string);
			if (converter.CanConvertTo(stringType) && converter.CanConvertFrom(stringType))
				return SettingsSerializeAs.String;

			return SettingsSerializeAs.Xml;
		}

		private ClientSettingsSection ConstructDefaultSection(IEnumerable<PropertyInfo> properties)
		{
			ClientSettingsSection section = new ClientSettingsSection();
			section.SectionInformation.RequirePermission = false;

			XmlDocument document = new XmlDocument();

			foreach (PropertyInfo property in properties)
			{
				SettingElement element = new SettingElement();
				element.Name = property.Name;
				element.SerializeAs = DetermineSerializeAs(property);

				SettingValueElement newElement = new SettingValueElement();
				XmlNode valueXml = document.CreateElement("value");
				valueXml.InnerText = SettingsClassMetaDataReader.GetDefaultValue(property);

				newElement.ValueXml = valueXml;

				element.Value = newElement;
				section.Settings.Add(element);
			}

			return section;
		}

		private bool StoreNonDefaultSettings
			(
				ConfigurationSectionGroup sectionGroup,
				string sectionName,
				List<PropertyInfo> scopeProperties,
				IDictionary<string, string> newValues,
				IDictionary<string, string> oldValues
			)
		{
			bool newSection = false;
			ConfigurationSection section = sectionGroup.Sections[sectionName];
			if (section == null)
			{
				newSection = true;
				ClientSettingsSection newClientSection = ConstructDefaultSection(scopeProperties);
				sectionGroup.Sections.Add(sectionName, newClientSection);
				if (sectionGroup.Name == _userSettingsGroup)
					newClientSection.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;

				section = sectionGroup.Sections[sectionName];
			}

			ClientSettingsSection clientSection = section as ClientSettingsSection;
			if (clientSection == null)
				throw new NotSupportedException(String.Format(SR.ExceptionSectionIsNotAClientSection, section.GetType().FullName));

			bool modified = false;

			foreach (PropertyInfo property in scopeProperties)
			{
				SettingElement element = clientSection.Settings.Get(property.Name);
				string requiredValue = SettingsClassMetaDataReader.GetDefaultValue(property);
				if (newValues.ContainsKey(property.Name))
					requiredValue = newValues[property.Name];

				if (element.Value.ValueXml.InnerText != requiredValue)
				{
					oldValues[property.Name] = element.Value.ValueXml.InnerText;

					element.Value.ValueXml.InnerText = requiredValue;
					modified = true;
				}
			}

			if (newSection && !modified)
				sectionGroup.Sections.Remove(sectionName);

			return modified;
		}

		private void GetNonDefaultSettings
			(
				ConfigurationSectionGroup sectionGroup,
				string sectionName,
				List<PropertyInfo> scopeProperties,
				IDictionary<string, string> values
			)	
		{
			if (sectionGroup == null)
				return; //the values are the same as the defaults.

			ConfigurationSection section = sectionGroup.Sections[sectionName];
			if (section == null)
				return; //the values are the same as the defaults.

			ClientSettingsSection clientSection = section as ClientSettingsSection;
			if (clientSection == null)
				throw new NotSupportedException(String.Format(SR.ExceptionSectionIsNotAClientSection, section.GetType().FullName));

			foreach (PropertyInfo property in scopeProperties)
			{
				string defaultValueUntranslated = SettingsClassMetaDataReader.GetDefaultValue(property, false);
				string defaultValueTranslated = SettingsClassMetaDataReader.GetDefaultValue(property);

				SettingElement element = clientSection.Settings.Get(property.Name);
				if (element == null)
					throw new Exception(String.Format(SR.ExceptionSettingsPropertyDoesNotExist, String.Format("{0}/{1}/{2}", sectionGroup, sectionName, property.Name)));

				string currentValue = element.Value.ValueXml.InnerText;
				bool isDefaultValue = (currentValue == defaultValueTranslated || currentValue == defaultValueUntranslated);

				if (!isDefaultValue)
					values[property.Name] = currentValue;
			}
		}

		private ConfigurationSectionGroup GetApplicationSettingsGroup(System.Configuration.Configuration configuration, bool create)
		{
			if (configuration.GetSectionGroup(_applicationSettingsGroup) == null && create)
				configuration.SectionGroups.Add(_applicationSettingsGroup, new ApplicationSettingsGroup());

			return configuration.GetSectionGroup(_applicationSettingsGroup);
		}

		private ConfigurationSectionGroup GetUserSettingsGroup(System.Configuration.Configuration configuration, bool create)
		{
			if (configuration.GetSectionGroup(_userSettingsGroup) == null && create)
				configuration.SectionGroups.Add(_userSettingsGroup, new UserSettingsGroup());

			return configuration.GetSectionGroup(_userSettingsGroup);
		}

		private void SplitPropertiesByScope
			(
				IEnumerable<PropertyInfo> properties,
				out List<PropertyInfo> applicationScopedProperties,
				out List<PropertyInfo> userScopedProperties
			)
		{
			applicationScopedProperties = new List<PropertyInfo>();
			userScopedProperties = new List<PropertyInfo>();

			foreach (PropertyInfo property in properties)
			{
				if (SettingsClassMetaDataReader.IsUserScoped(property))
					userScopedProperties.Add(property);
				else if (SettingsClassMetaDataReader.IsAppScoped(property))
					applicationScopedProperties.Add(property);
			}
		}

		#region IConfigurationStore Members
		
		public void LoadSettingsValues(Type settingsClass, string user, string instanceKey, IDictionary<string, string> values)
		{
			if (!String.IsNullOrEmpty(user))
				throw new NotSupportedException(SR.ExceptionOnlyDefaultProfileSupported);

			string groupName = SettingsClassMetaDataReader.GetGroupName(settingsClass);
			ICollection<PropertyInfo> properties = SettingsClassMetaDataReader.GetSettingsProperties(settingsClass);

			System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

			List<PropertyInfo> applicationScopedProperties;
			List<PropertyInfo> userScopedProperties;
			SplitPropertiesByScope(properties, out applicationScopedProperties, out userScopedProperties);

			if (applicationScopedProperties.Count > 0)
				GetNonDefaultSettings(this.GetApplicationSettingsGroup(configuration, false), groupName, applicationScopedProperties, values);
			if (userScopedProperties.Count > 0)
				GetNonDefaultSettings(this.GetUserSettingsGroup(configuration, false), groupName, userScopedProperties, values);
		}

		public void SaveSettingsValues(Type settingsClass, string user, string instanceKey, IDictionary<string, string> values)
		{
			if (!String.IsNullOrEmpty(user))
				throw new NotSupportedException(SR.ExceptionOnlyDefaultProfileSupported);

			string groupName = SettingsClassMetaDataReader.GetGroupName(settingsClass);
			ICollection<PropertyInfo> properties = SettingsClassMetaDataReader.GetSettingsProperties(settingsClass);

			System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

			List<PropertyInfo> applicationScopedProperties;
			List<PropertyInfo> userScopedProperties;
			SplitPropertiesByScope(properties, out applicationScopedProperties, out userScopedProperties);

			Dictionary<string, string> changedValues = new Dictionary<string, string>();

			bool modified = false;
			if (applicationScopedProperties.Count > 0)
				modified = StoreNonDefaultSettings(this.GetApplicationSettingsGroup(configuration, true), groupName, applicationScopedProperties, values, changedValues);
			if (userScopedProperties.Count > 0)
				modified |= StoreNonDefaultSettings(this.GetUserSettingsGroup(configuration, true), groupName, userScopedProperties, values, changedValues);

			if (modified)
			{
				configuration.Save(ConfigurationSaveMode.Minimal, true);
				ApplicationSettingsRegister.Instance.SynchronizeExistingSettings(settingsClass, changedValues, values);
			}
		}

		public void RemoveUserSettings(Type settingsClass, string user, string instanceKey)
		{
			throw new NotSupportedException(SR.ExceptionRemoveUserSettingNotSupported);
		}

		public void UpgradeUserSettings(Type settingsClass, string user, string instanceKey)
		{
			throw new NotSupportedException(SR.ExceptionUpgradeNotSupported);
		}

		#endregion
	}
}
