using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.PractitionerAdmin
{
    /// <summary>
    /// Provides data loading/saving for the <see cref="StaffSummaryComponent"/> and <see cref="StaffEditorComponent"/>
    /// </summary>
    [ServiceContract]
    public interface IPractitionerAdminService
    {
        /// <summary>
        /// Search for practitioners by name
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        FindPractitionersResponse FindPractitioners(FindPractitionersRequest request);

        /// <summary>
        /// List all practitioners for the <see cref="StaffSummaryComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        ListAllPractitionersResponse ListAllPractitioners(ListAllPractitionersRequest request);

        /// <summary>
        /// Add a new practitioner created via the <see cref="StaffEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddPractitionerResponse AddPractitioner(AddPractitionerRequest request);

        /// <summary>
        /// Update changes to a practitioner made via the <see cref="StaffEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(ConcurrentModificationException))]
        [FaultContract(typeof(RequestValidationException))]
        UpdatePractitionerResponse UpdatePractitioner(UpdatePractitionerRequest request);

        /// <summary>
        /// Loads all practitioner data for the <see cref="StaffEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadPractitionerForEditResponse LoadPractitionerForEdit(LoadPractitionerForEditRequest request);

        /// <summary>
        /// Loads all form data for the <see cref="StaffEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadPractitionerEditorFormDataResponse LoadPractitionerEditorFormData(LoadPractitionerEditorFormDataRequest request);
    }
}
