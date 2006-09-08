using System;
using System.Collections.Generic;

using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Services
{
    public interface IPatientReconciliationStrategy
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="patient"></param>
        /// <returns></returns>
        IList<PatientProfileMatch> FindReconciliationMatches(PatientProfile patient, IPatientProfileBroker broker);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="patient"></param>
        /// <param name="profileToBeReconciled"></param>
        void ReconcilePatient(Patient patient, PatientProfile profileToBeReconciled, IPatientBroker patientBroker, IPatientProfileBroker patientProfileBroker);
    }
}
