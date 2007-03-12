using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.Location
{
    [DataContract]
    public class GetAllLocationsResponse : DataContractBase
    {
        public GetAllLocationsResponse(List<LocationSummary> locations)
        {
            this.Locations = locations;
        }

        [DataMember]
        public List<LocationSummary> Locations;
    }

}
