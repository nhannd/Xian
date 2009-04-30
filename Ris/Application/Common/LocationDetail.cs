#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
