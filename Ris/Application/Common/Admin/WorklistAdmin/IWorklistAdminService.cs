using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    /// <summary>
    /// Provides services for administration of persistent worklist definitions
    /// </summary>
    [ServiceContract]
    public interface IWorklistAdminService
    {
        /// <summary>
        /// Returns data suitable for populating a form for the purpose of editing a worklist definition
        /// </summary>
        /// <param name="request"><see cref="GetWorklistEditFormDataRequest"/></param>
        /// <returns><see cref="GetWorklistEditFormDataResponse"/></returns>
        [OperationContract]
        GetWorklistEditFormDataResponse GetWorklistEditFormData(GetWorklistEditFormDataRequest request);

        /// <summary>
        /// Returns a list of RequestedProcedureTypeGroups appropriate for a specific worklist type
        /// </summary>
        /// <param name="request"><see cref="ListRequestedProcedureTypeGroupsForWorklistCategoryRequest"/></param>
        /// <returns><see cref="ListRequestedProcedureTypeGroupsForWorklistCategoryResponse"/></returns>
        [OperationContract]
        ListRequestedProcedureTypeGroupsForWorklistCategoryResponse ListRequestedProcedureTypeGroupsForWorklistCategory(ListRequestedProcedureTypeGroupsForWorklistCategoryRequest request);

        /// <summary>
        /// Returns a list of all persistent worklists
        /// </summary>
        /// <param name="request"><see cref="ListWorklistsRequest"/></param>
        /// <returns><see cref="ListWorklistsResponse"/></returns>
        [OperationContract]
        ListWorklistsResponse ListWorklists(ListWorklistsRequest request);

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
    }
}
