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
        IList<PatientProfile> ListPatients(PatientProfileSearchCriteria criteria);

        /// <summary>
        /// Loads the specified patient, including collections and related entities
        /// relevant in the context of patient administration.
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        PatientProfile LoadPatientDetails(long oid);

        /// <summary>
        /// Loads the specified patient.
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        PatientProfile LoadPatient(long oid);

        /// <summary>
        /// Add a new patient to the system
        /// </summary>
        /// <param name="patient"></param>
        void AddNewPatient(PatientProfile patient);

        /// <summary>
        /// Updates an existing patient
        /// </summary>
        /// <param name="patient"></param>
        void UpdatePatient(PatientProfile patient);
    }
}
