using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace ClearCanvas.Common.Configuration
{
    /// <summary>
    /// Defines the interface to a mechanism for the storage of configuration data
    /// </summary>
    public interface IConfigurationStore
    {
        /// <summary>
        /// Obtains the settings values for the specified settings class, for the current user and
        /// specified instance key. The method returns a dictionary containing only values for settings
        /// that differ from the default value as specified by the settings class.
        /// </summary>
        /// <param name="settingsClass"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        IDictionary<string, string> LoadSettingsValues(Type settingsClass, string instanceKey);

        /// <summary>
        /// Store the settings values for the specified settings class, for the current user and
        /// specified instance key.  The dictionary contains only values that differ from the default
        /// values as specified by the settings class.  Only these values should be stored.  Any previously
        /// stored settings values that are not contained in the dictionary should be removed from the store.
        /// </summary>
        /// <param name="settingsClass"></param>
        /// <param name="instanceKey"></param>
        /// <param name="values"></param>
        void SaveSettingsValues(Type settingsClass, string instanceKey, IDictionary<string, string> values);

        /// <summary>
        /// Removes user settings from this group, effectively causing them to be reset to their shared default
        /// values.  Application-scoped settings are unaffected.
        /// </summary>
        /// <param name="settingsClass"></param>
        /// <param name="instanceKey"></param>
        void RemoveUserSettings(Type settingsClass, string instanceKey);

        /// <summary>
        /// Upgrades user settings in the group, effectively importing any settings saved in a previous version
        /// of the application into the current version.  Application-scoped settings are unaffected.
        /// </summary>
        /// <param name="settingsClass"></param>
        /// <param name="instanceKey"></param>
        void UpgradeUserSettings(Type settingsClass, string instanceKey);
    }
}
