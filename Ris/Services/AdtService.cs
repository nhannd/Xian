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
        private IExtensionPoint _strategyExtensionPoint;

        public AdtService()
            :this(new PatientReconciliationStrategyExtensionPoint())
        {
        }

        internal AdtService(IExtensionPoint strategyExtensionPoint)
        {
            _strategyExtensionPoint = strategyExtensionPoint;
        }

        #region IAdtService Members

        [ReadOperation]
        public IList<PatientProfile> ListPatientProfiles(PatientProfileSearchCriteria criteria)
        {
            return GetPatientProfileBroker().Find(criteria);
        }

        [ReadOperation]
        public IList<PatientProfileMatch> FindPatientReconciliationMatches(PatientProfile patientProfile)
        {
            IPatientReconciliationStrategy strategy = (IPatientReconciliationStrategy)_strategyExtensionPoint.CreateExtension();

            // Reload the PatientProfile so that the following call to load related works
            // If the PatientProfile is not reloaded within this context, nHibernate will attempt to create a second PatientProfile with the
            // same OID, which results in an exception being thrown.
            patientProfile = GetPatientProfileBroker().Find(patientProfile.OID);

            Patient patient = patientProfile.Patient;
            GetPatientBroker().LoadRelated(patient, patient.Profiles);

            return strategy.FindReconciliationMatches(patientProfile, GetPatientProfileBroker());
        }

        [ReadOperation]
        public IList<PatientProfile> ListReconciledPatientProfiles(PatientProfile patientProfile)
        {
            Patient patient = patientProfile.Patient;

            // ensure that the profiles collection is loaded
            GetPatientBroker().LoadRelated(patient, patient.Profiles);

            // exclude the reference profile from the list of returned profiles
            IList<PatientProfile> reconciledProfiles = new List<PatientProfile>();
            foreach (PatientProfile profile in patient.Profiles)
            {
                if(!profile.MRN.Equals(patientProfile.MRN))
                {
                    reconciledProfiles.Add(profile);
                }
            }
            return reconciledProfiles;
        }

        [ReadOperation]
        public void LoadPatientProfiles(Patient patient)
        {
            // ensure that the profiles collection is loaded
            GetPatientBroker().LoadRelated(patient, patient.Profiles);
        }

        //[UpdateOperation]
        //public void ReconcilePatients(PatientProfile toBeKept, PatientProfile toBeReconciled)
        //{
        //    if( toBeKept == null )
        //    {
        //        throw new PatientReconciliationException("Patient to be kept is null");
        //    }
        //    if (toBeKept == toBeReconciled)
        //    {
        //        throw new PatientReconciliationException("Patients are the same");
        //    }
        //    IList<PatientProfile> list = new List<PatientProfile>();
        //    list.Add(toBeReconciled);
        //    DoReconciliation(toBeKept.Patient, list);
        //}

        //[UpdateOperation]
        //public void ReconcilePatients(Patient patient, PatientProfile toBeReconciled)
        //{
        //    IList<PatientProfile> list = new List<PatientProfile>();
        //    list.Add(toBeReconciled);
        //    DoReconciliation(patient, list);
        //}

        //[UpdateOperation]
        //public void ReconcilePatients(Patient patient, IList<PatientProfile> toBeReconciled)
        //{
        //        DoReconciliation(patient, toBeReconciled);
        //}

        [UpdateOperation]
        public void ReconcilePatient(Patient toBeKept, IList<Patient> toBeReconciled)
        {
            DoReconciliation(toBeKept, toBeReconciled);
        }

        #endregion

        //private void DoReconciliation(Patient patient, IList<PatientProfile> toBeReconciled)
        //{
        //    foreach (PatientProfile profile in toBeReconciled)
        //    {
        //        PatientIdentifier mrnToBeReconciled = profile.MRN;
        //        if (mrnToBeReconciled != null &&
        //            PatientHasProfileForSite(patient, mrnToBeReconciled.AssigningAuthority) == true)
        //        {
        //            throw new PatientReconciliationException("Patient already has identifier for site " + mrnToBeReconciled.AssigningAuthority);
        //        }

        //        // perform some additional validation on the profile?
        //        patient.AddProfile(profile);
        //    }

        //    GetPatientBroker().Store(patient);
        //}

        private void DoReconciliation(Patient toBeKept, IList<Patient> toBeReconciled)
        {
            if (PatientIdentifierConflictsFound(toBeKept, toBeReconciled) == true)
            {
                throw new PatientReconciliationException("Patient already has identifier for site");
            }

            foreach (Patient patient in toBeReconciled)
            {
                foreach (PatientProfile profile in patient.Profiles)
                {
                    toBeKept.AddProfile(profile);                    
                }
            }
            
            GetPatientBroker().Store(toBeKept);
        }

        private static bool PatientIdentifierConflictsFound(Patient toBeKept, IList<Patient> toBeReconciled)
        {
            foreach (Patient patient in toBeReconciled)
            {
                foreach (PatientProfile profile in patient.Profiles)
                {
                    CompositeIdentifier mrnToBeReconciled = profile.MRN;
                    if (mrnToBeReconciled != null &&
                        PatientHasProfileForSite(toBeKept, mrnToBeReconciled.AssigningAuthority) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool PatientHasProfileForSite(Patient patient, string site)
        {
            foreach (PatientProfile profile in patient.Profiles)
            {
                CompositeIdentifier id = profile.MRN;
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
