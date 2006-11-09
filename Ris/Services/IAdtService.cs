using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;

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

        Patient LoadPatientAndAllProfiles(EntityRef<PatientProfile> profileRef);

        /// <summary>
        /// Loads the patient profile with the specified OID, and optionally with details.
        /// </summary>
        /// <param name="profileRef">A reference to the patient profile to load</param>
        /// <param name="withDetails">If true, will also load the related detail collections</param>
        /// <returns></returns>
        PatientProfile LoadPatientProfile(EntityRef<PatientProfile> profileRef, bool withDetails);

        /// <summary>
        /// Searches for reconciliation candidates for the specified patient profile.
        /// </summary>
        /// <param name="patientProfile"></param>
        /// <returns>A list of matches</returns>
        IList<PatientProfileMatch> FindPatientReconciliationMatches(EntityRef<PatientProfile> patientProfileRef);

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
