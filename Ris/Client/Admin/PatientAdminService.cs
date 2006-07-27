using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

using Iesi.Collections;

namespace ClearCanvas.Ris.Client.Admin
{
    [ExtensionOf(typeof(ClearCanvas.Enterprise.ServiceLayerExtensionPoint))]
    public class PatientAdminService : HealthcareServiceLayer, IPatientAdminService
    {
        public PatientAdminService()
        {
        }

        #region IPatientAdminService Members

        [ReadOperation]
        public IList<Patient> ListPatients(PatientSearchCriteria criteria)
        {
            return this.GetPatientBroker().Find(criteria);
        }

        [ReadOperation]
        public Patient LoadPatientDetails(long oid)
        {
            IPatientBroker broker = this.GetPatientBroker();
            Patient patient = broker.Find(oid);

            // load all relevant collections
            broker.LoadRelated(patient, patient.Identifiers);
            broker.LoadRelated(patient, patient.Addresses);
            broker.LoadRelated(patient, patient.TelephoneNumbers);

            return patient;
        }

        [UpdateOperation]
        public void AddNewPatient(Patient patient)
        {
            this.GetPatientBroker().Store(patient);
        }

        [UpdateOperation]
        public void UpdatePatient(Patient patient)
        {
            this.GetPatientBroker().Store(patient);
        }

        #endregion
    }
}
