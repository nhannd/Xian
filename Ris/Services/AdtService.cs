using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Services
{

    [ExtensionPoint()]
    public class PatientReconciliationStrategyExtensionPoint : ExtensionPoint<IPatientReconciliationStrategy>
    {
    }

    [ExtensionOf(typeof(ClearCanvas.Enterprise.ServiceLayerExtensionPoint))]
    public class AdtService : HealthcareServiceLayer, IAdtService
    {
        private IPatientReconciliationStrategy _strategy;

        public AdtService()
            : this(new PatientReconciliationStrategyExtensionPoint())
        {
        }

        internal AdtService(IExtensionPoint xp)
        {
            _strategy = (IPatientReconciliationStrategy)xp.CreateExtension();
        }

        #region IAdtService Members

        [ReadOperation]
        public IList<PatientProfile> ListPatientProfiles(PatientProfileSearchCriteria criteria)
        {
            return GetPatientProfileBroker().Find(criteria);
        }

        public IList<PatientProfileMatch> FindPatientReconciliationMatches(PatientProfile patientProfile)
        {
            return _strategy.FindReconciliationMatches(patientProfile);
        }

        public IList<PatientProfile> ListReconciledPatientProfiles(PatientProfile patientProfile)
        {
            IList<PatientProfile> reconciledProfiles = new List<PatientProfile>();
            foreach (PatientProfile profile in patientProfile.Patient.Profiles)
            {
                reconciledProfiles.Add(profile);
            }
            return reconciledProfiles;
        }

        [UpdateOperation]
        public void ReconcilePatients(PatientProfile toBeKept, PatientProfile toBeReconciled)
        {
            if( toBeKept == null )
            {
                throw new PatientReconciliationException("Patient to be kept is null");
            }
            if (toBeKept == toBeReconciled)
            {
                throw new PatientReconciliationException("Patients are the same");
            }
            ReconcilePatients(toBeKept.Patient, toBeReconciled);
        }

        [UpdateOperation]
        public void ReconcilePatients(Patient patient, PatientProfile toBeReconciled)
        {
            PatientIdentifier mrnToBeReconciled = toBeReconciled.MRN;
            if( mrnToBeReconciled != null &&
                PatientHasProfileForSite(patient, mrnToBeReconciled.AssigningAuthority) == true )
            {
                throw new PatientReconciliationException("Patient already has identifier for site " + mrnToBeReconciled.AssigningAuthority);
            }

            // perform some additional validation on the profile?

            DoReconciliation(patient, toBeReconciled);
        }

        #endregion

        private void DoReconciliation(Patient patient, PatientProfile toBeReconciled)
        {
            _strategy.ReconcilePatient(patient, toBeReconciled);
        }

        private static bool PatientHasProfileForSite(Patient patient, string site)
        {
            foreach (PatientProfile profile in patient.Profiles)
            {
                PatientIdentifier id = profile.MRN;
                if (id != null && 
                    id.AssigningAuthority == site)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
