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
	/// Defines a service for saving/retrieving configuration data to/from a persistent store.
	/// </summary>
	[EnterpriseCoreService]
	[ServiceContract]
	[Authentication(true)]
	public interface IConfigurationService : IApplicationConfigurationReadService
	{
		/// <summary>
		/// Imports meta-data for a settings group.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		ImportSettingsGroupResponse ImportSettingsGroup(ImportSettingsGroupRequest request);

		/// <summary>
		/// Sets the content for the specified document, version, user and instance key.
		/// The user and instance key may be null.
		/// </summary>
		[OperationContract]
		SetConfigurationDocumentResponse SetConfigurationDocument(SetConfigurationDocumentRequest request);

		/// <summary>
		/// Removes any stored settings values for the specified group, version, user and instance key.
		/// The user and instance key may be null.
		/// </summary>
		[OperationContract]
		RemoveConfigurationDocumentResponse RemoveConfigurationDocument(RemoveConfigurationDocumentRequest request);

	}
}
