#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.ImageViewer.Common.Configuration
{
    public interface ISystemConfigurationSettingsStore
    {
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
    }
}
