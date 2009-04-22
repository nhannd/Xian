#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Text;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common;
using System.Configuration;
using System.Reflection;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using System.ComponentModel;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// The LocalSettingsStore, although it implements <see cref="ISettingsStore "/> does not serve 
	/// as a proper settings store for the <see cref="StandardSettingsProvider"/> (notice that it is not
	/// an extension of <see cref="SettingsStoreExtensionPoint"/>.  Instead, this class is instantiated
	/// directly by the <see cref="SettingsManagementComponent"/> when there are no such extensions available,
	/// and the application is using the <see cref="LocalFileSettingsProvider"/> (or app/user .config) to 
	/// store settings locally.  This 'settings store' is used solely to edit the default profile
	/// throught the settings management UI.
	/// </summary>
	internal class LocalSettingsStore : ISettingsStore
	{
		private static readonly string _applicationSettingsGroup = "applicationSettings";
		private static readonly string _userSettingsGroup = "userSettings";

		/// <summary>
		/// Constructor.
		/// </summary>
		public LocalSettingsStore()
		{
        }

		/// <summary>
		/// Determines how a particular property should be serialized based on its type.
		/// </summary>
		/// <param name="property">the property whose SerializeAs method is to be determined</param>
		/// <returns>a <see cref="SettingsSerializeAs"/> value</returns>
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

		/// <summary>
		/// Constructs a new <see cref="ClientSettingsSection"/> containing all of the default values
		/// for the particular settings class.
		/// </summary>
		/// <param name="properties">the properties to be added to the new section</param>
		/// <returns>a <see cref="ClientSettingsSection"/> object</returns>
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

		/// <summary>
		/// Stores/adds all setting values to a particular <see cref="ConfigurationSection"/>, not just the ones 
		/// that are different (default values are also stored).
		/// </summary>
		/// <param name="sectionGroup">the parent section group (applicationSettings or userSettings)</param>
		/// <param name="sectionName">the name of the <see cref="ClientSettingsSection"/> to change/store</param>
		/// <param name="scopeProperties">the settings properties that correspond to the same scope as the 
		/// sectionGroup (e.g. applicationSettings or userSettings)</param>
		/// <param name="newValues">the 'new' values to store.  Only dirty values are contained in this dictionary.</param>
		/// <param name="oldValues">for any values that get changed by this method, the previous values will be returned</param>
		/// <returns>whether or not any modifications were made, which will normally be true unless the values in the store
		/// are already the same.</returns>
		/// <exception cref="NotSupportedException">when the section corresponding to sectionName is not a <see cref="ClientSettingsSection"/></exception>
		private bool StoreSettings
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
				string requiredValue = element.Value.ValueXml.InnerText;
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

		/// <summary>
		/// Gets all the settings for a particular <see cref="ClientSettingsSection"/> that are different from the property defaults.
		/// </summary>
		/// <param name="sectionGroup">the parent section group (applicationSettings or userSettings)</param>
		/// <param name="sectionName">the name of the <see cref="ClientSettingsSection"/> whose values are to be retrieved</param>
		/// <param name="scopeProperties">the settings properties that correspond to the same scope as the 
		/// sectionGroup (e.g. applicationSettings or userSettings)</param>
		/// <param name="values">returns the property values that are different from the defaults</param>
		/// <exception cref="NotSupportedException"> when the section corresponding to sectionName is not a <see cref="ClientSettingsSection"/></exception>
		/// <exception cref="ArgumentException">if a property in scopeProperties is not found</exception>
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
					throw new ArgumentException(String.Format(SR.ExceptionSettingsPropertyDoesNotExist, String.Format("{0}/{1}/{2}", sectionGroup, sectionName, property.Name)));

				string currentValue = element.Value.ValueXml.InnerText;
				//translated or untranslated, it's still the default.
				bool isDefaultValue = (currentValue == defaultValueTranslated || currentValue == defaultValueUntranslated);

				if (!isDefaultValue)
					values[property.Name] = currentValue;
			}
		}

		/// <summary>
		/// Gets the applicationSettings group <see cref="ConfigurationSectionGroup"/>.
		/// </summary>
		/// <param name="configuration">the local configuration object</param>
		/// <param name="create">a boolean indicating whether or not to create the applicationSettings group</param>
		/// <returns>the applicationSettings group</returns>
		private ConfigurationSectionGroup GetApplicationSettingsGroup(System.Configuration.Configuration configuration, bool create)
		{
			if (configuration.GetSectionGroup(_applicationSettingsGroup) == null && create)
				configuration.SectionGroups.Add(_applicationSettingsGroup, new ApplicationSettingsGroup());

			return configuration.GetSectionGroup(_applicationSettingsGroup);
		}

		/// <summary>
		/// Gets the userSettings group <see cref="ConfigurationSectionGroup"/>.
		/// </summary>
		/// <param name="configuration">the local configuration object</param>
		/// <param name="create">a boolean indicating whether or not to create the userSettings group</param>
		/// <returns>the userSettings group</returns>
		private ConfigurationSectionGroup GetUserSettingsGroup(System.Configuration.Configuration configuration, bool create)
		{
			if (configuration.GetSectionGroup(_userSettingsGroup) == null && create)
				configuration.SectionGroups.Add(_userSettingsGroup, new UserSettingsGroup());

			return configuration.GetSectionGroup(_userSettingsGroup);
		}

		/// <summary>
		/// Splits up the application/user scoped settings properties
		/// </summary>
		/// <param name="properties">the entire set of settings properties</param>
		/// <param name="applicationScopedProperties">upon returning, contains the application settings scoped properties</param>
		/// <param name="userScopedProperties">upon returning, contains the user settings scoped properties</param>
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

		#region ISettingsStore Members
		
		/// <summary>
		/// Loads the settings values (both application and user scoped) for a given settings class.  Only the shared profile
		/// is supported (application settings + default user settings).
		/// </summary>
        /// <param name="group">the settings class for which to retrieve the defaults</param>
		/// <param name="user">must be null or ""</param>
		/// <param name="instanceKey">ignored</param>
        /// <returns>returns only those values that are different from the property defaults</returns>
		/// <exception cref="NotSupportedException">will be thrown if the user is specified</exception>
        public Dictionary<string, string> GetSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey)
		{
			if (!String.IsNullOrEmpty(user))
				throw new NotSupportedException(SR.ExceptionOnlyDefaultProfileSupported);

            Type settingsClass = Type.GetType(group.AssemblyQualifiedTypeName);
			ICollection<PropertyInfo> properties = SettingsClassMetaDataReader.GetSettingsProperties(settingsClass);

			System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

			List<PropertyInfo> applicationScopedProperties;
			List<PropertyInfo> userScopedProperties;
			SplitPropertiesByScope(properties, out applicationScopedProperties, out userScopedProperties);

            Dictionary<string, string> values = new Dictionary<string, string>();
			if (applicationScopedProperties.Count > 0)
				GetNonDefaultSettings(this.GetApplicationSettingsGroup(configuration, false), group.Name, applicationScopedProperties, values);
			if (userScopedProperties.Count > 0)
                GetNonDefaultSettings(this.GetUserSettingsGroup(configuration, false), group.Name, userScopedProperties, values);

            return values;
		}

		/// <summary>
		/// Stores the settings values (both application and user scoped) for a given settings class.  Only the shared profile
		/// is supported (application settings + default user settings).
		/// </summary>
        /// <param name="group">the settings class for which to store the values</param>
		/// <param name="user">must be null or ""</param>
		/// <param name="instanceKey">ignored</param>
		/// <param name="dirtyValues">contains the values to be stored</param>
		/// <exception cref="NotSupportedException">will be thrown if the user is specified</exception>
        public void PutSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey, Dictionary<string, string> dirtyValues)
		{
			if (!String.IsNullOrEmpty(user))
				throw new NotSupportedException(SR.ExceptionOnlyDefaultProfileSupported);

            Type settingsClass = Type.GetType(group.AssemblyQualifiedTypeName);
            ICollection<PropertyInfo> properties = SettingsClassMetaDataReader.GetSettingsProperties(settingsClass);

			System.Configuration.Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

			List<PropertyInfo> applicationScopedProperties;
			List<PropertyInfo> userScopedProperties;
			SplitPropertiesByScope(properties, out applicationScopedProperties, out userScopedProperties);

			Dictionary<string, string> changedValues = new Dictionary<string, string>();

			bool modified = false;
			if (applicationScopedProperties.Count > 0)
                modified = StoreSettings(this.GetApplicationSettingsGroup(configuration, true), group.Name, applicationScopedProperties, dirtyValues, changedValues);
			if (userScopedProperties.Count > 0)
                modified |= StoreSettings(this.GetUserSettingsGroup(configuration, true), group.Name, userScopedProperties, dirtyValues, changedValues);

			if (modified)
			{
				configuration.Save(ConfigurationSaveMode.Minimal, true);
			}
		}

		/// <summary>
		/// Unsupported.  An exception will always be thrown.
		/// </summary>
        /// <param name="group"></param>
		/// <param name="user"></param>
		/// <param name="instanceKey"></param>
		/// <exception cref="NotSupportedException">always thrown</exception>
        public void RemoveUserSettings(SettingsGroupDescriptor group, string user, string instanceKey)
		{
			throw new NotSupportedException(SR.ExceptionRemoveUserSettingNotSupported);
		}

		/// <summary>
		/// Unsupported.  An exception will always be thrown.
		/// </summary>
        /// <param name="group"></param>
		/// <param name="user"></param>
		/// <param name="instanceKey"></param>
		/// <exception cref="NotSupportedException">always thrown</exception>
        public void UpgradeUserSettings(SettingsGroupDescriptor group, string user, string instanceKey)
		{
			throw new NotSupportedException(SR.ExceptionUpgradeNotSupported);
		}

        /// <summary>
        /// Returns settings groups installed on local machine
        /// </summary>
        /// <returns></returns>
        public IList<SettingsGroupDescriptor> ListSettingsGroups()
        {
            return SettingsGroupDescriptor.ListInstalledSettingsGroups(false);
        }

        /// <summary>
        /// Returns settings properties for specified group, assuming plugin containing group resides on local machine
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group)
        {
            return SettingsPropertyDescriptor.ListSettingsProperties(group);
        }

        #endregion
    }
}
