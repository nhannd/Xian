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
                IConfigurationDocumentBroker broker = CurrentContext.GetBroker<IConfigurationDocumentBroker>();
                ConfigurationDocumentSearchCriteria criteria = BuildCurrentVersionCriteria(group, version, user, instanceKey);
                ConfigurationDocument settings = broker.FindOne(criteria);

                SettingsParser parser = new SettingsParser();
                parser.FromXml(settings.DocumentText, values);
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
            ConfigurationDocument settings = null;
            try
            {
                IConfigurationDocumentBroker broker = CurrentContext.GetBroker<IConfigurationDocumentBroker>();
                ConfigurationDocumentSearchCriteria criteria = BuildCurrentVersionCriteria(group, version, user, instanceKey);
                settings = broker.FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
                // no saved settings, create new
                settings = NewSettingsInstance(group, version, user, instanceKey);

                CurrentContext.Lock(settings, DirtyState.New);
            }

            // save the values
            SettingsParser parser = new SettingsParser();
            settings.DocumentText = parser.ToXml(values);
        }

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [UpdateOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public void UpgradeFromPreviousVersion(string group, Version version, string user, string instanceKey, IDictionary<string, string> values)
        {
            IConfigurationDocumentBroker broker = CurrentContext.GetBroker<IConfigurationDocumentBroker>();
            ConfigurationDocumentSearchCriteria criteria = BuildCurrentAndPerviousVersionsCriteria(group, version, user, instanceKey);

            // query for up to 2 instances, the current version and the immediately previous version
            // need to sort by version, descending, to ensure that we in fact get the current and immediately previous version
            criteria.DocumentVersionString.SortDesc(0);    
            IList<ConfigurationDocument> instances = broker.Find(criteria, new SearchResultPage(0, 2));

            ConfigurationDocument previousVersion = CollectionUtils.SelectFirst<ConfigurationDocument>(instances,
                delegate(ConfigurationDocument i) { return VersionUtils.FromPaddedVersionString(i.DocumentVersionString) < version; });

            // if there is no previous version, then there is nothing to upgrade
            if (previousVersion != null)
            {
                ConfigurationDocument currentVersion = CollectionUtils.SelectFirst<ConfigurationDocument>(instances,
                    delegate(ConfigurationDocument i) { return VersionUtils.FromPaddedVersionString(i.DocumentVersionString) == version; });

                // if the current version does not exist, create it
                if (currentVersion == null)
                {
                    currentVersion = NewSettingsInstance(group, version, user, instanceKey);
                    CurrentContext.Lock(currentVersion, DirtyState.New);
                }

                SettingsParser parser = new SettingsParser();

                // upgrade the current version from the previous
                parser.UpgradeFromPrevious(currentVersion, previousVersion);

                // place the latest values into the dictionary
                parser.FromXml(currentVersion.DocumentText, values);
            }
        }

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [UpdateOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public void Clear(string group, Version version, string user, string instanceKey)
        {
            try
            {
                IConfigurationDocumentBroker broker = CurrentContext.GetBroker<IConfigurationDocumentBroker>();
                ConfigurationDocumentSearchCriteria criteria = BuildCurrentVersionCriteria(group, version, user, instanceKey);
                ConfigurationDocument settings = broker.FindOne(criteria);
                settings.Clear();
            }
            catch (EntityNotFoundException)
            {
                // no saved values - nothing to do
            }
        }

        #endregion

        private ConfigurationDocument NewSettingsInstance(string groupName, Version version, string user, string instanceKey)
        {
            // force an empty instanceKey to null
            if (instanceKey != null && instanceKey.Length == 0)
                instanceKey = null;

            return new ConfigurationDocument(groupName, VersionUtils.ToPaddedVersionString(version), user, instanceKey, null);
        }

        private ConfigurationDocumentSearchCriteria BuildCurrentVersionCriteria(string groupName, Version version, string user, string instanceKey)
        {
            ConfigurationDocumentSearchCriteria criteria = BuildUnversionedCriteria(groupName, user, instanceKey);
            criteria.DocumentVersionString.EqualTo(VersionUtils.ToPaddedVersionString(version));
            return criteria;
        }

        private ConfigurationDocumentSearchCriteria BuildCurrentAndPerviousVersionsCriteria(string groupName, Version version, string user, string instanceKey)
        {
            ConfigurationDocumentSearchCriteria criteria = BuildUnversionedCriteria(groupName, user, instanceKey);
            criteria.DocumentVersionString.LessThanOrEqualTo(VersionUtils.ToPaddedVersionString(version));
            return criteria;
        }

        private ConfigurationDocumentSearchCriteria BuildUnversionedCriteria(string groupName, string user, string instanceKey)
        {
            ConfigurationDocumentSearchCriteria criteria = new ConfigurationDocumentSearchCriteria();
            criteria.DocumentName.EqualTo(groupName);

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
