using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Enterprise.Configuration.Brokers;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Configuration;

namespace ClearCanvas.Enterprise.Configuration
{
    [ExtensionOf(typeof(CoreServiceExtensionPoint))]
    [ServiceImplementsContract(typeof(IApplicationConfigurationReadService))]
    public class ApplicationConfigurationReadService : ConfigurationServiceBase, IApplicationConfigurationReadService
    {
        #region IApplicationConfigurationReadService Members

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        [ResponseCaching("GetSettingsMetadataCachingDirective")]
        public ListSettingsGroupsResponse ListSettingsGroups(ListSettingsGroupsRequest request)
        {
            IConfigurationSettingsGroupBroker broker = PersistenceContext.GetBroker<IConfigurationSettingsGroupBroker>();
            return new ListSettingsGroupsResponse(
                CollectionUtils.Map<ConfigurationSettingsGroup, SettingsGroupDescriptor>(
                    broker.FindAll(),
                    delegate(ConfigurationSettingsGroup g)
                    {
                        return g.GetDescriptor();
                    }));
        }

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        [ResponseCaching("GetSettingsMetadataCachingDirective")]
        public ListSettingsPropertiesResponse ListSettingsProperties(ListSettingsPropertiesRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.Group, "Group");

            ConfigurationSettingsGroupSearchCriteria where =
                ConfigurationSettingsGroup.GetCriteria(request.Group);

            IConfigurationSettingsGroupBroker broker = PersistenceContext.GetBroker<IConfigurationSettingsGroupBroker>();
            ConfigurationSettingsGroup group = broker.FindOne(where);

            return new ListSettingsPropertiesResponse(
                CollectionUtils.Map<ConfigurationSettingsProperty, SettingsPropertyDescriptor>(
                    group.SettingsProperties,
                    delegate(ConfigurationSettingsProperty p)
                    {
                        return p.GetDescriptor();
                    }));
        }

        // because this service is invoked by the framework, rather than by the application,
        // it is safest to use a new persistence scope
        [ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
        [ResponseCaching("GetDocumentCachingDirective")]
        public GetConfigurationDocumentResponse GetConfigurationDocument(GetConfigurationDocumentRequest request)
        {
            Platform.CheckForNullReference(request, "request");
            Platform.CheckMemberIsSet(request.DocumentKey, "DocumentKey");

            return GetConfigurationDocumentHelper(request.DocumentKey);
        }

        #endregion
    }
}
