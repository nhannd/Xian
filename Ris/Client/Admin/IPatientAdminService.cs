using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    public interface IPatientAdminService : IHealthcareServiceLayer
    {
        /// <summary>
        /// List all patients matching the specified criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<Patient> ListPatients(PatientSearchCriteria criteria);

        /// <summary>
        /// Loads the specified patient, including collections and related entities
        /// relevant in the context of patient administration.
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        Patient LoadPatientDetails(long oid);

        /// <summary>
        /// Loads the specified patient.
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        Patient LoadPatient(long oid);

        /// <summary>
        /// Add a new patient to the system
        /// </summary>
        /// <param name="patient"></param>
        void AddNewPatient(Patient patient);

        /// <summary>
        /// Updates an existing patient
        /// </summary>
        /// <param name="patient"></param>
        void UpdatePatient(Patient patient);
    }
}
