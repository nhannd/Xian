using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Services
{
    [ExtensionOf(typeof(PatientReconciliationStrategyExtensionPoint))]
    public class DefaultPatientReconciliationStrategy : IPatientReconciliationStrategy
    {
        #region IPatientReconciliationStrategy Members

        public IList<PatientProfileMatch> FindReconciliationMatches(ClearCanvas.Healthcare.PatientProfile patient)
        {
            IList<PatientProfileMatch> matches = new List<PatientProfileMatch>();
            return matches;
        }

        public void ReconcilePatient(ClearCanvas.Healthcare.Patient patient, ClearCanvas.Healthcare.PatientProfile profileToBeReconciled)
        {
            patient.Profiles.Add(profileToBeReconciled);
            profileToBeReconciled.Patient = patient;
        }

        #endregion
    }
}
