using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class LocationDetail : DataContractBase
    {
        [DataMember]
        public EntityRef Facility;

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
    }
}
