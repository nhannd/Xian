using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Configuration
{
    public interface IConfigurationService
    {
        ConfigSettingsInstance LoadConfigSettings(string group, string version, string user, string instanceKey);
        void SaveConfigSettings(ConfigSettingsInstance settings);

        ConfigSettingsGroup ImportConfigSettingsClass(Type settingsClass);
    }
}
