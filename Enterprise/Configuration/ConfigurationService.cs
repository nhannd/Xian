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
        public void LoadSettingsValues(string group, Version version, string user, string instanceKey, IDictionary<string, string> values)
        {
            try
            {
                IConfigSettingsInstanceBroker broker = CurrentContext.GetBroker<IConfigSettingsInstanceBroker>();
                ConfigSettingsInstanceSearchCriteria criteria = BuildCurrentVersionCriteria(group, version, user, instanceKey);
                ConfigSettingsInstance settings = broker.FindOne(criteria);
                settings.GetValues(values);
            }
            catch (EntityNotFoundException)
            {
                // no saved values - nothing to do
            }
        }



        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [UpdateOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public void SaveSettingsValues(string group, Version version, string user, string instanceKey, IDictionary<string, string> values)
        {
            ConfigSettingsInstance settings = null;
            try
            {
                IConfigSettingsInstanceBroker broker = CurrentContext.GetBroker<IConfigSettingsInstanceBroker>();
                ConfigSettingsInstanceSearchCriteria criteria = BuildCurrentVersionCriteria(group, version, user, instanceKey);
                settings = broker.FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
                // no saved settings, create new
                settings = NewSettingsInstance(group, version, user, instanceKey);

                CurrentContext.Lock(settings, DirtyState.New);
            }

            // save the values
            settings.SetValues(values);
        }

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [UpdateOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public void UpgradeFromPreviousVersion(string group, Version version, string user, string instanceKey, IDictionary<string, string> values)
        {
            IConfigSettingsInstanceBroker broker = CurrentContext.GetBroker<IConfigSettingsInstanceBroker>();
            ConfigSettingsInstanceSearchCriteria criteria = BuildCurrentAndPerviousVersionsCriteria(group, version, user, instanceKey);

            // query for up to 2 instances, the current version and the immediately previous version
            // need to sort by version, descending, to ensure that we in fact get the current and immediately previous version
            criteria.GroupVersionString.SortDesc(0);    
            IList<ConfigSettingsInstance> instances = broker.Find(criteria, new SearchResultPage(0, 2));

            ConfigSettingsInstance previousVersion = CollectionUtils.SelectFirst<ConfigSettingsInstance>(instances,
                delegate(ConfigSettingsInstance i) { return VersionUtils.FromPaddedVersionString(i.GroupVersionString) < version; });

            // if there is no previous version, then there is nothing to upgrade
            if (previousVersion != null)
            {
                ConfigSettingsInstance currentVersion = CollectionUtils.SelectFirst<ConfigSettingsInstance>(instances,
                    delegate(ConfigSettingsInstance i) { return VersionUtils.FromPaddedVersionString(i.GroupVersionString) == version; });

                // if the current version does not exist, create it
                if (currentVersion == null)
                {
                    currentVersion = NewSettingsInstance(group, version, user, instanceKey);
                    CurrentContext.Lock(currentVersion, DirtyState.New);
                }

                // upgrade the current version from the previous
                currentVersion.UpgradeFrom(previousVersion);

                // place the latest values into the dictionary
                currentVersion.GetValues(values);
            }
        }

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [UpdateOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public void Clear(string group, Version version, string user, string instanceKey)
        {
            try
            {
                IConfigSettingsInstanceBroker broker = CurrentContext.GetBroker<IConfigSettingsInstanceBroker>();
                ConfigSettingsInstanceSearchCriteria criteria = BuildCurrentVersionCriteria(group, version, user, instanceKey);
                ConfigSettingsInstance settings = broker.FindOne(criteria);
                settings.Clear();
            }
            catch (EntityNotFoundException)
            {
                // no saved values - nothing to do
            }
        }

        #endregion

        private ConfigSettingsInstance NewSettingsInstance(string groupName, Version version, string user, string instanceKey)
        {
            // force an empty instanceKey to null
            if (instanceKey != null && instanceKey.Length == 0)
                instanceKey = null;

            return new ConfigSettingsInstance(groupName, VersionUtils.ToPaddedVersionString(version), user, instanceKey, null);
        }

        private ConfigSettingsInstanceSearchCriteria BuildCurrentVersionCriteria(string groupName, Version version, string user, string instanceKey)
        {
            ConfigSettingsInstanceSearchCriteria criteria = BuildUnversionedCriteria(groupName, user, instanceKey);
            criteria.GroupVersionString.EqualTo(VersionUtils.ToPaddedVersionString(version));
            return criteria;
        }

        private ConfigSettingsInstanceSearchCriteria BuildCurrentAndPerviousVersionsCriteria(string groupName, Version version, string user, string instanceKey)
        {
            ConfigSettingsInstanceSearchCriteria criteria = BuildUnversionedCriteria(groupName, user, instanceKey);
            criteria.GroupVersionString.LessThanOrEqualTo(VersionUtils.ToPaddedVersionString(version));
            return criteria;
        }

        private ConfigSettingsInstanceSearchCriteria BuildUnversionedCriteria(string groupName, string user, string instanceKey)
        {
            ConfigSettingsInstanceSearchCriteria criteria = new ConfigSettingsInstanceSearchCriteria();
            criteria.GroupName.EqualTo(groupName);

            if (instanceKey != null && instanceKey.Length > 0)
            {
                criteria.InstanceKey.EqualTo(instanceKey);
            }
            else
            {
                criteria.InstanceKey.IsNull();
            }

            if (user != null && user.Length > 0)
            {
                criteria.User.EqualTo(user);
            }
            else
            {
                criteria.User.IsNull();
            }

            return criteria;
        }

    }
}
