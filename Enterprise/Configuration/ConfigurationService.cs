using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Configuration.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Configuration
{
    [ExtensionOf(typeof(ServiceLayerExtensionPoint))]
    public class ConfigurationService : ConfigurationServiceLayer, IConfigurationService
    {
        #region IConfigurationService Members

        [ReadOperation]
        public ConfigSettingsInstance LoadConfigSettings(string groupName, string version, string user, string instanceKey)
        {
            // if the instance key is an empty string, convert it to null
            if (instanceKey != null && instanceKey.Length == 0)
                instanceKey = null;

            ConfigSettingsGroupSearchCriteria groupCriteria = new ConfigSettingsGroupSearchCriteria();
            groupCriteria.GroupName.EqualTo(groupName);
            groupCriteria.PluginVersion.EqualTo(version);

            ConfigSettingsInstanceSearchCriteria instanceCriteria = new ConfigSettingsInstanceSearchCriteria();
            instanceCriteria.User.EqualTo(user);
            if (instanceKey != null)
            {
                instanceCriteria.InstanceKey.EqualTo(instanceKey);
            }
            else
            {
                instanceCriteria.InstanceKey.IsNull();
            }

            // try to load a settings instance for the given user and instanceKey
            IConfigSettingsInstanceBroker broker = CurrentContext.GetBroker<IConfigSettingsInstanceBroker>();
            IList<ConfigSettingsInstance> instances = broker.Find(groupCriteria, instanceCriteria);
            if (instances.Count > 0)
            {
                return instances[0];
            }
            else
            {
                // if no such instance found, then just load the group and create a new instance
                IConfigSettingsGroupBroker groupBroker = CurrentContext.GetBroker<IConfigSettingsGroupBroker>();
                ConfigSettingsGroup group = groupBroker.FindOne(groupCriteria);

                return new ConfigSettingsInstance(group, user, instanceKey);
            }
        }


        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [UpdateOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public void SaveConfigSettings(ConfigSettingsInstance settings)
        {
            settings.PrepareSave();
            CurrentContext.Lock(settings, settings.Unsaved ? DirtyState.New : DirtyState.Dirty);
        }

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [UpdateOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public ConfigSettingsGroup ImportConfigSettingsClass(Type settingsClass)
        {
            SettingsImporter importer = new SettingsImporter();
            string groupName = SettingsClassMetaDataReader.GetGroupName(settingsClass);
            string version = SettingsClassMetaDataReader.GetVersion(settingsClass);

            ConfigSettingsGroup group = null;
            try
            {
                // load the group if it already exists
                ConfigSettingsGroupSearchCriteria criteria = new ConfigSettingsGroupSearchCriteria();
                criteria.GroupName.EqualTo(groupName);
                criteria.PluginVersion.EqualTo(version);

                IConfigSettingsGroupBroker groupBroker = CurrentContext.GetBroker<IConfigSettingsGroupBroker>();
                group = groupBroker.FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
                // if the group does not exist, create it
                group = new ConfigSettingsGroup(groupName, version);
                CurrentContext.Lock(group, DirtyState.New);
            }

            // import the settingsClass
            importer.Import(settingsClass, group);
            group.PrepareSave();

            return group;
        }

        #endregion
    }
}
