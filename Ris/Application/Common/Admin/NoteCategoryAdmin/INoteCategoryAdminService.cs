using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin
{
    /// <summary>
    /// Provides operations to administer note categories
    /// </summary>
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
