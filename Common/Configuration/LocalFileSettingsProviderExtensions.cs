using System;
using System.Collections.Generic;
using System.Configuration;
using SystemConfiguration = System.Configuration.Configuration;

namespace ClearCanvas.Common.Configuration
{
    internal static class LocalFileSettingsProviderExtensions
    {
        public static void UpgradeSharedPropertyValues(LocalFileSettingsProvider provider, SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
        {
            SettingsPropertyValueCollection currentValues = GetSharedPropertyValues(provider, context, properties);
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

			SetSharedPropertyValues(provider, context, currentValues);
        }

        public static SettingsPropertyValueCollection GetPreviousSharedPropertyValues(LocalFileSettingsProvider provider, SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
        {
        	var values = new SettingsPropertyValueCollection();
			if (String.IsNullOrEmpty(previousExeConfigFilename))
				return values;

        	var settingsClass = (Type)context["SettingsClassType"];
			var previousValues = SystemConfigurationHelper.GetSettingsValues(
				SystemConfigurationHelper.GetExeConfiguration(previousExeConfigFilename), settingsClass);
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

        public static SettingsPropertyValueCollection GetSharedPropertyValues(LocalFileSettingsProvider provider, SettingsContext context, SettingsPropertyCollection properties)
        {
			var settingsClass = (Type)context["SettingsClassType"];
        	var storedValues = SystemConfigurationHelper.GetSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass);

			// Create new collection of values
			SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

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

        public static void SetSharedPropertyValues(LocalFileSettingsProvider provider, SettingsContext context, SettingsPropertyValueCollection values)
        {
            var valuesToStore = new Dictionary<string, string>();
			foreach (SettingsPropertyValue value in values)
			{
				if (value.SerializedValue != null)
					valuesToStore[value.Name] = (string)value.SerializedValue;
			}

        	var settingsClass = (Type)context["SettingsClassType"];
			SystemConfigurationHelper.PutSettingsValues(SystemConfigurationHelper.GetExeConfiguration(), settingsClass, valuesToStore);

            foreach (SettingsPropertyValue value in values)
                value.IsDirty = false;
        }
    }
}