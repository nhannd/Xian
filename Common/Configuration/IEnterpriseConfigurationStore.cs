using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ClearCanvas.Common.Configuration
{
    public interface IEnterpriseConfigurationStore
    {
        SettingsProvider GetSettingsProvider();
    }
}
