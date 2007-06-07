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

            // HACK: add local groups
            // this is probably a bad security practice in a production environment, because local plugins may not be trusted
            // but for development it makes things easier
            IList<SettingsGroupDescriptor> localGroups = SettingsGroupDescriptor.ListInstalledSettingsGroups();
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
