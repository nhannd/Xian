using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class GetAllLocationsResponse : DataContractBase
    {
        [DataMember]
        public LocationListItem[] Locations;
    }

    [DataContract]
    public class LocationListItem : DataContractBase
    {
        [DataMember]
        public EntityRef LocationRef;

        [DataMember]
        public string FacilityName;

        [DataMember]
        public string FacilityCode;

        [DataMember]
        public string Building;

        [DataMember]
        public string Floor;

        [DataMember]
        public string PointOfCare;

        [DataMember]
        public string Room;

        [DataMember]
        public string Bed;

        [DataMember]
        public bool Active;

        [DataMember]
        public DateTime? InactiveDate;

    }
}
