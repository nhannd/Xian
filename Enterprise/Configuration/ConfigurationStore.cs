using System;
using System.Collections.Generic;
using System.Text;

using System.Configuration;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Configuration
{
    /// <summary>
    /// Implementation of <see cref="IConfigurationStore"/>, extends <see cref="ConfigurationStoreExtensionPoint"/>.
    /// Acts as a client-side proxy to <see cref="IConfigurationService"/>
    /// </summary>
    [ExtensionOf(typeof(ConfigurationStoreExtensionPoint))]
    public class ConfigurationStore : IConfigurationStore
    {
        private IConfigurationService _service;

        public ConfigurationStore()
        {
            _service = ApplicationContext.GetService<IConfigurationService>();
        }

        #region IEnterpriseConfigurationStore Members

        public void LoadSettingsValues(Type settingsClass, string user, string instanceKey, IDictionary<string, string> values)
        {
            _service.LoadSettingsValues(
                SettingsClassMetaDataReader.GetGroupName(settingsClass),
                SettingsClassMetaDataReader.GetVersion(settingsClass),
                user,
                instanceKey,
                values);
        }

        public void SaveSettingsValues(Type settingsClass, string user, string instanceKey, IDictionary<string, string> values)
        {
            _service.SaveSettingsValues(
                SettingsClassMetaDataReader.GetGroupName(settingsClass),
                SettingsClassMetaDataReader.GetVersion(settingsClass),
                user,
                instanceKey,
                values);
        }

        public void RemoveUserSettings(Type settingsClass, string user, string instanceKey)
        {
            _service.Clear(
                SettingsClassMetaDataReader.GetGroupName(settingsClass),
                SettingsClassMetaDataReader.GetVersion(settingsClass),
                user,
                instanceKey);
        }

        public void UpgradeUserSettings(Type settingsClass, string user, string instanceKey)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            _service.UpgradeFromPreviousVersion(
                SettingsClassMetaDataReader.GetGroupName(settingsClass),
                SettingsClassMetaDataReader.GetVersion(settingsClass),
                user,
                instanceKey,
                values);
        }

        #endregion
    }
}
