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
            return this.CurrentContext.GetBroker<IPatientProfileBroker>().Find(criteria);
        }

        [ReadOperation]
        public IList<PatientProfileMatch> FindPatientReconciliationMatches(PatientProfile patientProfile)
        {
            IPatientReconciliationStrategy strategy = (IPatientReconciliationStrategy)_strategyExtensionPoint.CreateExtension();

            IPatientProfileBroker broker = this.CurrentContext.GetBroker<IPatientProfileBroker>();

            // Reload the PatientProfile so that the following call to load related works
            // If the PatientProfile is not reloaded within this context, nHibernate will attempt to create a second PatientProfile with the
            // same OID, which results in an exception being thrown.
            patientProfile = broker.Find(patientProfile.OID);

            Patient patient = patientProfile.Patient;
            this.CurrentContext.GetBroker<IPatientBroker>().LoadProfiles(patient);

            return strategy.FindReconciliationMatches(patientProfile, broker);
        }

        [ReadOperation]
        public void LoadPatientProfiles(Patient patient)
        {
            // ensure that the profiles collection is loaded
            IPatientBroker broker = this.CurrentContext.GetBroker<IPatientBroker>();
            broker.LoadProfiles(patient);
        }

        [ReadOperation]
        public void LoadPatientProfileDetails(PatientProfile profile)
        {
            IPatientProfileBroker broker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            broker.LoadAddresses(profile);
            broker.LoadTelephoneNumbers(profile);
        }

        [ReadOperation]
        public PatientProfile LoadPatientProfile(long oid, bool withDetails)
        {
            IPatientProfileBroker broker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            PatientProfile profile = broker.Find(oid);
            if (withDetails)
            {
                broker.LoadAddresses(profile);
                broker.LoadTelephoneNumbers(profile);
            }
            return profile;
        }

        [UpdateOperation]
        public void ReconcilePatients(Patient toBeKept, IList<Patient> toBeReconciled)
        {
            DoReconciliation(toBeKept, toBeReconciled);
        }

        [UpdateOperation]
        public Patient CreatePatientForProfile(PatientProfile profile)
        {
            Patient patient = Patient.New();
            patient.AddProfile(profile);

            IPatientBroker broker = this.CurrentContext.GetBroker<IPatientBroker>();
            broker.Store(patient);

            return patient;
        }

        [UpdateOperation]
        public void UpdatePatientProfile(PatientProfile profile)
        {
            // first save changes to the profile
            IPatientProfileBroker profileBroker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            profileBroker.Store(profile);

            // now save the patient itself, which should update its version
            IPatientBroker patientBroker = this.CurrentContext.GetBroker<IPatientBroker>();
            patientBroker.Store(profile.Patient);
        }

        #endregion

        private void DoReconciliation(Patient destPatient, IList<Patient> sourcePatients)
        {
            if (PatientIdentifierConflictsFound(destPatient, sourcePatients) == true)
            {
                throw new PatientReconciliationException("Patient already has identifier for site");
            }

            IPatientBroker broker = this.CurrentContext.GetBroker<IPatientBroker>();
            foreach (Patient sourcePatient in sourcePatients)
            {
                // add each profile from the source to the dest patient
                foreach (PatientProfile sourceProfile in sourcePatient.Profiles)
                {
                    destPatient.AddProfile(sourceProfile);
                }

                // remove the profiles from the now defunct source patient
                sourcePatient.Profiles.Clear();

                // delete the source patient - NB: this doesn't work due to some NHibernate issues
                //broker.Delete(sourcePatient);
            }

            broker.Store(destPatient);
        }

        private bool PatientIdentifierConflictsFound(Patient toBeKept, IList<Patient> toBeReconciled)
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

        private bool PatientHasProfileForSite(Patient patient, string site)
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
