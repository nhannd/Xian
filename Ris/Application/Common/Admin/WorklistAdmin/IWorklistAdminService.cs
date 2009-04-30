#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    /// <summary>
    /// Provides services for administration of persistent worklist definitions
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface IWorklistAdminService
    {
        /// <summary>
        /// Returns a list of worklist categories.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListWorklistCategoriesResponse ListWorklistCategories(ListWorklistCategoriesRequest request);

        /// <summary>
        /// Returns a list of worklists matching the specified criteria.
        /// </summary>
        /// <param name="request"><see cref="ListWorklistsRequest"/></param>
        /// <returns><see cref="ListWorklistsResponse"/></returns>
        [OperationContract]
        ListWorklistClassesResponse ListWorklistClasses(ListWorklistClassesRequest request);
        
        /// <summary>
        /// Returns a list of worklists matching the specified criteria.
        /// </summary>
        /// <param name="request"><see cref="ListWorklistsRequest"/></param>
        /// <returns><see cref="ListWorklistsResponse"/></returns>
        [OperationContract]
        ListWorklistsResponse ListWorklists(ListWorklistsRequest request);

        /// <summary>
        /// Returns a list of ProcedureTypeGroups of a specified class.
        /// </summary>
        /// <param name="request"><see cref="ListProcedureTypeGroupsRequest"/></param>
        /// <returns><see cref="ListProcedureTypeGroupsResponse"/></returns>
        [OperationContract]
        ListProcedureTypeGroupsResponse ListProcedureTypeGroups(ListProcedureTypeGroupsRequest request);

        /// <summary>
        /// Returns data suitable for populating a form for the purpose of editing a worklist definition
        /// </summary>
        /// <param name="request"><see cref="GetWorklistEditFormDataRequest"/></param>
        /// <returns><see cref="GetWorklistEditFormDataResponse"/></returns>
        [OperationContract]
        GetWorklistEditFormDataResponse GetWorklistEditFormData(GetWorklistEditFormDataRequest request);

        /// <summary>
        /// Loads a worklist definition for editing
        /// </summary>
        /// <param name="request"><see cref="LoadWorklistForEditRequest"/></param>
        /// <returns><see cref="LoadWorklistForEditResponse"/></returns>
        [OperationContract]
        LoadWorklistForEditResponse LoadWorklistForEdit(LoadWorklistForEditRequest request);

        /// <summary>
        /// Adds a new worklist
        /// </summary>
        /// <param name="request"><see cref="AddWorklistRequest"/></param>
        /// <returns><see cref="AddWorklistResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddWorklistResponse AddWorklist(AddWorklistRequest request);

        /// <summary>
        /// Updates an existing worklist
        /// </summary>
        /// <param name="request"><see cref="UpdateWorklistRequest"/></param>
        /// <returns><see cref="UpdateWorklistResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        UpdateWorklistResponse UpdateWorklist(UpdateWorklistRequest request);

        /// <summary>
        /// Deletes an existing worklist.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ConcurrentModificationException))]
        DeleteWorklistResponse DeleteWorklist(DeleteWorklistRequest request);
    }
}
