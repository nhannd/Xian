using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Healthcare;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Services
{
    [ExtensionOf(typeof(ClearCanvas.Enterprise.ServiceLayerExtensionPoint))]
    public class AdtReferenceDataService : HealthcareServiceLayer, IAdtReferenceDataService
    {
        #region IAdtReferenceDataService Members

        [ReadOperation]
        public IList<Facility> GetAllFacilities()
        {
            return this.CurrentContext.GetBroker<IFacilityBroker>().FindAll();
        }

        [UpdateOperation]
        public void AddFacility(string facilityName)
        {
            Facility facility = new Facility();
            this.CurrentContext.Lock(facility, DirtyState.New);
            facility.Name = facilityName;
        }

        [ReadOperation]
        public IList<Location> GetAllLocations()
        {
            return this.CurrentContext.GetBroker<ILocationBroker>().FindAll();
        }

        [ReadOperation]
        public IList<Location> GetLocations(EntityRef<Facility> facility)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        [UpdateOperation]
        public void AddLocation(Location location)
        {
            this.CurrentContext.Lock(location, DirtyState.New);
        }

        #endregion
    }
}
