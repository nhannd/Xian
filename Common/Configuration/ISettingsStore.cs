#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Configuration;

namespace ClearCanvas.Common.Configuration
{
    /// <summary>
    /// Defines the interface to a mechanism for the storage of application and user settings.
    /// </summary>
    /// <remarks>
    /// This interface is more specialized than <see cref="IConfigurationStore"/>, in that it is designed
    /// specifically to support derivatives of the <see cref="SettingsProvider"/> class in order to support the .NET 
    /// settings framework.
    /// </remarks>
    public interface ISettingsStore
    {
        /// <summary>
        /// Gets a value indicating whether this store supports importing of meta-data.
        /// </summary>
        bool SupportsImport { get; }

        /// <summary>
        /// Lists all settings groups for which this store maintains settings values.
        /// </summary>
        /// <remarks>
        /// Generally this corresponds to the the list of all types derived from <see cref="ApplicationSettingsBase"/> found
		/// in all installed plugins and related assemblies.
		/// </remarks>
        IList<SettingsGroupDescriptor> ListSettingsGroups();

        /// <summary>
		/// Gets the settings group that immediately precedes the one provided.
		/// </summary>
		SettingsGroupDescriptor GetPreviousSettingsGroup(SettingsGroupDescriptor group);

        /// <summary>
        /// Lists the settings properties for the specified settings group.
        /// </summary>
        IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group);

        /// <summary>
        /// Imports meta-data for the specified settings group and its properties.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="properties"></param>
        void ImportSettingsGroup(SettingsGroupDescriptor group, List<SettingsPropertyDescriptor> properties);

        /// <summary>
        /// Obtains the settings values for the specified settings group, user and instance key.  If user is null,
        /// the shared settings are obtained.
        /// </summary>
        /// <remarks>
		/// The returned dictionary may contain values for all settings in the group, or it may
		/// contain only those values that differ from the default values defined by the settings group.
        /// </remarks>
        Dictionary<string, string> GetSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey);

        /// <summary>
        /// Store the settings values for the specified settings group, for the current user and
        /// specified instance key.  If user is null, the values are stored as shared settings.
        /// </summary>
        /// <remarks>
        /// The <paramref name="dirtyValues"/> dictionary should contain values for any settings that are dirty.
		/// </remarks>
        void PutSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey, Dictionary<string, string> dirtyValues);

        /// <summary>
        /// Removes user settings from this group, effectively causing them to be reset to their shared default
        /// values.
        /// </summary>
        /// <remarks>
		/// Application-scoped settings are unaffected.
		/// </remarks>
        void RemoveUserSettings(SettingsGroupDescriptor group, string user, string instanceKey);
    }
}
