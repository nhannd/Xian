using System;
using System.Collections.Generic;
using System.Text;

using System.Configuration;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Configuration
{
    /// <summary>
    /// Implementation of <see cref="IConfigurationStore"/>, extends <see cref="EnterpriseConfigurationStoreExtensionPoint"/>.
    /// Acts as a client-side proxy to <see cref="IConfigurationService"/>
    /// </summary>
    [ExtensionOf(typeof(EnterpriseConfigurationStoreExtensionPoint))]
    public class ConfigurationStore : IConfigurationStore
    {
        private IConfigurationService _service;

        public ConfigurationStore()
        {
            _service = ApplicationContext.GetService<IConfigurationService>();
        }

        #region IEnterpriseConfigurationStore Members

        public IDictionary<string, string> LoadSettingsValues(Type settingsClass, string instanceKey)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            // get the shared values
            _service.LoadSettingsValues(
                SettingsClassMetaDataReader.GetGroupName(settingsClass),
                SettingsClassMetaDataReader.GetVersion(settingsClass),
                null,
                null,
                values);

            // overwrite the shared values with the user values
            //TODO: determine current user
            string user = "me";
            _service.LoadSettingsValues(
                SettingsClassMetaDataReader.GetGroupName(settingsClass),
                SettingsClassMetaDataReader.GetVersion(settingsClass),
                user,
                instanceKey,
                values);

            return values;
        }

        public void SaveSettingsValues(Type settingsClass, string instanceKey, IDictionary<string, string> values)
        {
            //TODO: determine current user
            string user = "me";
            _service.SaveSettingsValues(
                SettingsClassMetaDataReader.GetGroupName(settingsClass),
                SettingsClassMetaDataReader.GetVersion(settingsClass),
                user,
                instanceKey,
                values);
        }

        public void RemoveUserSettings(Type settingsClass, string instanceKey)
        {
            //TODO: determine current user
            string user = "me";
            _service.Clear(
                SettingsClassMetaDataReader.GetGroupName(settingsClass),
                SettingsClassMetaDataReader.GetVersion(settingsClass),
                user,
                instanceKey);
        }

        public void UpgradeUserSettings(Type settingsClass, string instanceKey)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            //TODO: determine current user
            string user = "me";
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
