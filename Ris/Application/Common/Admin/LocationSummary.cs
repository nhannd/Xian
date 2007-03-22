using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class LocationSummary : DataContractBase
    {
        public LocationSummary(EntityRef locationRef,
            string facilityName,
            string facilityCode,
            string building,
            string floor,
            string pointOfCare,
            string room,
            string bed,
            bool active,
            DateTime? inactiveDate)
        {
            this.LocationRef = locationRef;
            this.FacilityName = facilityName;
            this.FacilityCode = facilityCode;
            this.Building = building;
            this.Floor = floor;
            this.PointOfCare = pointOfCare;
            this.Room = room;
            this.Bed = bed;
            this.Active = active;
            this.InactiveDate = inactiveDate;
        }

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
