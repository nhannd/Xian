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
using System.Collections.Specialized;

using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using System.Threading;

namespace ClearCanvas.Common.Configuration
{
    /// <summary>
    /// Communicates with an <see cref="IConfigurationStore"/> to manage loading and saving of
    /// settings from the store.
    /// </summary>
    /// <remarks>
	/// Supports the framework and is not intended for use by application code.  Use 
	/// <see cref="StandardSettingsProvider"/> instead.
	/// </remarks>
    internal class ConfigurationStoreSettingsProvider : SettingsProvider, IApplicationSettingsProvider
    {
        private string _appName;
        private IConfigurationStore _store;

        internal ConfigurationStoreSettingsProvider(IConfigurationStore store)
        {
            _store = store;
        }

        /// <summary>
        /// Gets the Application Name used to initialize the settings subsystem.
        /// </summary>
		public override string ApplicationName
        {
            get
            {
                return _appName;
            }
            set
            {
                _appName = value;
            }
        }

    	///<summary>
    	///Returns the collection of settings property values for the specified application instance and settings property group.
    	///</summary>
    	public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection props)
        {
            Type settingsClass = (Type)context["SettingsClassType"];
            string settingsKey = (string)context["SettingsKey"];
            string user = Thread.CurrentPrincipal.Identity.Name;

            // load shared values
            Dictionary<string, string> storedValues = _store.LoadSettingsValues(
                new SettingsGroupDescriptor(settingsClass), null, settingsKey);

            // load user values
            Dictionary<string, string> userValues = _store.LoadSettingsValues(
                new SettingsGroupDescriptor(settingsClass), user, settingsKey);

            // overwrite shared values with user values, if they exist
            foreach (KeyValuePair<string, string> kvp in userValues)
            {
                storedValues[kvp.Key] = kvp.Value;
            }

            // Create new collection of values
            SettingsPropertyValueCollection values = new SettingsPropertyValueCollection();

            // Iterate through the settings to be retrieved
            foreach (SettingsProperty setting in props)
            {
                SettingsPropertyValue value = new SettingsPropertyValue(setting);
                value.IsDirty = false;

                // use the stored value, or set the SerializedValue to null, which tells .NET to use the default value
                value.SerializedValue = storedValues.ContainsKey(setting.Name) ? storedValues[setting.Name] : null;
                values.Add(value);
            }
 
            return values;
        }

    	///<summary>
    	///Sets the values of the specified group of property settings.
    	///</summary>
    	public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection settings)
        {
            Dictionary<string, string> valuesToStore = new Dictionary<string, string>();

            foreach (SettingsPropertyValue value in settings)
            {
                // only user scoped settings should be saved (changes to app settings are made directly
                // via the IEnterpriseConfigurationStore and not through this class)

                // only values that are different than the default should be persisted to the store
                // the reason is that, when a new version of the settings class is deployed,
                // the defaults of the new version should take precedence over the defaults of the old version
                // but should not take precedence over the stored values
                // the only way to make this distinction is to not store any values that are same as default
                if(IsUserScoped(value.Property) && !value.UsingDefaultValue)
                {
                    valuesToStore[value.Name] = (string)value.SerializedValue;
                }
            }

            Type settingsClass = (Type)context["SettingsClassType"];
            string settingsKey = (string)context["SettingsKey"];
            string user = Thread.CurrentPrincipal.Identity.Name;

            // must call this method even if valuesToStore is empty (in which case the stored values are cleared)
            _store.SaveSettingsValues(new SettingsGroupDescriptor(settingsClass), user, settingsKey, valuesToStore);
        }

        private bool IsUserScoped(SettingsProperty settingsProperty)
        {
            return CollectionUtils.Contains(settingsProperty.Attributes.Values,
                delegate(object obj) { return obj is UserScopedSettingAttribute; });
        }

        #region IApplicationSettingsProvider Members

        /// <summary>
        /// Not implemented.
        /// </summary>
        public SettingsPropertyValue GetPreviousVersion(SettingsContext context, SettingsProperty property)
        {
            // seems like implementing this method would be quite inefficient, unless we could be sure that
            // the IConfigurationStore implementation had sufficient optimizations in place
            // let's leave this to be implemented "as needed"
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Resets all settings back to the defaults.
        /// </summary>
        /// <remarks>
		/// Note that this implementation resets the user-scoped settings only.  It does not touch application-scoped settings.
		/// </remarks>
        public void Reset(SettingsContext context)
        {
            Type settingsClass = (Type)context["SettingsClassType"];
            string settingsKey = (string)context["SettingsKey"];
            string user = Thread.CurrentPrincipal.Identity.Name;

            _store.RemoveUserSettings(new SettingsGroupDescriptor(settingsClass), user, settingsKey);
        }

        /// <summary>
        /// Upgrades the settings from a previous version.
        /// </summary>
		/// <remarks>
		/// Note that this implementation upgrades user-scoped settings only, and it upgrades all settings in the group,
        /// regardless of the contents of the specified properties collection.  It does not touch application-scoped settings.
		/// </remarks>
        public void Upgrade(SettingsContext context, SettingsPropertyCollection properties)
        {
            Type settingsClass = (Type)context["SettingsClassType"];
            string settingsKey = (string)context["SettingsKey"];
            string user = Thread.CurrentPrincipal.Identity.Name;

            // here we just upgrade the settings in the store... the .net framework will call GetPropertyValues again
            // to obtain the new values
            _store.UpgradeUserSettings(new SettingsGroupDescriptor(settingsClass), user, settingsKey);
        }

        #endregion
    }
}
