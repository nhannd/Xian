using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

using Iesi.Collections;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Services.Admin.Admin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class PatientAdminService : ApplicationServiceBase, IPatientAdminService
    {
        public PatientAdminService()
        {
        }

        #region IPatientAdminService Members

        [ReadOperation]
        public IList<PatientProfile> ListPatientProfiles(PatientProfileSearchCriteria criteria)
        {
            IPatientProfileBroker profileBroker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            return profileBroker.Find(criteria);
        }

        [ReadOperation]
        public PatientProfile LoadPatientProfile(EntityRef profileRef)
        {
            IPatientProfileBroker profileBroker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            PatientProfile patient = profileBroker.Load(profileRef);

            return patient;
        }

        [ReadOperation]
        public PatientProfile LoadPatientProfileDetails(EntityRef profileRef)
        {
            IPatientProfileBroker broker = this.CurrentContext.GetBroker<IPatientProfileBroker>();
            PatientProfile patient = broker.Load(profileRef);

            // load all relevant collections
            broker.LoadAddressesForPatientProfile(patient);
            broker.LoadTelephoneNumbersForPatientProfile(patient);

            return patient;
        }

        [UpdateOperation]
        public void AddNewPatient(PatientProfile patient)
        {
            throw new NotImplementedException();
        }

        [UpdateOperation]
        public void UpdatePatientProfile(PatientProfile patient)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
