using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.PatientAdmin
{
    public interface IPatientAdminService
    {
        /// <summary>
        /// List all patients matching the specified criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        //IList<PatientProfile> ListPatientProfiles(PatientProfileSearchCriteria criteria);
        ListPatientProfilesResponse ListPatientProfiles(ListPatientProfilesRequest request);

        /// <summary>
        /// Loads the specified patient, including collections and related entities
        /// relevant in the context of patient administration.
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        //PatientProfile LoadPatientProfileDetails(EntityRef profileRef);
        LoadPatientProfileDetailsResponse LoadPatientProfileDetails(LoadPatientProfileDetailsRequest request);

        /// <summary>
        /// Loads the specified patient.
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        //PatientProfile LoadPatientProfile(EntityRef profileRef);
        LoadPatientProfileResponse LoadPatientProfile(LoadPatientProfileRequest request);

        /// <summary>
        /// Add a new patient to the system
        /// </summary>
        /// <param name="patient"></param>
        //void AddNewPatient(PatientProfile patient);
        AddNewPatientResponse AddNewPatient(AddNewPatientRequest request);

        /// <summary>
        /// Updates an existing patient
        /// </summary>
        /// <param name="patient"></param>
        //void UpdatePatientProfile(PatientProfile patient);
        UpdatePatientProfileResponse UpdatePatientProfile(UpdatePatientProfileRequest request);
    }
}
