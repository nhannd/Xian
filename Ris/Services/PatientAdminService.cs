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
            return this.GetPatientProfileBroker().Find(criteria);
        }

        [ReadOperation]
        public PatientProfile LoadPatient(long oid)
        {
            IPatientProfileBroker broker = this.GetPatientProfileBroker();
            PatientProfile patient = broker.Find(oid);

            // load all relevant collections
            broker.LoadRelated(patient, patient.Identifiers);
            return patient;
        }

        [ReadOperation]
        public PatientProfile LoadPatientDetails(long oid)
        {
            IPatientProfileBroker broker = this.GetPatientProfileBroker();
            PatientProfile patient = broker.Find(oid);

            // load all relevant collections
            broker.LoadRelated(patient, patient.Identifiers);
            broker.LoadRelated(patient, patient.Addresses);
            broker.LoadRelated(patient, patient.TelephoneNumbers);

            return patient;
        }

        [UpdateOperation]
        public void AddNewPatient(PatientProfile patient)
        {
            this.GetPatientBroker().Store(patient.Patient);
            this.GetPatientProfileBroker().Store(patient);
        }

        [UpdateOperation]
        public void UpdatePatient(PatientProfile patient)
        {
            this.GetPatientBroker().Store(patient.Patient);
            this.GetPatientProfileBroker().Store(patient);
        }

        #endregion
    }
}
