using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Services
{
    /// <summary>
    /// Provides ADT (Admit-Discharge-Transfer) services 
    /// </summary>
    public interface IAdtService : IHealthcareServiceLayer
    {
        /// <summary>
        /// List patient profiles matching the specified criteria
        /// </summary>
        /// <param name="criteria">Criteria to match</param>
        /// <returns>A list of patient profiles</returns>
        IList<PatientProfile> ListPatientProfiles(PatientProfileSearchCriteria criteria);

        /// <summary>
        /// Loads the <see cref="Patient.Profiles"/> collection, if it is not already loaded.
        /// </summary>
        /// <param name="patient">The patient to load profiles for</param>
        void LoadPatientProfiles(Patient patient);

        /// <summary>
        /// Loads the details collections for the specified patient profile, if not already loaded.
        /// </summary>
        /// <param name="profile"></param>
        void LoadPatientProfileDetails(PatientProfile profile);

        /// <summary>
        /// Loads the patient profile with the specified OID, and optionally with details.
        /// </summary>
        /// <param name="oid">The OID of the patient profile to load</param>
        /// <param name="withDetails">If true, will also load the related detail collections</param>
        /// <returns></returns>
        PatientProfile LoadPatientProfile(long oid, bool withDetails);

        /// <summary>
        /// Searches for reconciliation candidates for the specified patient profile.
        /// </summary>
        /// <param name="patientProfile"></param>
        /// <returns>A list of matches</returns>
        IList<PatientProfileMatch> FindPatientReconciliationMatches(PatientProfile patientProfile);

        /// <summary>
        /// Reconciles the specified list of patients to the specified patient.
        /// </summary>
        /// <param name="toBeKept">Destination patient</param>
        /// <param name="toBeReconciled">Source patients</param>
        void ReconcilePatients(Patient destPatient, IList<Patient> sourcePatients);

        /// <summary>
        /// Creates a new patient for the specified profile, and returns the <see cref="Patient"/> object
        /// </summary>
        /// <param name="profile"></param>
        Patient CreatePatientForProfile(PatientProfile profile);
 
        /// <summary>
        /// Updates the specified patient profile
        /// </summary>
        /// <param name="profile"></param>
        void UpdatePatientProfile(PatientProfile profile);
    }
}
