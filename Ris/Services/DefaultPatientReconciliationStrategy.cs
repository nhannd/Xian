using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Services
{
    [ExtensionOf(typeof(PatientReconciliationStrategyExtensionPoint))]
    public class DefaultPatientReconciliationStrategy : IPatientReconciliationStrategy
    {
        private IPatientProfileBroker _broker;

        #region IPatientReconciliationStrategy Members

        public IList<PatientProfileMatch> FindReconciliationMatches(ClearCanvas.Healthcare.PatientProfile patient)
        {
            IList<PatientProfileMatch> matches = new List<PatientProfileMatch>();

            // 3 out of 4 matches
            PatientProfileSearchCriteria high = new PatientProfileSearchCriteria();
            //IList<PatientProfile> highMatches = _service.GetP

            // 2 out of 4 matches
            PatientProfileSearchCriteria med = new PatientProfileSearchCriteria();

            // 1 out of 4 matches
            PatientProfileSearchCriteria low = new PatientProfileSearchCriteria();

            return matches;
        }

        public void ReconcilePatient(ClearCanvas.Healthcare.Patient patient, ClearCanvas.Healthcare.PatientProfile profileToBeReconciled)
        {
            patient.Profiles.Add(profileToBeReconciled);
            profileToBeReconciled.Patient = patient;
        }

        public IPatientProfileBroker Broker
        {
            get { return _broker; }
            set { _broker = value; }
        }

        #endregion
    }
}
