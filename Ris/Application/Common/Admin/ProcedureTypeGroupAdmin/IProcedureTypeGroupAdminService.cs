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

namespace ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeGroupAdmin
{
    /// <summary>
    /// 
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface IProcedureTypeGroupAdminService
    {
        /// <summary>
		/// Loads details of specified item for editing.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetProcedureTypeGroupEditFormDataResponse GetProcedureTypeGroupEditFormData(
            GetProcedureTypeGroupEditFormDataRequest request);

        /// <summary>
		/// Loads details of specified item for editing.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
		GetProcedureTypeGroupSummaryFormDataResponse GetProcedureTypeGroupSummaryFormData(
			GetProcedureTypeGroupSummaryFormDataRequest request);

        /// <summary>
		/// Summary list of all items.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListProcedureTypeGroupsResponse ListProcedureTypeGroups(
            ListProcedureTypeGroupsRequest request);

        /// <summary>
		/// Loads all form data needed to edit an item.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadProcedureTypeGroupForEditResponse LoadProcedureTypeGroupForEdit(
            LoadProcedureTypeGroupForEditRequest request);

        /// <summary>
		/// Adds a new item.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddProcedureTypeGroupResponse AddProcedureTypeGroup(
            AddProcedureTypeGroupRequest request);

        /// <summary>
		/// Updates an item.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        UpdateProcedureTypeGroupResponse UpdateProcedureTypeGroup(
            UpdateProcedureTypeGroupRequest request);

		/// <summary>
		/// Deletes an item.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[OperationContract]
		[FaultContract(typeof(RequestValidationException))]
		[FaultContract(typeof(ConcurrentModificationException))]
		DeleteProcedureTypeGroupResponse DeleteProcedureTypeGroup(
			DeleteProcedureTypeGroupRequest request);
	}
}