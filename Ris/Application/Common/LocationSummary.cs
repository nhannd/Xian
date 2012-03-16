#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class LocationSummary : DataContractBase, IEquatable<LocationSummary>
	{
		public LocationSummary(EntityRef locationRef,
			string id,
			string name,
			FacilitySummary facility,
			string building,
			string floor,
			string pointOfCare,
			bool deactivated)
		{
			this.LocationRef = locationRef;
			this.Id = id;
			this.Name = name;
			this.Facility = facility;
			this.Building = building;
			this.Floor = floor;
			this.PointOfCare = pointOfCare;
			this.Deactivated = deactivated;
		}

		public LocationSummary()
		{
		}

		[DataMember]
		public EntityRef LocationRef;

		[DataMember]
		public string Id;

		[DataMember]
		public string Name;

		[DataMember]
		public FacilitySummary Facility;

		[DataMember]
		public string Building;

		[DataMember]
		public string Floor;

		[DataMember]
		public string PointOfCare;

		[DataMember]
		public bool Deactivated;

		public bool Equals(LocationSummary other)
		{
			if (other == null) return false;
			return Equals(LocationRef, other.LocationRef);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as LocationSummary);
		}

		public override int GetHashCode()
		{
			return LocationRef.GetHashCode();
		}
	}
}
