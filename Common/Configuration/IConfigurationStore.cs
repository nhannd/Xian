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
        /// Lists all settings groups for which this configuration store maintains settings values.  Generally
        /// this corresponds to the the list of all types derived from <see cref="ApplicationSettingsBase"/> found
        /// in all installed plugins and related assemblies.
        /// </summary>
        /// <returns></returns>
        IList<SettingsGroupDescriptor> ListSettingsGroups();

        /// <summary>
        /// Lists the settings properties for the specified settings group.
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group);


        /// <summary>
        /// Obtains the settings values for the specified settings class, user and instance key.
        /// Only returns values for settings that differ from the default value as specified by the settings class.
        /// </summary>
        /// <param name="settingsClass"></param>
        /// <param name="user"></param>
        /// <param name="instanceKey"></param>
        Dictionary<string, string> LoadSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey);

        /// <summary>
        /// Store the settings values for the specified settings class, for the current user and
        /// specified instance key.  The dictionary should contain only values that differ from the default
        /// values as specified by the settings class, as only these values should be stored.  Any previously
        /// stored settings values that are not contained in the dictionary will be removed from the store.
        /// </summary>
        /// <param name="settingsClass"></param>
        /// <param name="user"></param>
        /// <param name="instanceKey"></param>
        /// <param name="values"></param>
        void SaveSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey, Dictionary<string, string> values);

        /// <summary>
        /// Removes user settings from this group, effectively causing them to be reset to their shared default
        /// values.  Application-scoped settings are unaffected.
        /// </summary>
        /// <param name="settingsClass"></param>
        /// <param name="user"></param>
        /// <param name="instanceKey"></param>
        void RemoveUserSettings(SettingsGroupDescriptor group, string user, string instanceKey);

        /// <summary>
        /// Upgrades user settings in the group, effectively importing any settings saved in a previous version
        /// of the application into the current version.  Application-scoped settings are unaffected.
        /// </summary>
        /// <param name="settingsClass"></param>
        /// <param name="user"></param>
        /// <param name="instanceKey"></param>
        void UpgradeUserSettings(SettingsGroupDescriptor group, string user, string instanceKey);
    }
}
