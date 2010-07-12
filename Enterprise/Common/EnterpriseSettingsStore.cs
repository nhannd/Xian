#region License

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
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Common.Utilities;

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
			Type serviceContract = !String.IsNullOrEmpty(user) ?
				typeof(IConfigurationService) : typeof(IApplicationConfigurationReadService);

			var service = (IApplicationConfigurationReadService)Platform.GetService(serviceContract);
            using (service as IDisposable)
            {
				var values = new Dictionary<string, string>();
				var parser = new SettingsParser();

				var userDocument = service.GetConfigurationDocument(
                    new GetConfigurationDocumentRequest(
                                    new ConfigurationDocumentKey(group.Name, group.Version, user, instanceKey))).Content;
                    parser.FromXml(userDocument, values);
                return values;
            }
        }

        public void PutSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey, Dictionary<string, string> dirtyValues)
        {
            // note: if user == null, we are saving shared settings, if user is valued, we are saving user settings
            // but both are never edited as a single operation

            // the approach taken here is to create an XML document that represents a diff between
            // the default settings (as specified by the settings group meta-data) and the modified settings,
            // and store that document in the configuration store

			Platform.GetService(delegate(IConfigurationService service)
                {
                    // first obtain the meta-data for the settings group properties
                    IList<SettingsPropertyDescriptor> properties = ListSettingsProperties(group, service);

                    var parser = new SettingsParser();
                    var values = new Dictionary<string, string>();

					var documentKey = new ConfigurationDocumentKey(group.Name, group.Version, user, instanceKey);

                    // next we obtain any previously stored configuration document for this settings group
					string document = service.GetConfigurationDocument(new GetConfigurationDocumentRequest(documentKey)).Content;
                    parser.FromXml(document, values);

                    // update the values that have changed
                    foreach (KeyValuePair<string, string> kvp in dirtyValues)
                        values[kvp.Key] = kvp.Value;

                    // now remove any values that are identical to the default values
                    foreach (SettingsPropertyDescriptor property in properties)
                    {
                        string value;
                        if(values.TryGetValue(property.Name, out value))
                        {
							if (Equals(value, property.DefaultValue))
                                values.Remove(property.Name);
                        }
                    }

					if (values.Count > 0)
					{
                    // generate the document and save it
                    document = parser.ToXml(values);
						service.SetConfigurationDocument(new SetConfigurationDocumentRequest(documentKey, document));
					}
					else
					{
						// every value is the same as the default, so the document can be removed
						service.RemoveConfigurationDocument(new RemoveConfigurationDocumentRequest(documentKey));
					}
                });
        }

        public void RemoveUserSettings(SettingsGroupDescriptor group, string user, string instanceKey)
        {
            Platform.CheckForNullReference(user, "user");

        	var documentKey = new ConfigurationDocumentKey(group.Name, group.Version, user, instanceKey);
			Platform.GetService( (IConfigurationService service) => service.RemoveConfigurationDocument(new RemoveConfigurationDocumentRequest(documentKey)));
        }

        public IList<SettingsGroupDescriptor> ListSettingsGroups()
        {
            List<SettingsGroupDescriptor> groups = null;

            // obtain the list of settings groups
            Platform.GetService((IApplicationConfigurationReadService service) => groups = ListSettingsGroups(service));

            return groups;
        }

		public SettingsGroupDescriptor GetPreviousSettingsGroup(SettingsGroupDescriptor group)
		{
			List<SettingsGroupDescriptor> groups = null;
			ListSettingsGroupsRequest request = new ListSettingsGroupsRequest();

			//TODO/NOTE: I had changed the service interface so the server would return the previous group, but in order to
			//not have to release enterprise for Summer, I just did this.
			Platform.GetService((IApplicationConfigurationReadService service) => groups = service.ListSettingsGroups(request).Groups);

			if (groups == null || groups.Count == 0)
				return null;

			var sameGroup = CollectionUtils.Select(groups, (other) => other.AssemblyQualifiedTypeName == group.AssemblyQualifiedTypeName);
			var lessEqualGroups = CollectionUtils.Select(sameGroup, (other) => other.Version <= group.Version);
			if (lessEqualGroups.Count < 2)
				return null;

			//Sort ascending.
			lessEqualGroups.Sort((other1, other2) => other1.Version.CompareTo(other2.Version));
			//The current version is last, the previous is second last.
			return lessEqualGroups[lessEqualGroups.Count - 2];
		}

        public IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group)
        {
            // use the configuration service to obtain the properties
            IList<SettingsPropertyDescriptor> properties = null;
			Platform.GetService((IApplicationConfigurationReadService service) => properties = ListSettingsProperties(group, service));
            return properties;
        }
		
        public bool SupportsImport
        {
            get { return true; }
        }

        public void ImportSettingsGroup(SettingsGroupDescriptor group, List<SettingsPropertyDescriptor> properties)
        {
            Platform.GetService((IConfigurationService service) => 
									service.ImportSettingsGroup(new ImportSettingsGroupRequest(group, properties)));
        }

        #endregion

        private static List<SettingsGroupDescriptor> ListSettingsGroups(IApplicationConfigurationReadService service)
        {
            return service.ListSettingsGroups(new ListSettingsGroupsRequest()).Groups;
        }
        private static IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group, IApplicationConfigurationReadService service)
        {
            return service.ListSettingsProperties(new ListSettingsPropertiesRequest(group)).Properties;
        }
    }
}
