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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin
{
    /// <summary>
    /// Provides operations to administer staff groups.
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface IStaffGroupAdminService
    {
        /// <summary>
        /// Lists staff groups based on a text query.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        TextQueryResponse<StaffGroupSummary> TextQuery(StaffGroupTextQueryRequest request);

        /// <summary>
        /// Summary list of all staff groups.
        /// </summary>
        [OperationContract]
        ListStaffGroupsResponse ListStaffGroups(ListStaffGroupsRequest request);

        /// <summary>
        /// Loads details of specified staff group for editing.
        /// </summary>
        [OperationContract]
        LoadStaffGroupForEditResponse LoadStaffGroupForEdit(LoadStaffGroupForEditRequest request);

        /// <summary>
        /// Loads all form data needed to edit a staff group.
        /// </summary>
        [OperationContract]
        LoadStaffGroupEditorFormDataResponse LoadStaffGroupEditorFormData(LoadStaffGroupEditorFormDataRequest request);

        /// <summary>
        /// Adds a new staff group.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddStaffGroupResponse AddStaffGroup(AddStaffGroupRequest request);

        /// <summary>
        /// Updates a staff group.
        /// </summary>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateStaffGroupResponse UpdateStaffGroup(UpdateStaffGroupRequest request);

		/// <summary>
		/// Deletes a staff group.
		/// </summary>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteStaffGroupResponse DeleteStaffGroup(DeleteStaffGroupRequest request);
	}
}
