using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;

using ClearCanvas.Enterprise;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Configuration
{
    public class EnterpriseSettingsProvider : SettingsProvider
    {
        private string _appName;
        private IConfigurationService _service;

        public EnterpriseSettingsProvider(IConfigurationService service)
        {
            _service = service;
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
            ConfigSettingsInstance storedSettings = GetConfigSettingsInstance(context);

            // Create new collection of values
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

            // Iterate through the settings to be retrieved
            foreach (SettingsProperty setting in props)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(setting);
                value.IsDirty = false;
                value.SerializedValue = storedSettings[setting.Name];
                values.Add(value);
            }
            return values;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection settings)
        {
            ConfigSettingsInstance storedSettings = GetConfigSettingsInstance(context);

            foreach (SettingsPropertyValue value in settings)
            {
                if(IsUserScoped(value.Property) && value.IsDirty)
                {
                    storedSettings[value.Name] = (string)value.SerializedValue;
                }
            }

            // save settings
            _service.SaveConfigSettings(storedSettings);
        }

        private bool IsUserScoped(SettingsProperty settingsProperty)
        {
            return CollectionUtils.Contains(settingsProperty.Attributes.Values,
                delegate(object obj) { return obj is UserScopedSettingAttribute; });
        }

        private ConfigSettingsInstance GetConfigSettingsInstance(SettingsContext context)
        {
            Type settingsClass = (Type)context["SettingsClassType"];

            // TODO: determine current user
            string user = "me";
            string version = SettingsClassMetaDataReader.GetVersion(settingsClass);
            string groupName = SettingsClassMetaDataReader.GetGroupName(settingsClass);
            string settingsKey = (string)context["SettingsKey"];

            ConfigSettingsInstance settings = null;
            try
            {
                // try to load the settings
                return _service.LoadConfigSettings(groupName, version, user, settingsKey);
            }
            catch (EntityNotFoundException)
            {
                // the settings group has not been imported yet, so import it now
                ConfigSettingsGroup group = _service.ImportConfigSettingsClass(settingsClass);

                // return a new settings instance object
                return new ConfigSettingsInstance(group, user, settingsKey);
            }
        }

        #region IApplicationSettingsProvider Members

        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Reset(SettingsContext context)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
