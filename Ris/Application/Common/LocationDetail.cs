#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class LocationDetail : DataContractBase
    {
        public LocationDetail(
			EntityRef locationRef,
			string id,
			string name,
			string description,
            FacilitySummary facility,
            string building,
            string floor,
            string pointOfCare,
            string room,
            string bed, 
			bool deactivated)
        {
        	this.LocationRef = locationRef;
        	this.Id = id;
        	this.Name = name;
        	this.Description = description;
            this.Facility = facility;
            this.Building = building;
            this.Floor = floor;
            this.PointOfCare = pointOfCare;
            this.Room = room;
            this.Bed = bed;
        	this.Deactivated = deactivated;
        }

        public LocationDetail()
        {
        }


		[DataMember]
		public EntityRef LocationRef;

		[DataMember]
		public string Id;

		[DataMember]
		public string Name;

		[DataMember]
		public string Description;

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

		[DataMember]
		public bool Deactivated;
	}
}
