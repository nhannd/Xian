#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    /// <summary>
    /// Provides operations to administer staffs
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    [ServiceKnownType(typeof(StaffTextQueryRequest))]
    public interface IStaffAdminService
    {
        /// <summary>
        /// Returns a list of staff based on a textual query.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        TextQueryResponse<StaffSummary> TextQuery(StaffTextQueryRequest request);

        /// <summary>
        /// Summary list of all staffs
        /// </summary>
        /// <param name="request"><see cref="ListStaff"/></param>
        /// <returns><see cref="ListStaff"/></returns>
        [OperationContract]
        ListStaffResponse ListStaff(ListStaffRequest request);

        /// <summary>
        /// Add a new staff.  A staff with the same name as an existing staff cannnot be added.
        /// </summary>
        /// <param name="request"><see cref="AddStaffRequest"/></param>
        /// <returns><see cref="AddStaffResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddStaffResponse AddStaff(AddStaffRequest request);

        /// <summary>
        /// Update a new staff.  A staff with the same name as an existing staff cannnot be updated.
        /// </summary>
        /// <param name="request"><see cref="UpdateStaffRequest"/></param>
        /// <returns><see cref="UpdateStaffResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateStaffResponse UpdateStaff(UpdateStaffRequest request);

		/// <summary>
		/// Delete a staff.
		/// </summary>
		/// <param name="request"><see cref="DeleteStaffRequest"/></param>
		/// <returns><see cref="DeleteStaffResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteStaffResponse DeleteStaff(DeleteStaffRequest request);

        /// <summary>
        /// Load details for a specified staff
        /// </summary>
        /// <param name="request"><see cref="LoadStaffForEditRequest"/></param>
        /// <returns><see cref="LoadStaffForEditResponse"/></returns>
        [OperationContract]
        LoadStaffForEditResponse LoadStaffForEdit(LoadStaffForEditRequest request);

        /// <summary>
        /// Loads all form data needed to edit a staff
        /// </summary>
        /// <param name="request"><see cref="LoadStaffEditorFormDataRequest"/></param>
        /// <returns><see cref="LoadStaffEditorFormDataResponse"/></returns>
        [OperationContract]
        LoadStaffEditorFormDataResponse LoadStaffEditorFormData(LoadStaffEditorFormDataRequest request);
    }
}
