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
    public class LocationAdminService : HealthcareServiceLayer, ILocationAdminService
    {
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

        [UpdateOperation]
        public void UpdateLocation(Location location)
        {
            this.CurrentContext.Lock(location, DirtyState.Dirty);
        }

        [ReadOperation]
        public Location LoadLocation(EntityRef<Location> locationRef)
        {
            ILocationBroker locationBroker = CurrentContext.GetBroker<ILocationBroker>();
            Location location = locationBroker.Load(locationRef);
            locationBroker.LoadFacilityForLocation(location);
            return location;
        }
    }
}
