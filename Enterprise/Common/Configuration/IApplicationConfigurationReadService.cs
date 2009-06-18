using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common.Configuration
{
    /// <summary>
    /// Defines a service for allowing un-authenticated processes to read application settings.
    /// </summary>
    [EnterpriseCoreService]
    [ServiceContract]
    [Authentication(false)]
    public interface IApplicationConfigurationReadService
    {
        /// <summary>
        /// Lists settings groups installed in the local plugin base.
        /// </summary>
        [OperationContract]
        ListSettingsGroupsResponse ListSettingsGroups(ListSettingsGroupsRequest request);

        /// <summary>
        /// Lists the settings properties for the specified settings group.
        /// </summary>
        [OperationContract]
        ListSettingsPropertiesResponse ListSettingsProperties(ListSettingsPropertiesRequest request);

        /// <summary>
        /// Gets the document specified by the name, version, user and instance key.
        /// The user and instance key may be null.
        /// </summary>
        [OperationContract]
        GetConfigurationDocumentResponse GetConfigurationDocument(GetConfigurationDocumentRequest request);
    }
}
