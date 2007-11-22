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
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using System.ServiceModel;
using System.ServiceModel.Security;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Implementation of <see cref="IConfigurationStore"/>, extends <see cref="ConfigurationStoreExtensionPoint"/>.
    /// Acts as a client-side proxy to <see cref="IConfigurationService"/>
    /// </summary>
    [ExtensionOf(typeof(ConfigurationStoreExtensionPoint))]
    public class EnterpriseConfigurationStore : IConfigurationStore
    {
        private IList<SettingsGroupDescriptor> _remoteGroups;

        public EnterpriseConfigurationStore()
        {
        }

        #region IConfigurationStore Members

        public Dictionary<string, string> LoadSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey)
        {
            Dictionary<string, string> values = new Dictionary<string,string>();

            Platform.GetService<IConfigurationService>(
                delegate(IConfigurationService service)
                {
                    values = service.LoadSettingsValues(group.Name, group.Version, user, instanceKey);
                });

            return values;
        }

        public void SaveSettingsValues(SettingsGroupDescriptor group, string user, string instanceKey, Dictionary<string, string> values)
        {
            Platform.GetService<IConfigurationService>(
                delegate(IConfigurationService service)
                {
                    service.SaveSettingsValues(group.Name, group.Version, user, instanceKey, values);
                });
        }

        public void RemoveUserSettings(SettingsGroupDescriptor group, string user, string instanceKey)
        {
            Platform.GetService<IConfigurationService>(
                delegate(IConfigurationService service)
                {
                    service.RemoveSettingsValues(
                        group.Name,
                        group.Version,
                        user,
                        instanceKey);
                });
        }

        public void UpgradeUserSettings(SettingsGroupDescriptor group, string user, string instanceKey)
        {
            // TODO implement this later
            throw new NotImplementedException();
        }

        public IList<SettingsGroupDescriptor> ListSettingsGroups()
        {
            // init remote groups if not initialized
            if (_remoteGroups == null)
            {
                _remoteGroups = ListSettingsGroupsRemote();
            }

            // add remote groups
            List<SettingsGroupDescriptor> groups = new List<SettingsGroupDescriptor>(_remoteGroups);

            // add local groups
            // note that local settings groups are excluded (ListInstalledSettingsGroups(true)) because they
            // are not stored in the enterprise configuration store
            IList<SettingsGroupDescriptor> localGroups = SettingsGroupDescriptor.ListInstalledSettingsGroups(true);
            foreach (SettingsGroupDescriptor group in localGroups)
            {
                if (!groups.Contains(group))
                    groups.Add(group);
            }

            return groups;
        }

        public IList<SettingsPropertyDescriptor> ListSettingsProperties(SettingsGroupDescriptor group)
        {
            // init remote groups if not initialized
            if (_remoteGroups == null)
            {
                _remoteGroups = ListSettingsGroupsRemote();
            }

            // if the group is remote, get properties from remote
            if (_remoteGroups.Contains(group))
            {
                return ListSettingsPropertiesRemote(group);
            }
            else
            {
                // group is local
                return SettingsPropertyDescriptor.ListSettingsProperties(group);
            }
        }

        #endregion

        private IList<SettingsGroupDescriptor> ListSettingsGroupsRemote()
        {
            List<SettingsGroupInfo> groups = null;

            Platform.GetService<IConfigurationService>(
                delegate(IConfigurationService service)
                {
                    groups = service.ListSettingsGroups();
                });

            return CollectionUtils.Map<SettingsGroupInfo, SettingsGroupDescriptor, List<SettingsGroupDescriptor>>(
                groups,
                delegate(SettingsGroupInfo info)
                {
                    return info.ToDescriptor();
                });
        }

        private IList<SettingsPropertyDescriptor> ListSettingsPropertiesRemote(SettingsGroupDescriptor group)
        {
            List<SettingsPropertyInfo> properties = null;

            Platform.GetService<IConfigurationService>(
                delegate(IConfigurationService service)
                {
                    properties = service.ListSettingsProperties(new SettingsGroupInfo(group));
                });

            return CollectionUtils.Map<SettingsPropertyInfo, SettingsPropertyDescriptor, List<SettingsPropertyDescriptor>>(
                properties,
                delegate(SettingsPropertyInfo info)
                {
                    return info.ToDescriptor();
                });
        }
    }
}
