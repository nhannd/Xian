using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Configuration.Brokers;
using ClearCanvas.Enterprise.Core;
using System.Threading;
using System.Configuration;
using ClearCanvas.Common.Configuration;
using System.Reflection;
using System.Security.Permissions;

namespace ClearCanvas.Enterprise.Configuration
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IConfigurationService))]
    public class ConfigurationService : CoreServiceLayer, IConfigurationService
    {
        #region IConfigurationService Members

        // this method is only available for administration
        [PrincipalPermission(SecurityAction.Demand, Role=AuthorityTokens.ConfigurationAdmin)]
        public List<SettingsGroupInfo> ListSettingsGroups()
        {
            return CollectionUtils.Map<SettingsGroupDescriptor, SettingsGroupInfo, List<SettingsGroupInfo>>(
                SettingsGroupDescriptor.ListInstalledSettingsGroups(),
                delegate(SettingsGroupDescriptor desc)
                {
                    return new SettingsGroupInfo(desc);
                });
        }

        // this method is only available for administration
        [PrincipalPermission(SecurityAction.Demand, Role = AuthorityTokens.ConfigurationAdmin)]
        public List<SettingsPropertyInfo> ListSettingsProperties(SettingsGroupInfo group)
        {
            return CollectionUtils.Map<SettingsPropertyDescriptor, SettingsPropertyInfo, List<SettingsPropertyInfo>>(
                SettingsPropertyDescriptor.ListSettingsProperties(group.ToDescriptor()),
                delegate(SettingsPropertyDescriptor desc)
                {
                    return new SettingsPropertyInfo(desc);
                });
        }

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public Dictionary<string, string> LoadSettingsValues(string name, Version version, string user, string instanceKey)
        {
            CheckReadAccess(user);

            try
            {
                IConfigurationDocumentBroker broker = PersistenceContext.GetBroker<IConfigurationDocumentBroker>();
                ConfigurationDocumentSearchCriteria criteria = BuildCurrentVersionCriteria(name, version, user, instanceKey);
                ConfigurationDocument document = broker.FindOne(criteria);

                SettingsParser parser = new SettingsParser();
                Dictionary<string, string> values = new Dictionary<string,string>();
                parser.FromXml(document.DocumentText, values);

                return values;
            }
            catch (EntityNotFoundException)
            {
                // no stored values
                return new Dictionary<string, string>();
            }
        }



        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [UpdateOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public void SaveSettingsValues(string name, Version version, string user, string instanceKey, Dictionary<string, string> values)
        {
            CheckWriteAccess(user);

            ConfigurationDocument document = null;
            try
            {
                IConfigurationDocumentBroker broker = PersistenceContext.GetBroker<IConfigurationDocumentBroker>();
                ConfigurationDocumentSearchCriteria criteria = BuildCurrentVersionCriteria(name, version, user, instanceKey);
                document = broker.FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
                // no saved settings, create new
                document = NewDocument(name, version, user, instanceKey);

                PersistenceContext.Lock(document, DirtyState.New);
            }

            // save the text
            SettingsParser parser = new SettingsParser();
            document.DocumentText = parser.ToXml(values);
        }
/*
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
                    currentVersion = NewDocument(group, version, user, instanceKey);
                    CurrentContext.Lock(currentVersion, DirtyState.New);
                }

                SettingsParser parser = new SettingsParser();

                // upgrade the current version from the previous
                parser.UpgradeFromPrevious(currentVersion, previousVersion);

                // place the latest values into the dictionary
                parser.FromXml(currentVersion.DocumentText, values);
            }
        }
*/
        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [UpdateOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        public void RemoveSettingsValues(string name, Version version, string user, string instanceKey)
        {
            CheckWriteAccess(user);
   
            try
            {
                IConfigurationDocumentBroker broker = PersistenceContext.GetBroker<IConfigurationDocumentBroker>();
                ConfigurationDocumentSearchCriteria criteria = BuildCurrentVersionCriteria(name, version, user, instanceKey);
                ConfigurationDocument document = broker.FindOne(criteria);
                broker.Delete(document);
            }
            catch (EntityNotFoundException)
            {
                // no document - nothing to remove
            }
        }

        #endregion

        private void CheckReadAccess(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                // all users can read application configuration docs
            }
            else
            {
                // user can only read their own configuration docs
                if (user != Thread.CurrentPrincipal.Identity.Name)
                    throw new System.Security.SecurityException(SR.ExceptionUserNotAuthorized);
            }
        }

        private void CheckWriteAccess(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                // this is an application configuration doc - need admin permission
                if (!Thread.CurrentPrincipal.IsInRole(ClearCanvas.Enterprise.Common.AuthorityTokens.ConfigurationAdmin))
                    throw new System.Security.SecurityException(SR.ExceptionUserNotAuthorized);
            }
            else
            {
                // user can only save their own configuration docs
                if (user != Thread.CurrentPrincipal.Identity.Name)
                    throw new System.Security.SecurityException(SR.ExceptionUserNotAuthorized);
            }
        }

        private ConfigurationDocument NewDocument(string name, Version version, string user, string instanceKey)
        {
            // force an empty instanceKey to null
            if (instanceKey != null && instanceKey.Length == 0)
                instanceKey = null;

            return new ConfigurationDocument(name, VersionUtils.ToPaddedVersionString(version), user, instanceKey, null);
        }

        private ConfigurationDocumentSearchCriteria BuildCurrentVersionCriteria(string name, Version version, string user, string instanceKey)
        {
            ConfigurationDocumentSearchCriteria criteria = BuildUnversionedCriteria(name, user, instanceKey);
            criteria.DocumentVersionString.EqualTo(VersionUtils.ToPaddedVersionString(version));
            return criteria;
        }

        private ConfigurationDocumentSearchCriteria BuildCurrentAndPerviousVersionsCriteria(string name, Version version, string user, string instanceKey)
        {
            ConfigurationDocumentSearchCriteria criteria = BuildUnversionedCriteria(name, user, instanceKey);
            criteria.DocumentVersionString.LessThanOrEqualTo(VersionUtils.ToPaddedVersionString(version));
            return criteria;
        }

        private ConfigurationDocumentSearchCriteria BuildUnversionedCriteria(string name, string user, string instanceKey)
        {
            ConfigurationDocumentSearchCriteria criteria = new ConfigurationDocumentSearchCriteria();
            criteria.DocumentName.EqualTo(name);

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
