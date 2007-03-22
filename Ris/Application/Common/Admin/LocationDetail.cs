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
        public LocationDetail(EntityRef facility,
            string facilityName,
            string facilityCode,
            string building,
            string floor,
            string pointOfCare,
            string room,
            string bed)
        {
            this.Facility = facility;
            this.FacilityName = facilityName;
            this.FacilityCode = facilityCode;
            this.Building = building;
            this.Floor = floor;
            this.PointOfCare = pointOfCare;
            this.Room = room;
            this.Bed = bed;
        }

        [DataMember]
        public EntityRef Facility;

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
    }
}
