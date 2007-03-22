using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.PractitionerAdmin
{
    public interface IPractitionerAdminService
    {
        /// <summary>
        /// Search for a practitioner by name
        /// </summary>
        /// <param name="surname">The practitioner surname to search for.  May not be null</param>
        /// <param name="givenName">The practitioner givenname to search for.  May be null</param>
        /// <returns>A list of matching practitioners</returns>
        [OperationContract]
        FindPractitionersResponse FindPractitioners(FindPractitionersRequest request);

        /// <summary>
        /// Return all practitioner
        /// </summary>
        /// <returns>A list of all practitioners</returns>
        [OperationContract]
        ListAllPractitionersResponse ListAllPractitioners(ListAllPractitionersRequest request);

        /// <summary>
        /// Add a practitioner
        /// </summary>
        /// <param name="practitioner"></param>
        [OperationContract]
        AddPractitionerResponse AddPractitioner(AddPractitionerRequest request);

        /// <summary>
        /// Update a practitioner
        /// </summary>
        /// <param name="practitioner"></param>
        /// <returns></returns>
        [OperationContract]
        UpdatePractitionerResponse UpdatePractitioner(UpdatePractitionerRequest request);
        
        /// <summary>
        /// Load a practitioner from an entity ref
        /// </summary>
        /// <param name="practitionerRef"></param>
        /// <param name="withDetails">If true, will also load the related detail collections</param>
        /// <returns></returns>
        [OperationContract]
        LoadPractitionerForEditResponse LoadPractitionerForEdit(LoadPractitionerForEditRequest request);

        /// <summary>
        /// Loads all form data for the <see cref=StaffEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadPractitionerEditorFormDataResponse LoadPractitionerEditorFormData(LoadPractitionerEditorFormDataRequest request);
    }
}
