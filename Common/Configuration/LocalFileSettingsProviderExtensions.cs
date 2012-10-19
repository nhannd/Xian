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
using System.Configuration;
using SystemConfiguration = System.Configuration.Configuration;

namespace ClearCanvas.Common.Configuration
{
    internal static class LocalFileSettingsProviderExtensions
    {
        public static void UpgradeSharedPropertyValues(LocalFileSettingsProvider provider, SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename, string currentExeConfigFilename = null)
        {
            SettingsPropertyValueCollection currentValues = GetSharedPropertyValues(provider, context, properties, currentExeConfigFilename);
            SettingsPropertyValueCollection previousValues = GetPreviousSharedPropertyValues(provider, context, properties, previousExeConfigFilename);

			foreach (SettingsProperty property in properties)
			{
				SettingsPropertyValue previousValue = previousValues[property.Name];
				if (previousValue != null)
				{
					SettingsPropertyValue currentValue = currentValues[property.Name];
					if (currentValue == null)
						currentValues.Add(previousValue);
					else
						currentValue.SerializedValue = previousValue.SerializedValue;
				}
			}

			SetSharedPropertyValues(provider, context, currentValues, currentExeConfigFilename);
        }

        public static SettingsPropertyValueCollection GetPreviousSharedPropertyValues(LocalFileSettingsProvider provider, SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
        {
        	var values = new SettingsPropertyValueCollection();
			if (String.IsNullOrEmpty(previousExeConfigFilename))
				return values;

        	var settingsClass = (Type)context["SettingsClassType"];
			var previousValues = new ConfigurationFileReader(previousExeConfigFilename).GetSettingsValues(settingsClass);
			if (previousValues == null)
				return values;

			foreach (var value in previousValues)
			{
				var property = properties[value.Key];
				if (property == null)
					continue;

				var settingsValue = new SettingsPropertyValue(property) { SerializedValue = value.Value, IsDirty = false };
				values.Add(settingsValue);
			}

			return values;
        }

        public static SettingsPropertyValueCollection GetSharedPropertyValues(LocalFileSettingsProvider provider, SettingsContext context, SettingsPropertyCollection properties, string currentExeConfigFilename = null)
        {
			var settingsClass = (Type)context["SettingsClassType"];

            var systemConfiguration = String.IsNullOrEmpty(currentExeConfigFilename)
                              ? SystemConfigurationHelper.GetExeConfiguration()
                              : SystemConfigurationHelper.GetExeConfiguration(currentExeConfigFilename);

        	var storedValues = systemConfiguration.GetSettingsValues(settingsClass);

			// Create new collection of values
			var values = new SettingsPropertyValueCollection();

			foreach (SettingsProperty setting in properties)
			{
				var value = new SettingsPropertyValue(setting)
				{
					SerializedValue = storedValues.ContainsKey(setting.Name) ? storedValues[setting.Name] : null,
					IsDirty = false
				};

				// use the stored value, or set the SerializedValue to null, which tells .NET to use the default value
				values.Add(value);
			}

			return values;
		}

        public static void SetSharedPropertyValues(LocalFileSettingsProvider provider, SettingsContext context, SettingsPropertyValueCollection values, string currentExeConfigFilename)
        {
            var valuesToStore = new Dictionary<string, string>();
			foreach (SettingsPropertyValue value in values)
			{
				if (value.SerializedValue != null)
					valuesToStore[value.Name] = (string)value.SerializedValue;
			}

            var systemConfiguration = String.IsNullOrEmpty(currentExeConfigFilename)
                                          ? SystemConfigurationHelper.GetExeConfiguration()
                                          : SystemConfigurationHelper.GetExeConfiguration(currentExeConfigFilename);

        	var settingsClass = (Type)context["SettingsClassType"];
            systemConfiguration.PutSettingsValues(settingsClass, valuesToStore);

            foreach (SettingsPropertyValue value in values)
                value.IsDirty = false;
        }
    }
}