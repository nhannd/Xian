using System;
using System.ServiceModel;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PatientAdmin
{
    /// <summary>
    /// Provides data loading/saving for the <see cref="PatientProfileEditorComponent"/>
    /// </summary>
    [ServiceContract]
    public interface IPatientAdminService
    {
        /// <summary>
        /// Loads all form data for the <see cref="PatientProfileEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadPatientProfileEditorFormDataResponse LoadPatientProfileEditorFormData(LoadPatientProfileEditorFormDataRequest request);

        /// <summary>
        /// Loads all patient profile data for the <see cref="PatientProfileEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        LoadPatientProfileForAdminEditResponse LoadPatientProfileForAdminEdit(LoadPatientProfileForAdminEditRequest request);

        /// <summary>
        /// Saves changes to a patient profile made via the <see cref="PatientProfileEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        SaveAdminEditsForPatientProfileResponse SaveAdminEditsForPatientProfile(SaveAdminEditsForPatientProfileRequest request);

        /// <summary>
        /// Add a new patient profile via the <see cref="PatientProfileEditorComponent"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [OperationContract]
        AdminAddPatientProfileResponse AdminAddPatientProfile(AdminAddPatientProfileRequest request);

        [OperationContract]
        SaveNewNoteForPatientResponse SaveNewNoteForPatient(SaveNewNoteForPatientRequest request);
    }
}
