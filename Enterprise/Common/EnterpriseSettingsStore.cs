#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Common.Configuration;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// This class is an implementation of <see cref="ISettingsStore"/> that uses a <see cref="IConfigurationService"/>
    /// as a back-end storage.
    /// </summary>
    [ExtensionOf(typeof(SettingsStoreExtensionPoint))]
    public class EnterpriseSettingsStore : ISettingsStore
    {
        public EnterpriseSettingsStore()
        {
        }

        #region ISettingsStore Members

        public Dictionary<string, string> GetSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey)
        {
            Dictionary<string, string> values = new Dictionary<string,string>();

			Platform.GetService<Configuration.IConfigurationService>(
				delegate(Configuration.IConfigurationService service)
                {
                    SettingsParser parser = new SettingsParser();

                    string sharedDocument = service.GetConfigurationDocument(
						new GetConfigurationDocumentRequest(
							new ConfigurationDocumentKey(group.Name, group.Version, null, instanceKey))).Content;

                    parser.FromXml(sharedDocument, values);

                    // if the group contains user-scoped settings, get the user document
                    // and overwrite any values with the user's values
                    if(group.HasUserScopedSettings)
                    {
                        string userDocument = service.GetConfigurationDocument(
									new GetConfigurationDocumentRequest(
										new ConfigurationDocumentKey(group.Name, group.Version, user, instanceKey))).Content;
                        parser.FromXml(userDocument, values);
                    }
                });

            return values;
        }

        public void PutSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey, Dictionary<string, string> dirtyValues)
        {
            // note: if user == null, we are saving shared settings, if user is valued, we are saving user settings
            // but both are never edited as a single operation

            // the approach taken here is to create an XML document that represents a diff between
            // the default settings (as specified by the settings group meta-data) and the modified settings,
            // and store that document in the configuration store

            // the reason for storing only the diff is that, when a new version of the plugin(s) is deployed,
            // the defaults of the new version should take precedence over the defaults of the old version
            // but should not take precedence over the stored values
            // the only way to make this distinction is to not store any values that are same as default


            // first obtain the meta-data for the settings group properties
            IList<SettingsPropertyDescriptor> properties = this.ListSettingsProperties(group);

			Platform.GetService<Configuration.IConfigurationService>(
				delegate(Configuration.IConfigurationService service)
                {
                    SettingsParser parser = new SettingsParser();
                    Dictionary<string, string> values = new Dictionary<string, string>();

                    // next we obtain any previously stored configuration document for this settings group
                    string document = service.GetConfigurationDocument(
						new GetConfigurationDocumentRequest(
							new ConfigurationDocumentKey(group.Name, group.Version, user, instanceKey))).Content;
                    parser.FromXml(document, values);

                    // update the values that have changed
                    foreach (KeyValuePair<string, string> kvp in dirtyValues)
                    {
                        values[kvp.Key] = kvp.Value;
                    }

                    // now remove any values that are identical to the default values
                    foreach (SettingsPropertyDescriptor property in properties)
                    {
                        string value;
                        if(values.TryGetValue(property.Name, out value))
                        {
                            if (value.Equals(property.DefaultValue))
                                values.Remove(property.Name);
                        }
                    }

                    // generate the document and save it
                    document = parser.ToXml(values);
                    service.SetConfigurationDocument(
						new SetConfigurationDocumentRequest(
							new ConfigurationDocumentKey(group.Name, group.Version, user, instanceKey), document));
                });
        }

        public void RemoveUserSettings(SettingsGroupDescriptor group, string user, string instanceKey)
        {
            Platform.CheckForNullReference(user, "user");

			Platform.GetService<Configuration.IConfigurationService>(
				delegate(Configuration.IConfigurationService service)
                {
                    service.RemoveConfigurationDocument(
						new RemoveConfigurationDocumentRequest(
							new ConfigurationDocumentKey(group.Name, group.Version, user, instanceKey)));
                });
        }

        public void UpgradeUserSettings(SettingsGroupDescriptor group, string user, string instanceKey)
        {
            // TODO implement this later
            //throw new NotImplementedException();
        }

        public IList<SettingsGroupDescriptor> ListSettingsGroups()
        {
            List<SettingsGroupDescriptor> groups = null;

            // obtain the list of settings groups from the configuration service
			Platform.GetService<Configuration.IConfigurationService>(
				delegate(Configuration.IConfigurationService service)
                {
                	groups = service.ListSettingsGroups(new ListSettingsGroupsRequest()).Groups;
                });

            return groups;
        }

        public IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group)
        {
            // use the configuration service to obtain the properties
            IList<SettingsPropertyDescriptor> properties = null;
			Platform.GetService<Configuration.IConfigurationService>(
				delegate(Configuration.IConfigurationService service)
                {
                	properties = service.ListSettingsProperties(new ListSettingsPropertiesRequest(group)).Properties;
                });
            return properties;
        }

        public bool SupportsImport
        {
            get { return true; }
        }

        public void ImportSettingsGroup(SettingsGroupDescriptor group, List<SettingsPropertyDescriptor> properties)
        {
            Platform.GetService<Configuration.IConfigurationService>(
                delegate(Configuration.IConfigurationService service)
                {
                    service.ImportSettingsGroup(
                        new ImportSettingsGroupRequest(group, properties));
                });
        }

        #endregion
    }
}
