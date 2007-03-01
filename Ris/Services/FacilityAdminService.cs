using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Services
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class FacilityAdminService : HealthcareServiceLayer, IFacilityAdminService
    {
        [ReadOperation]
        public IList<Facility> GetAllFacilities()
        {
            return this.CurrentContext.GetBroker<IFacilityBroker>().FindAll();
        }

        [UpdateOperation]
        public void AddFacility(string facilityName)
        {
            Facility facility = new Facility();
            facility.Name = facilityName;
            this.CurrentContext.Lock(facility, DirtyState.New);
        }

        [UpdateOperation]
        public void AddFacility(Facility facility)
        {
            this.CurrentContext.Lock(facility, DirtyState.New);
        }

        [UpdateOperation]
        public void UpdateFacility(Facility facility)
        {
            this.CurrentContext.Lock(facility, DirtyState.Dirty);
        }

        [ReadOperation]
        public Facility LoadFacility(EntityRef facilityRef)
        {
            IFacilityBroker facilityBroker = CurrentContext.GetBroker<IFacilityBroker>();
            return facilityBroker.Load(facilityRef);
        }
    }
}
