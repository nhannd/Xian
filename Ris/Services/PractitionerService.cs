using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Ris.Services
{
    [ExtensionOf(typeof(ServiceLayerExtensionPoint))]
    public class PractitionerService : HealthcareServiceLayer, IPractitionerService
    {
        #region IPractitionerService Members

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
        public Practitioner LoadPractitioner(EntityRef<Practitioner> practitionerRef)
        {
            IPractitionerBroker broker = this.CurrentContext.GetBroker<IPractitionerBroker>();
            return broker.Load(practitionerRef);
        }

        [UpdateOperation]
        public void AddPractitioner(Practitioner practitioner)
        {
            this.CurrentContext.Lock(practitioner, DirtyState.New);
        }

        #endregion
    }
}
