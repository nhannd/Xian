using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin
{
    /// <summary>
    /// Provides data loading/saving for the <see cref="NoteCategorySummaryComponent"/> and <see cref="NoteCategoryEditorComponent"/>
    /// </summary>
    [ServiceContract]
    public interface INoteCategoryAdminService
    {
        /// <summary>
        /// List all NoteCategories for the <see cref="NoteCategorySummaryComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListAllNoteCategoriesResponse ListAllNoteCategories(ListAllNoteCategoriesRequest request);

        /// <summary>
        /// Add a new NoteCategory created via the <see cref="NoteCategoryEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddNoteCategoryResponse AddNoteCategory(AddNoteCategoryRequest request);

        /// <summary>
        /// Update changes to a NoteCategory made via the <see cref="NoteCategoryEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateNoteCategoryResponse UpdateNoteCategory(UpdateNoteCategoryRequest request);

        /// <summary>
        /// Loads all form data for the <see cref="NoteCategoryEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        GetNoteCategoryEditFormDataResponse GetNoteCategoryEditFormData(GetNoteCategoryEditFormDataRequest request);

        /// <summary>
        /// Loads all NoteCategory data for the <see cref="NoteCategoryEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadNoteCategoryForEditResponse LoadNoteCategoryForEdit(LoadNoteCategoryForEditRequest request);
    }
}
