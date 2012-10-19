#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.Admin.ProtocolAdmin
{
    /// <summary>
    /// Provides operations to administer protocol codes and protocol groups
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface IProtocolAdminService
    {
		/// <summary>
		/// Lists all protocol codes.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
    	ListProtocolCodesResponse ListProtocolCodes(ListProtocolCodesRequest request);

		/// <summary>
		/// Loads protocol code for editing.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		LoadProtocolCodeForEditResponse LoadProtocolCodeForEdit(LoadProtocolCodeForEditRequest request);

		/// <summary>
        /// Adds a new protocol code with specified name and description (optional)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddProtocolCodeResponse AddProtocolCode(AddProtocolCodeRequest request);

        /// <summary>
        /// Updates name and/or description of specified protocol code
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		UpdateProtocolCodeResponse UpdateProtocolCode(UpdateProtocolCodeRequest request);

        /// <summary>
        /// Marks a protocol code as deleted
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		DeleteProtocolCodeResponse DeleteProtocolCode(DeleteProtocolCodeRequest request);

        /// <summary>
        /// Summary list of all protocol groups
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListProtocolGroupsResponse ListProtocolGroups(ListProtocolGroupsRequest request);

        /// <summary>
        /// Loads details for specified protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadProtocolGroupForEditResponse LoadProtocolGroupForEdit(LoadProtocolGroupForEditRequest request);

        /// <summary>
        /// Provides a list of available protocol codes and reading groups that can be assigned while adding/updating a protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetProtocolGroupEditFormDataResponse GetProtocolGroupEditFormData(GetProtocolGroupEditFormDataRequest request);

        /// <summary>
        /// Adds a new protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		AddProtocolGroupResponse AddProtocolGroup(AddProtocolGroupRequest request);

        /// <summary>
        /// Updates an existing protocol group
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		UpdateProtocolGroupResponse UpdateProtocolGroup(UpdateProtocolGroupRequest request);

        /// <summary>
        /// Deletes an existing protocol
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		DeleteProtocolGroupResponse DeleteProtocolGroup(DeleteProtocolGroupRequest request);
    }
}
