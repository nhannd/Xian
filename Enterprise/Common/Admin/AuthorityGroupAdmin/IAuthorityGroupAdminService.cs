#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ServiceModel;

namespace ClearCanvas.Enterprise.Common.Admin.AuthorityGroupAdmin
{
	/// <summary>
	/// Provides operations to administer authority groups.
	/// </summary>
	[EnterpriseCoreService]
	[ServiceContract]
    public interface IAuthorityGroupAdminService
	{		
		/// <summary>
		/// Add a new authority group
		/// </summary>
		/// <param name="request"><see cref="AddAuthorityGroupRequest"/></param>
		/// <returns><see cref="AddAuthorityGroupResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddAuthorityGroupResponse AddAuthorityGroup(AddAuthorityGroupRequest request);

		/// <summary>
		/// Updates an authority group
		/// </summary>
		/// <param name="request"><see cref="UpdateAuthorityGroupRequest"/></param>
		/// <returns><see cref="UpdateAuthorityGroupResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(UserAccessDeniedException))]
        UpdateAuthorityGroupResponse UpdateAuthorityGroup(UpdateAuthorityGroupRequest request);

		/// <summary>
		/// Deletes an authority group
		/// </summary>
		/// <param name="request"><see cref="DeleteAuthorityGroupRequest"/></param>
		/// <returns><see cref="DeleteAuthorityGroupResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(AuthorityGroupIsNotEmptyException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteAuthorityGroupResponse DeleteAuthorityGroup(DeleteAuthorityGroupRequest request);

		/// <summary>
		/// Load details for a specified authority group
		/// </summary>
		/// <param name="request"><see cref="LoadAuthorityGroupForEditRequest"/></param>
		/// <returns><see cref="LoadAuthorityGroupForEditResponse"/></returns>
		[OperationContract]
		LoadAuthorityGroupForEditResponse LoadAuthorityGroupForEdit(LoadAuthorityGroupForEditRequest request);

		/// <summary>
		/// Provides a list of all available authority tokens
		/// </summary>
		/// <param name="request"><see cref="ListAuthorityTokensRequest"/></param>
		/// <returns><see cref="ListAuthorityTokensResponse"/></returns>
		[OperationContract]
		ListAuthorityTokensResponse ListAuthorityTokens(ListAuthorityTokensRequest request);

		/// <summary>
		/// Imports authority tokens from a remote source.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		ImportAuthorityTokensResponse ImportAuthorityTokens(ImportAuthorityTokensRequest request);

		/// <summary>
		/// Imports authority groups from a remote source.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		ImportAuthorityGroupsResponse ImportAuthorityGroups(ImportAuthorityGroupsRequest request);

        /// <summary>
        /// Summary list of all authority groups
        /// </summary>
        /// <param name="request"><see cref="ListAuthorityGroupsRequest"/></param>
        /// <returns><see cref="ListAuthorityGroupsResponse"/></returns>
        [OperationContract]
        ListAuthorityGroupsResponse ListAuthorityGroups(ListAuthorityGroupsRequest request);
	}
}