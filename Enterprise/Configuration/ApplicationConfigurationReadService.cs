#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common.Configuration;
using ClearCanvas.Enterprise.Configuration.Brokers;
using ClearCanvas.Common.Utilities;

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
			var broker = PersistenceContext.GetBroker<IConfigurationSettingsGroupBroker>();
			return new ListSettingsGroupsResponse(
				CollectionUtils.Map(broker.FindAll(), (ConfigurationSettingsGroup g) => g.GetDescriptor()));
		}

		// because this service is invoked by the framework, rather than by the application,
		// it is safest to use a new persistence scope
		[ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
		[ResponseCaching("GetSettingsMetadataCachingDirective")]
		public ListSettingsPropertiesResponse ListSettingsProperties(ListSettingsPropertiesRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckMemberIsSet(request.Group, "Group");

			var where = ConfigurationSettingsGroup.GetCriteria(request.Group);

			var broker = PersistenceContext.GetBroker<IConfigurationSettingsGroupBroker>();
			var group = broker.FindOne(where);

			return new ListSettingsPropertiesResponse(
				CollectionUtils.Map(group.SettingsProperties, (ConfigurationSettingsProperty p) => p.GetDescriptor()));
		}

		// because this service is invoked by the framework, rather than by the application,
		// it is safest to use a new persistence scope
		[ReadOperation(PersistenceScopeOption = PersistenceScopeOption.RequiresNew)]
		public ListConfigurationDocumentsResponse ListConfigurationDocuments(ListConfigurationDocumentsRequest request)
		{
			Platform.CheckForNullReference(request, "request");
			Platform.CheckForNullReference(request.Query, "Query");

			return ListConfigurationDocumentsHelper(request.Query);
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
