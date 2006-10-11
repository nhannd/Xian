using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

using Iesi.Collections;

namespace ClearCanvas.Ris.Services
{
    [ExtensionOf(typeof(ClearCanvas.Enterprise.ServiceLayerExtensionPoint))]
    public class PatientAdminService : HealthcareServiceLayer, IPatientAdminService
    {
        public PatientAdminService()
        {
        }

        #region IPatientAdminService Members

        [ReadOperation]
        public IList<PatientProfile> ListPatients(PatientProfileSearchCriteria criteria)
        {
            IPatientProfileBroker profileBroker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            return profileBroker.Find(criteria);
        }

        [ReadOperation]
        public PatientProfile LoadPatient(long oid)
        {
            IPatientProfileBroker profileBroker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            PatientProfile patient = profileBroker.Find(oid);

            return patient;
        }

        [ReadOperation]
        public PatientProfile LoadPatientDetails(long oid)
        {
            IPatientProfileBroker broker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            PatientProfile patient = broker.Find(oid);

            // load all relevant collections
            broker.LoadRelated(patient, patient.Addresses);
            broker.LoadRelated(patient, patient.TelephoneNumbers);

            return patient;
        }

        [UpdateOperation]
        public void AddNewPatient(PatientProfile patient)
        {
            IPatientBroker broker = this.CurrentContext.GetBroker<IPatientBroker>();
            broker.Store(patient.Patient);
        }

        [UpdateOperation]
        public void UpdatePatient(PatientProfile patient)
        {
            IPatientBroker broker = this.CurrentContext.GetBroker<IPatientBroker>();
            broker.Store(patient.Patient);
        }

        #endregion
    }
}
