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

using System;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin
{
    /// <summary>
    /// Provides operations to administer note categories
    /// </summary>
    [RisApplicationService]
    [ServiceContract]
    public interface INoteCategoryAdminService
    {
        /// <summary>
        /// Summary list of all note categories
        /// </summary>
        /// <param name="request"><see cref="ListAllNoteCategoriesRequest"/></param>
        /// <returns><see cref="ListAllNoteCategoriesResponse"/></returns>
        [OperationContract]
        ListAllNoteCategoriesResponse ListAllNoteCategories(ListAllNoteCategoriesRequest request);

        /// <summary>
        /// Add a new note category.  A note category with the same name as an existing note category cannnot be added.
        /// </summary>
        /// <param name="request"><see cref="AddNoteCategoryRequest"/></param>
        /// <returns><see cref="AddNoteCategoryResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddNoteCategoryResponse AddNoteCategory(AddNoteCategoryRequest request);

        /// <summary>
        /// Update a new note category.  A note category with the same name as an existing note category cannnot be updated.
        /// </summary>
        /// <param name="request"><see cref="UpdateNoteCategoryRequest"/></param>
        /// <returns><see cref="UpdateNoteCategoryResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateNoteCategoryResponse UpdateNoteCategory(UpdateNoteCategoryRequest request);

		/// <summary>
		/// Delete a note category.
		/// </summary>
		/// <param name="request"><see cref="DeleteNoteCategoryRequest "/></param>
		/// <returns><see cref="DeleteNoteCategoryResponse"/></returns>
		[OperationContract]
		[FaultContract(typeof(ConcurrentModificationException))]
		[FaultContract(typeof(RequestValidationException))]
		DeleteNoteCategoryResponse DeleteNoteCategory(DeleteNoteCategoryRequest request);
		
		/// <summary>
        /// Loads all form data needed to edit a note category
        /// </summary>
        /// <param name="request"><see cref="GetNoteCategoryEditFormDataRequest"/></param>
        /// <returns><see cref="GetNoteCategoryEditFormDataResponse"/></returns>
        [OperationContract]
        GetNoteCategoryEditFormDataResponse GetNoteCategoryEditFormData(GetNoteCategoryEditFormDataRequest request);

        /// <summary>
        /// Load details for a note category
        /// </summary>
        /// <param name="request"><see cref="LoadNoteCategoryForEditRequest"/></param>
        /// <returns><see cref="LoadNoteCategoryForEditResponse"/></returns>
        [OperationContract]
        LoadNoteCategoryForEditResponse LoadNoteCategoryForEdit(LoadNoteCategoryForEditRequest request);
    }
}
