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
