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