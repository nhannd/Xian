using System;
using System.Collections.Generic;
using System.Text;

using System.Configuration;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Implementation of <see cref="IConfigurationStore"/>, extends <see cref="ConfigurationStoreExtensionPoint"/>.
    /// Acts as a client-side proxy to <see cref="IConfigurationService"/>
    /// </summary>
    [ExtensionOf(typeof(ConfigurationStoreExtensionPoint))]
    public class EnterpriseConfigurationStore : IConfigurationStore
    {
        public EnterpriseConfigurationStore()
        {
        }

        #region IConfigurationStore Members

        public string LoadDocument(string name, Version version, string user, string instanceKey)
        {
            string document = null;
            Platform.GetService<IConfigurationService>(
                delegate(IConfigurationService service)
                {
                    document = service.LoadDocument(name, version, user, instanceKey);
                });

            return document;
        }

        public void SaveDocument(string name, Version version, string user, string instanceKey, string documentText)
        {
            Platform.GetService<IConfigurationService>(
                delegate(IConfigurationService service)
                {
                    service.SaveDocument(name, version, user, instanceKey, documentText);
                });
        }

        public void LoadSettingsValues(Type settingsClass, string user, string instanceKey, IDictionary<string, string> values)
        {
            try
            {
                string xml = LoadDocument(
                    SettingsClassMetaDataReader.GetGroupName(settingsClass),
                    SettingsClassMetaDataReader.GetVersion(settingsClass),
                    user,
                    instanceKey);

                SettingsParser parser = new SettingsParser();
                parser.FromXml(xml, values);
            }
            catch (ConfigurationDocumentNotFoundException)
            {
                // no saved settings
            }
        }

        public void SaveSettingsValues(Type settingsClass, string user, string instanceKey, IDictionary<string, string> values)
        {
            SettingsParser parser = new SettingsParser();
            string xml = parser.ToXml(values);

            SaveDocument(
                SettingsClassMetaDataReader.GetGroupName(settingsClass),
                SettingsClassMetaDataReader.GetVersion(settingsClass),
                user,
                instanceKey,
                xml);
        }

        public void RemoveUserSettings(Type settingsClass, string user, string instanceKey)
        {
            Platform.GetService<IConfigurationService>(
                delegate(IConfigurationService service)
                {
                    service.RemoveDocument(
                        SettingsClassMetaDataReader.GetGroupName(settingsClass),
                        SettingsClassMetaDataReader.GetVersion(settingsClass),
                        user,
                        instanceKey);
                });
        }

        public void UpgradeUserSettings(Type settingsClass, string user, string instanceKey)
        {
            // TODO implement this later
            throw new NotImplementedException();
        }

        #endregion
    }
}
