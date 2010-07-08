﻿#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Common.Configuration;
using System.Configuration;

namespace ClearCanvas.Desktop.Configuration
{
	/// <summary>
	/// The LocalSettingsStore, although it implements <see cref="ISettingsStore "/> does not serve 
	/// as a proper settings store for the <see cref="StandardSettingsProvider"/> (notice that it is not
	/// an extension of <see cref="SettingsStoreExtensionPoint"/>.  Instead, this class is instantiated
	/// directly by the <see cref="SettingsManagementComponent"/> when there are no such extensions available,
	/// and the application is using the <see cref="LocalFileSettingsProvider"/> (or app/user .config) to 
	/// store settings locally.  This 'settings store' is used solely to edit the default profile
	/// throught the settings management UI.
	/// </summary>
	internal class LocalSettingsStore : ISettingsStore
	{
	    private readonly System.Configuration.Configuration _configuration;

		/// <summary>
		/// Constructor.
		/// </summary>
		public LocalSettingsStore()
		{
            _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

		#region ISettingsStore Members
		
		/// <summary>
		/// Loads the settings values (both application and user scoped) for a given settings class.  Only the shared profile
		/// is supported (application settings + default user settings).
		/// </summary>
        /// <param name="group">the settings class for which to retrieve the defaults</param>
		/// <param name="user">must be null or ""</param>
		/// <param name="instanceKey">must be null or ""</param>
        /// <returns>returns only those values that are different from the property defaults</returns>
		/// <exception cref="NotSupportedException">will be thrown if the user or instance key is specified</exception>
        public Dictionary<string, string> GetSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey)
		{
		    CheckUser(user);
            CheckInstanceKey(instanceKey);

			Type type = Type.GetType(group.AssemblyQualifiedTypeName, true);
            return SystemConfigurationHelper.GetSettingsValues(_configuration, type);
		}

		/// <summary>
		/// Stores the settings values (both application and user scoped) for a given settings class.  Only the shared profile
		/// is supported (application settings + default user settings).
		/// </summary>
        /// <param name="group">the settings class for which to store the values</param>
		/// <param name="user">must be null or ""</param>
        /// <param name="instanceKey">must be null or ""</param>
		/// <param name="dirtyValues">contains the values to be stored</param>
		/// <exception cref="NotSupportedException">will be thrown if the user or instance key is specified</exception>
        public void PutSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey, Dictionary<string, string> dirtyValues)
		{
            CheckUser(user);
            CheckInstanceKey(instanceKey);

			Type type = Type.GetType(group.AssemblyQualifiedTypeName, true);
            SystemConfigurationHelper.PutSettingsValues(_configuration, type, dirtyValues);
			}

		/// <summary>
		/// Unsupported.  An exception will always be thrown.
		/// </summary>
        /// <param name="group"></param>
		/// <param name="user"></param>
		/// <param name="instanceKey"></param>
		/// <exception cref="NotSupportedException">always thrown</exception>
        public void RemoveUserSettings(SettingsGroupDescriptor group, string user, string instanceKey)
		{
            throw new NotSupportedException("Local settings store does not support removal of user settings.");
		}

		/// <summary>
        /// Returns settings groups installed on local machine
        /// </summary>
        /// <returns></returns>
        public IList<SettingsGroupDescriptor> ListSettingsGroups()
        {
            return SettingsGroupDescriptor.ListInstalledSettingsGroups(false);
        }

		public SettingsGroupDescriptor GetPreviousSettingsGroup(SettingsGroupDescriptor group)
		{
			throw new NotSupportedException("Local settings store does not support retrieval of previous settings group.");
		}

        /// <summary>
        /// Returns settings properties for specified group, assuming plugin containing group resides on local machine
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group)
        {
            return SettingsPropertyDescriptor.ListSettingsProperties(group);
        }

        /// <summary>
        /// Local store does not support import.
        /// </summary>
        public bool SupportsImport
        {
            get { return false; }
        }

        /// <summary>
        /// Local store does not support import.
        /// </summary>
        public void ImportSettingsGroup(SettingsGroupDescriptor group, List<SettingsPropertyDescriptor> properties)
        {
            throw new NotSupportedException("Local settings store does not support the Import operation.");
        }

        #endregion

        private static void CheckUser(string user)
        {
            if (!String.IsNullOrEmpty(user))
                throw new NotSupportedException("Only the default profile is currently supported.  " + 
                    "User profiles must be modified via standard means (e.g. Tools/Options).");

    }

        private static void CheckInstanceKey(string instanceKey)
        {
            if (!String.IsNullOrEmpty(instanceKey))
                throw new NotSupportedException("Settings instance keys are not supported.");
        }
	}
}
