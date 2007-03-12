using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Services.Admin.FacilityAdmin
{
    [ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
    public class FacilityAdminService : ApplicationServiceBase, IFacilityAdminService
    {
        [ReadOperation]
        public IList<FacilityAdmin> GetAllFacilities()
        {
            return this.CurrentContext.GetBroker<IFacilityBroker>().FindAll();
        }

        [UpdateOperation]
        public void AddFacility(string facilityName)
        {
            FacilityAdmin facility = new FacilityAdmin();
            facility.Name = facilityName;
            this.CurrentContext.Lock(facility, DirtyState.New);
        }

        [UpdateOperation]
        public void AddFacility(FacilityAdmin facility)
        {
            this.CurrentContext.Lock(facility, DirtyState.New);
        }

        [UpdateOperation]
        public void UpdateFacility(FacilityAdmin facility)
        {
            this.CurrentContext.Lock(facility, DirtyState.Dirty);
        }

        [ReadOperation]
        public FacilityAdmin LoadFacility(EntityRef facilityRef)
        {
            IFacilityBroker facilityBroker = CurrentContext.GetBroker<IFacilityBroker>();
            return facilityBroker.Load(facilityRef);
        }
    }
}
