using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.Location
{
    [DataContract]
    public class LocationSummary : DataContractBase
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
