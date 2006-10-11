using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Services
{
    public interface IAdtService : IHealthcareServiceLayer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IList<PatientProfile> ListPatientProfiles(PatientProfileSearchCriteria criteria);

        void LoadPatientProfiles(Patient patient);

        void LoadPatientProfileDetails(PatientProfile profile);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="patientProfile"></param>
        /// <returns></returns>
        IList<PatientProfile> ListReconciledPatientProfiles(PatientProfile patientProfile);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="patientProfile"></param>
        /// <returns></returns>
        IList<PatientProfileMatch> FindPatientReconciliationMatches(PatientProfile patientProfile);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="toBeKept"></param>
        /// <param name="toBeReconciled"></param>
        void ReconcilePatient(Patient toBeKept, IList<Patient> toBeReconciled);

        void CreatePatient(Patient patient);
    }
}
