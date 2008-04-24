using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Imex;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Configuration.Brokers;

namespace ClearCanvas.Enterprise.Configuration.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("Configuration", ItemsPerFile = 1)]
    public class ConfigurationImex : XmlEntityImex<ConfigurationDocument, ConfigurationImex.ConfigurationData>
    {
        [DataContract]
        public class ConfigurationData
        {
            [DataMember]
            public string Name;

            [DataMember]
            public string Version;

            [DataMember]
            public string User;

            [DataMember]
            public string InstanceKey;
            
            [DataMember]
            public string Body;
        }


        #region Overrides

        protected override IList<ConfigurationDocument> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            ConfigurationDocumentSearchCriteria where = new ConfigurationDocumentSearchCriteria();
            where.DocumentName.SortAsc(0);
            where.DocumentVersionString.SortAsc(1);
            where.User.SortAsc(2);
            where.InstanceKey.SortAsc(3);

            return context.GetBroker<IConfigurationDocumentBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override ConfigurationData Export(ConfigurationDocument configDocument, IReadContext context)
        {
            ConfigurationData data = new ConfigurationData();
            data.Name = configDocument.DocumentName;
            data.Version = configDocument.DocumentVersionString;
            data.User = configDocument.User;
            data.InstanceKey = configDocument.InstanceKey;
            data.Body = configDocument.Body.DocumentText;

            return data;
        }

        protected override void Import(ConfigurationData data, IUpdateContext context)
        {
            ConfigurationDocument document = LoadOrCreateDocument(data.Name, data.Version, data.User, data.InstanceKey, context);
            document.Body.DocumentText = data.Body;
        }

        #endregion


        private ConfigurationDocument LoadOrCreateDocument(string name, string version, string user, string instanceKey, IPersistenceContext context)
        {
            ConfigurationDocument document = null;

            try
            {
                ConfigurationDocumentSearchCriteria criteria = new ConfigurationDocumentSearchCriteria();
                criteria.DocumentName.EqualTo(name);
                criteria.DocumentVersionString.EqualTo(version);

                if (!string.IsNullOrEmpty(instanceKey))
                    criteria.InstanceKey.EqualTo(instanceKey);
                else
                    criteria.InstanceKey.IsNull();

                if (!string.IsNullOrEmpty(user))
                    criteria.User.EqualTo(user);
                else
                    criteria.User.IsNull();

                IConfigurationDocumentBroker broker = context.GetBroker<IConfigurationDocumentBroker>();
                document = broker.FindOne(criteria);
            }
            catch (EntityNotFoundException)
            {
                document = new ConfigurationDocument(
                    name,
                    version,
                    StringUtilities.NullIfEmpty(user),
                    StringUtilities.NullIfEmpty(instanceKey));
                context.Lock(document, DirtyState.New);
            }
            return document;
        }
    }
}

