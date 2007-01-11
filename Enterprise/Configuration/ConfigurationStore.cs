using System;
using System.Collections.Generic;
using System.Text;

using System.Configuration;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Configuration
{
    [ExtensionOf(typeof(EnterpriseConfigurationStoreExtensionPoint))]
    public class ConfigurationStore : IEnterpriseConfigurationStore
    {
        #region IEnterpriseConfigurationStore Members

        public SettingsProvider GetSettingsProvider()
        {
            return new EnterpriseSettingsProvider(ApplicationContext.GetService<IConfigurationService>());
        }

        #endregion
    }
}
