using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin
{
    /// <summary>
    /// Provides operations to administer staffs
    /// </summary>
    [ServiceContract]
    public interface IExternalPractitionerAdminService
    {
        /// <summary>
        /// Summary list of all staffs
        /// </summary>
        /// <param name="request"><see cref="ListAllStaffsRequest"/></param>
        /// <returns><see cref="ListAllStaffsResponse"/></returns>
        [OperationContract]
        ListExternalPractitionersResponse ListExternalPractitioners(ListExternalPractitionersRequest request);

        /// <summary>
        /// Add a new staff.  A staff with the same name as an existing staff cannnot be added.
        /// </summary>
        /// <param name="request"><see cref="AddStaffRequest"/></param>
        /// <returns><see cref="AddStaffResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddExternalPractitionerResponse AddExternalPractitioner(AddExternalPractitionerRequest request);

        /// <summary>
        /// Update a new staff.  A staff with the same name as an existing staff cannnot be updated.
        /// </summary>
        /// <param name="request"><see cref="UpdateStaffRequest"/></param>
        /// <returns><see cref="UpdateStaffResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdateExternalPractitionerResponse UpdateExternalPractitioner(UpdateExternalPractitionerRequest request);

        /// <summary>
        /// Load details for a specified staff
        /// </summary>
        /// <param name="request"><see cref="LoadStaffForEditRequest"/></param>
        /// <returns><see cref="LoadStaffForEditResponse"/></returns>
        [OperationContract]
        LoadExternalPractitionerForEditResponse LoadExternalPractitionerForEdit(LoadExternalPractitionerForEditRequest request);

        /// <summary>
        /// Loads all form data needed to edit a staff
        /// </summary>
        /// <param name="request"><see cref="LoadStaffEditorFormDataRequest"/></param>
        /// <returns><see cref="LoadStaffEditorFormDataResponse"/></returns>
        [OperationContract]
        LoadExternalPractitionerEditorFormDataResponse LoadExternalPractitionerEditorFormData(LoadExternalPractitionerEditorFormDataRequest request);
    }
}
