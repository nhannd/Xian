using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    [ServiceContract]
    public interface IStaffAdminService
    {
        /// <summary>
        /// Search for a staff by name
        /// </summary>
        /// <param name="surname">The staff surname to search for.  May not be null</param>
        /// <param name="givenName">The staff givenname to search for.  May be null</param>
        /// <returns>A list of matching staffs</returns>
        [OperationContract]
        FindStaffsResponse FindStaffs(FindStaffsRequest request);

        /// <summary>
        /// Return all staff
        /// </summary>
        /// <returns>A list of all staffs</returns>
        [OperationContract]
        ListAllStaffsResponse ListAllStaffs(ListAllStaffsRequest request);

        /// <summary>
        /// Add a staff
        /// </summary>
        /// <param name="staff"></param>
        [OperationContract]
        AddStaffResponse AddStaff(AddStaffRequest request);

        /// <summary>
        /// Update a staff
        /// </summary>
        /// <param name="staff"></param>
        /// <returns></returns>
        [OperationContract]
        UpdateStaffResponse UpdateStaff(UpdateStaffRequest request);

        /// <summary>
        /// Load a staff from an entity ref, and optionally with details
        /// </summary>
        /// <param name="staffRef">A reference to the staff to load</param>
        /// <param name="withDetails">If true, will also load the related detail collections</param>
        /// <returns></returns>
        [OperationContract]
        LoadStaffForEditResponse LoadStaffForEdit(LoadStaffForEditRequest request);
    }
}
