#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
