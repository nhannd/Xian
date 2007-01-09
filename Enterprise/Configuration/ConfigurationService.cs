using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Configuration.Brokers;

namespace ClearCanvas.Enterprise.Configuration
{
    [ExtensionOf(typeof(ServiceLayerExtensionPoint))]
    public class ConfigurationService : ConfigurationServiceLayer, IConfigurationService
    {
        #region IConfigurationService Members

        [ReadOperation]
        public IList<ConfigSetting> LoadConfigSettings(string appName, string appVersion, string settingsGroup, string settingsKey)
        {
            ConfigSettingSearchCriteria criteria = new ConfigSettingSearchCriteria();
            criteria.ApplicationName.EqualTo(appName);
            criteria.ApplicationVersion.EqualTo(appVersion);
            criteria.SettingsGroup.EqualTo(settingsGroup);
            criteria.SettingsKey.EqualTo(settingsKey);

            // TODO add user vs app settings scope criteria

            IConfigSettingBroker broker = this.CurrentContext.GetBroker<IConfigSettingBroker>();
            return broker.Find(criteria);
        }

        #endregion
    }
}
