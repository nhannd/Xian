using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Services.Admin.Practitioner
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class PractitionerAdminService : ApplicationServiceBase, IPractitionerAdminService
    {
        #region IPractitionerAdminService Members

        [ReadOperation]
        public IList<Practitioner> FindPractitioners(string surname, string givenName)
        {
            Platform.CheckForNullReference(surname, "surname");

            IPractitionerBroker broker = this.CurrentContext.GetBroker<IPractitionerBroker>();

            PractitionerSearchCriteria criteria = new PractitionerSearchCriteria();
            criteria.Name.FamilyName.Like(StringCriteriaWithWildcardAppendedTo(surname));
            if (givenName != null)
            {
                criteria.Name.GivenName.Like(StringCriteriaWithWildcardAppendedTo(givenName));
            }

            return broker.Find(criteria);
        }

        private string StringCriteriaWithWildcardAppendedTo(string surname)
        {
            return surname.IndexOf('%') < 0 ? surname + "%" : surname;
        }

        [ReadOperation]
        public Practitioner LoadPractitioner(EntityRef practitionerRef)
        {
            IPractitionerBroker broker = this.CurrentContext.GetBroker<IPractitionerBroker>();
            return broker.Load(practitionerRef);
        }

        [ReadOperation]
        public IList<Practitioner> GetAllPractitioners()
        {
            return this.CurrentContext.GetBroker<IPractitionerBroker>().FindAll();
        }

        [UpdateOperation]
        public void AddPractitioner(Practitioner practitioner)
        {
            this.CurrentContext.Lock(practitioner, DirtyState.New);
        }

        [UpdateOperation]
        public void UpdatePractitioner(Practitioner practitioner)
        {
            this.CurrentContext.Lock(practitioner, DirtyState.Dirty);
        }

        [ReadOperation]
        public Practitioner LoadPractitioner(EntityRef practitionerRef, bool withDetails)
        {
            IPractitionerBroker practitionerBroker = CurrentContext.GetBroker<IPractitionerBroker>();
            Practitioner practitioner = practitionerBroker.Load(practitionerRef);
            if (withDetails)
            {
                IStaffBroker staffBroker = CurrentContext.GetBroker<IStaffBroker>();
                staffBroker.LoadAddressesForStaff(practitioner);
                staffBroker.LoadTelephoneNumbersForStaff(practitioner);
            }
            return practitioner;
        }

        #endregion
    }
}
