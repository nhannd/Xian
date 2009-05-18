using System;
using System.Configuration;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Configuration
{

    // TODO add a description of the purpose of the settings group here
    [SettingsGroupDescription("")]
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
    internal sealed partial class ConfigurationStoreSettings
    {
        ///<summary>
        /// Public constructor.  Server-side settings classes should be instantiated via constructor rather
        /// than using the <see cref="ConfigurationStoreSettings.Default"/> property to avoid creating a static instance.
        ///</summary>
        public ConfigurationStoreSettings()
        {
            // Note: server-side settings classes do not register in the <see cref="ApplicationSettingsRegistry"/>
        }
    }
}
