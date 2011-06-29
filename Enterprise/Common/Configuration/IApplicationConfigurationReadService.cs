#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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
		/// Lists configuration documents matching specified criteria.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		ListConfigurationDocumentsResponse ListConfigurationDocuments(ListConfigurationDocumentsRequest request);


		/// <summary>
		/// Gets the document specified by the name, version, user and instance key.
		/// The user and instance key may be null.
		/// </summary>
		[OperationContract]
		GetConfigurationDocumentResponse GetConfigurationDocument(GetConfigurationDocumentRequest request);
	}
}
