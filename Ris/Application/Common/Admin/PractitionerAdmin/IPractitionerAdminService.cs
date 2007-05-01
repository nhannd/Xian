using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.PractitionerAdmin
{
    /// <summary>
    /// Provides operations to administer practitioners
    /// </summary>
    [ServiceContract]
    public interface IPractitionerAdminService
    {
        /// <summary>
        /// Search for practitioners by name
        /// </summary>
        /// <param name="request"><see cref="FindPractitionersRequest"/></param>
        /// <returns><see cref="FindPractitionersResponse"/></returns>
        [OperationContract]
        FindPractitionersResponse FindPractitioners(FindPractitionersRequest request);

        /// <summary>
        /// Summary list of all practitioners
        /// </summary>
        /// <param name="request"><see cref="ListAllPractitionersRequest"/></param>
        /// <returns><see cref="ListAllPractitionersResponse"/></returns>
        [OperationContract]
        ListAllPractitionersResponse ListAllPractitioners(ListAllPractitionersRequest request);

        /// <summary>
        /// Add a new practitioner.  A practitioner with the same name as an existing staff cannnot be added.
        /// </summary>
        /// <param name="request"><see cref="AddPractitionerRequest"/></param>
        /// <returns><see cref="AddPractitionerResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddPractitionerResponse AddPractitioner(AddPractitionerRequest request);

        /// <summary>
        /// Update a new practitioner.  A practitioner with the same name as an existing staff cannnot be udpdate.
        /// </summary>
        /// <param name="request"><see cref="UpdatePractitionerRequest"/></param>
        /// <returns><see cref="UpdatePractitionerResponse"/></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdatePractitionerResponse UpdatePractitioner(UpdatePractitionerRequest request);

        /// <summary>
        /// Load details for a specified practitioner
        /// </summary>
        /// <param name="request"><see cref="LoadPractitionerForEditRequest"/></param>
        /// <returns><see cref="LoadPractitionerForEditResponse"/></returns>
        [OperationContract]
        LoadPractitionerForEditResponse LoadPractitionerForEdit(LoadPractitionerForEditRequest request);

        /// <summary>
        /// Loads all form data needed to edit a practitioner
        /// </summary>
        /// <param name="request"><see cref="LoadPractitionerEditorFormDataRequest"/></param>
        /// <returns><see cref="LoadPractitionerEditorFormDataResponse"/></returns>
        [OperationContract]
        LoadPractitionerEditorFormDataResponse LoadPractitionerEditorFormData(LoadPractitionerEditorFormDataRequest request);
    }
}
