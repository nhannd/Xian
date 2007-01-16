using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Configuration
{
    /// <summary>
    /// Defines a service for saving/retrieving configuration data to/from a persistent store.
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Loads settings values for the specified group, version, user and instance key, placing the results in
        /// the specified dictionary.  The user may be null, in which case the shared settings defaults are returned.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="version"></param>
        /// <param name="user"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        void LoadSettingsValues(string group, Version version, string user, string instanceKey, IDictionary<string, string> values);

        /// <summary>
        /// Stores the settings values for the specified group, version, user and instance key.  The user may be null,
        /// in which case the values are stored as shared defaults.  Note that the values in the dictionary completely overwrite
        /// any previously stored values (e.g. an empty dictionary will effectively erase any stored settings).
        /// </summary>
        /// <param name="group"></param>
        /// <param name="version"></param>
        /// <param name="user"></param>
        /// <param name="instanceKey"></param>
        /// <param name="values"></param>
        void SaveSettingsValues(string group, Version version, string user, string instanceKey, IDictionary<string, string> values);

        /// <summary>
        /// Upgrades *to* the specified version by finding the immediately preceding version and overwriting any values in the 
        /// specified version with those of the previous version, if they exist, and then saving the specified version.  The values
        /// returned in the dictionary reflect the latest values of the settings, and can be used as if they were returned
        /// from <see cref="LoadSettingsValues"/>
        /// </summary>
        /// <param name="group"></param>
        /// <param name="version"></param>
        /// <param name="user"></param>
        /// <param name="instanceKey"></param>
        /// <param name="values"></param>
        void UpgradeFromPreviousVersion(string group, Version version, string user, string instanceKey, IDictionary<string, string> values);

        /// <summary>
        /// Clears the stored settings values for the specified group, version, user and instance key.  The user may be null,
        /// in which case the values cleared are the shared defaults.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="version"></param>
        /// <param name="user"></param>
        /// <param name="instanceKey"></param>
        void Clear(string group, Version version, string user, string instanceKey);

    }
}
