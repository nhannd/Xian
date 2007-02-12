using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using System.Threading;

namespace ClearCanvas.Common.Configuration
{
    /// <summary>
    /// Communicates with an <see cref="IConfigurationStore"/> to manage loading and saving of
    /// settings from the store.  Supports the framework and is not intended for use by application code.
    /// Use <see cref="StandardSettingsProvider"/> instead.
    /// </summary>
    internal class ConfigurationStoreSettingsProvider : SettingsProvider, IApplicationSettingsProvider
    {
        private string _appName;
        private IConfigurationStore _store;

        internal ConfigurationStoreSettingsProvider(IConfigurationStore store)
        {
            _store = store;
        }

        public override string ApplicationName
        {
            get
            {
                return _appName;
            }
            set
            {
                _appName = value;
            }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
        }

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
        {
            Type settingsClass = (Type)context["SettingsClassType"];
            string settingsKey = (string)context["SettingsKey"];
            string user = Thread.CurrentPrincipal.Identity.Name;

            Dictionary<string, string> storedValues = new Dictionary<string, string>();

            // load shared values
            _store.LoadSettingsValues(settingsClass, null, settingsKey, storedValues);

            // overwrite shared values with user values, if they exist
            _store.LoadSettingsValues(settingsClass, user, settingsKey, storedValues);

            // Create new collection of values
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

            // Iterate through the settings to be retrieved
            foreach (SettingsProperty setting in props)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(setting);
                value.IsDirty = false;

                // use the stored value, or set the SerializedValue to null, which tells .NET to use the default value
                value.SerializedValue = storedValues.ContainsKey(setting.Name) ? storedValues[setting.Name] : null;
                values.Add(value);
            }
 
            return values;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection settings)
        {
            Dictionary<string, string> valuesToStore = new Dictionary<string, string>();

            foreach (SettingsPropertyValue value in settings)
            {
                // only user scoped settings should be saved (changes to app settings are made directly
                // via the IEnterpriseConfigurationStore and not through this class)

                // only values that are different than the default should be persisted to the store
                // the reason is that, when a new version of the settings class is deployed,
                // the defaults of the new version should take precedence over the defaults of the old version
                // but should not take precedence over the stored values
                // the only way to make this distinction is to not store any values that are same as default
                if(IsUserScoped(value.Property) && !value.UsingDefaultValue)
                {
                    valuesToStore[value.Name] = (string)value.SerializedValue;
                }
            }

            Type settingsClass = (Type)context["SettingsClassType"];
            string settingsKey = (string)context["SettingsKey"];
            string user = Thread.CurrentPrincipal.Identity.Name;

            // must call this method even if valuesToStore is empty (in which case the stored values are cleared)
            _store.SaveSettingsValues(settingsClass, user, settingsKey, valuesToStore);
        }

        private bool IsUserScoped(SettingsProperty settingsProperty)
        {
            return CollectionUtils.Contains(settingsProperty.Attributes.Values,
                delegate(object obj) { return obj is UserScopedSettingAttribute; });
        }

        #region IApplicationSettingsProvider Members

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="context"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
        {
            // seems like implementing this method would be quite inefficient, unless we could be sure that
            // the IConfigurationStore implementation had sufficient optimizations in place
            // let's leave this to be implemented "as needed"
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Note that this implementation resets the user-scoped settings only.  It does not touch application-scoped settings.
        /// </summary>
        /// <param name="context"></param>
        public void Reset(SettingsContext context)
        {
            Type settingsClass = (Type)context["SettingsClassType"];
            string settingsKey = (string)context["SettingsKey"];
            string user = Thread.CurrentPrincipal.Identity.Name;

            _store.RemoveUserSettings(settingsClass, user, settingsKey);
        }

        /// <summary>
        /// Note that this implementation upgrades user-scoped settings only, and it upgrades all settings in the group,
        /// regardless of the contents of the specified properties collection.  It does not touch application-scoped settings.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="properties"></param>
        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
        {
            Type settingsClass = (Type)context["SettingsClassType"];
            string settingsKey = (string)context["SettingsKey"];
            string user = Thread.CurrentPrincipal.Identity.Name;

            // here we just upgrade the settings in the store... the .net framework will call GetPropertyValues again
            // to obtain the new values
            _store.UpgradeUserSettings(settingsClass, user, settingsKey);
        }

        #endregion
    }
}
