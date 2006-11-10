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
        public IList<Facility> GetFacilities()
        {
            FacilitySearchCriteria allMatches = new FacilitySearchCriteria();
            IFacilityBroker facilityBroker = this.CurrentContext.GetBroker<IFacilityBroker>();

            return facilityBroker.Find(allMatches);
        }

        [UpdateOperation]
        public void AddFacility(Facility facility)
        {
            this.CurrentContext.Lock(facility);
            //IFacilityBroker facilityBroker = this.CurrentContext.GetBroker<IFacilityBroker>();
            //facilityBroker.Store(facility);
        }

        #endregion
    }
}
