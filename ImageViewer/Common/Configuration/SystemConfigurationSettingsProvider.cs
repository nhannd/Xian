using System;
using System.Collections.Generic;
using System.Configuration;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Common.Configuration
{
    public sealed class SystemConfigurationSettingsProvider : SettingsProvider, ISharedApplicationSettingsProvider
    {
        private readonly object _syncLock = new object();
        private ISystemConfigurationSettingsStore _store;

        public override string ApplicationName { get; set; }

        public SystemConfigurationSettingsProvider()
		{
			// according to MSDN recommendation, use the name of the executing assembly here
            ApplicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
		}

        ///<summary>
        ///Initializes the provider.
        ///</summary>
        ///
        ///<param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        ///<param name="name">The friendly name of the provider.</param>
        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            lock (_syncLock)
            {
                // obtain a source provider
                try
                {
                    _store = Platform.GetService<ISystemConfigurationSettingsStore>();
                }
                catch (NotSupportedException)
                {
                    // TODO (Marmot)
                    //Platform.Log(LogLevel.Warn, SR.LogConfigurationStoreNotFound);

                    // default to LocalFileSettingsProvider as a last resort
                }

                // init source provider
                // according to sample implementations, use the application name here
                base.Initialize(ApplicationName, config);
            }
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection)
        {
            var values = GetPropertyValues(context, collection, false);
            var settingsClass = (Type)context["SettingsClassType"];
        
            TranslateValues(settingsClass,values);

            return values;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection)
        {
            lock (_syncLock)
            {
                // locate dirty values that should be saved
                var valuesToStore = new Dictionary<string, string>();
                foreach (SettingsPropertyValue value in collection)
                {
                    //If storing the shared values, we store everything, otherwise only store user values.
                    if (value.IsDirty)
                        valuesToStore[value.Name] = (string)value.SerializedValue;
                }

                if (valuesToStore.Count > 0)
                {
                    var settingsClass = (Type)context["SettingsClassType"];
                    var settingsKey = (string)context["SettingsKey"];

                    _store.PutSettingsValues(new SettingsGroupDescriptor(settingsClass), null, settingsKey, valuesToStore);

                    // successfully saved user settings are no longer dirty
                    foreach (var storedValue in valuesToStore)
                        collection[storedValue.Key].IsDirty = false;
                }
            }
        }

        private static void TranslateValues(Type settingsClass, SettingsPropertyValueCollection values)
        {
            foreach (SettingsPropertyValue value in values)
            {
                if (value.SerializedValue == null || (value.SerializedValue is string) && ((string)value.SerializedValue) == ((string)value.Property.DefaultValue))
                    value.SerializedValue = SettingsClassMetaDataReader.TranslateDefaultValue(settingsClass, (string)value.Property.DefaultValue);
            }
        }

        public bool CanUpgradeSharedPropertyValues(SettingsContext context)
        {
            return false;            
        }

        public void UpgradeSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
        {
            // do nothing
        }

        public SettingsPropertyValueCollection GetPreviousSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties, string previousExeConfigFilename)
        {
            // return whats in the db as is
            return GetSharedPropertyValues(context, properties);
        }

        public SettingsPropertyValueCollection GetSharedPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
        {
            var settingsClass = (Type)context["SettingsClassType"];
            var settingsKey = (string)context["SettingsKey"];

            var group = new SettingsGroupDescriptor(settingsClass);
            var values = GetSharedPropertyValues(group, settingsKey, properties);
            TranslateValues(settingsClass, values);
            return values;
        }

        public void SetSharedPropertyValues(SettingsContext context, SettingsPropertyValueCollection values)
        {
            var valuesToStore = new Dictionary<string, string>();
            foreach (SettingsPropertyValue value in values)
            {
                if (value.IsDirty)
                    valuesToStore[value.Name] = (string)value.SerializedValue;
            }

            if (valuesToStore.Count > 0)
            {
                var settingsClass = (Type)context["SettingsClassType"];
                var settingsKey = (string)context["SettingsKey"];

                _store.PutSettingsValues(new SettingsGroupDescriptor(settingsClass), null, settingsKey, valuesToStore);
            }

            foreach (SettingsPropertyValue value in values)
                value.IsDirty = false;
        }


        private SettingsPropertyValueCollection GetSharedPropertyValues(SettingsGroupDescriptor group, string settingsKey, SettingsPropertyCollection properties)
        {
            var sharedValues = new Dictionary<string, string>();
            foreach (var sharedValue in _store.GetSettingsValues(group, null, settingsKey))
                sharedValues[sharedValue.Key] = sharedValue.Value;

            return GetSettingsValues(properties, sharedValues);
        }

        private static SettingsPropertyValueCollection GetSettingsValues(SettingsPropertyCollection properties, IDictionary<string, string> storedValues)
        {
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

        private SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties, bool returnPrevious)
        {
            var settingsClass = (Type)context["SettingsClassType"];
            var settingsKey = (string)context["SettingsKey"];

            var group = new SettingsGroupDescriptor(settingsClass);

            var storedValues = new Dictionary<string, string>();

            foreach (var userDefault in _store.GetSettingsValues(@group, null, settingsKey))
                storedValues[userDefault.Key] = userDefault.Value;

            return GetSettingsValues(properties, storedValues);
        }
    }
}
