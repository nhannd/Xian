using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

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
        /// Loads the details collections for the specified patient profile, if not already loaded.
        /// </summary>
        /// <param name="profile"></param>
        Patient LoadPatientAndAllProfiles(EntityRef profileRef);

        /// <summary>
        /// Loads a patient from the specified patient entity reference
        /// </summary>
        /// <param name="patientRef"></param>
        /// <returns></returns>
        Patient LoadPatient(EntityRef patientRef);

        /// <summary>
        /// Loads the <see cref="Patient.Visits"/> collection, if it is not already loaded.
        /// </summary>
        /// <param name="patient">The patient to load profiles for</param>
        IList<Visit> ListPatientVisits(EntityRef patientRef);

        /// <summary>
        /// Loads the Visit for the specified Visit reference
        /// </summary>
        /// <param name="visitRef"></param>
        /// <returns></returns>
        Visit LoadVisit(EntityRef visitRef, bool withDetails);

        /// <summary>
        /// Loads the patient profile with the specified OID, and optionally with details.
        /// </summary>
        /// <param name="profileRef">A reference to the patient profile to load</param>
        /// <param name="withDetails">If true, will also load the related detail collections</param>
        /// <returns></returns>
        PatientProfile LoadPatientProfile(EntityRef profileRef, bool withDetails);

        /// <summary>
        /// Searches for reconciliation candidates for the specified patient profile.
        /// </summary>
        /// <param name="patientProfile"></param>
        /// <returns>A list of matches</returns>
        IList<PatientProfileMatch> FindPatientReconciliationMatches(EntityRef patientProfileRef);

        /// <summary>
        /// Loads specified profiles, with details, and computes the discrepancy according to the specified set
        /// of testable discrepancies.
        /// </summary>
        /// <param name="profileRefs"></param>
        /// <param name="testables"></param>
        /// <returns></returns>
        PatientProfileDiff LoadPatientProfileDiff(EntityRef[] profileRefs, PatientProfileDiscrepancy testables);
 
            
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

        /// <summary>
        /// Saves a new visit entity and associates it with the specified patient
        /// </summary>
        /// <param name="visit"></param>
        /// <param name="patientRef"></param>
        void SaveNewVisit(Visit visit, EntityRef patientRef);

        /// <summary>
        /// Updates the specified visit
        /// </summary>
        /// <param name="visit"></param>
        void UpdateVisit(Visit visit);
    }
}
