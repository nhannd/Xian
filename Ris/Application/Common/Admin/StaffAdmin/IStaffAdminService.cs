using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    /// <summary>
    /// Provides data loading/saving for the <see cref="StaffSummaryComponent"/> and <see cref="StaffEditorComponent"/>
    /// </summary>
    [ServiceContract]
    public interface IStaffAdminService
    {
        /// <summary>
        /// Search for staffs by name
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        FindStaffsResponse FindStaffs(FindStaffsRequest request);

        /// <summary>
        /// List all staffs for the <see cref="StaffSummaryComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListAllStaffsResponse ListAllStaffs(ListAllStaffsRequest request);

        /// <summary>
        /// Update changes to a staff made via the <see cref="StaffEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AddStaffResponse AddStaff(AddStaffRequest request);

        /// <summary>
        /// Update changes to a staff made via the <see cref="StaffEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        UpdateStaffResponse UpdateStaff(UpdateStaffRequest request);

        /// <summary>
        /// Loads all staff data for the <see cref="StaffEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadStaffForEditResponse LoadStaffForEdit(LoadStaffForEditRequest request);

        /// <summary>
        /// Loads all form data for the <see cref="StaffEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadStaffEditorFormDataResponse LoadStaffEditorFormData(LoadStaffEditorFormDataRequest request);
    }
}
