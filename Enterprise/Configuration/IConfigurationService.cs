using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Configuration
{
    public interface IConfigurationService
    {
        IList<ConfigSetting> LoadConfigSettings(string appName, string appVersion, string settingsGroup, string settingsKey);
    }
}
