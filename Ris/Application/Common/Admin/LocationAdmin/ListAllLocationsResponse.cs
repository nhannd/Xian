using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.LocationAdmin
{
    [DataContract]
    public class ListAllLocationsResponse : DataContractBase
    {
        public ListAllLocationsResponse(List<LocationSummary> locations)
        {
            this.Locations = locations;
        }

        [DataMember]
        public List<LocationSummary> Locations;
    }

}
