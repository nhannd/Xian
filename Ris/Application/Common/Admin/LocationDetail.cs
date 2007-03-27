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
            this.Facility = new FacilitySummary(facility, facilityCode, facilityName);
            this.Building = building;
            this.Floor = floor;
            this.PointOfCare = pointOfCare;
            this.Room = room;
            this.Bed = bed;
        }

        public LocationDetail(
            FacilitySummary facility,
            string building,
            string floor,
            string pointOfCare,
            string room,
            string bed)
        {
            this.Facility = facility;
            this.Building = building;
            this.Floor = floor;
            this.PointOfCare = pointOfCare;
            this.Room = room;
            this.Bed = bed;
        }

        public LocationDetail()
        {
        }

        [DataMember]
        public FacilitySummary Facility;

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
