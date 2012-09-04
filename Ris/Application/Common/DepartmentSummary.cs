#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class DepartmentSummary : DataContractBase, IEquatable<DepartmentSummary>
	{
		public DepartmentSummary()
		{
		}

		public DepartmentSummary(EntityRef departmentRef, string id, string name, string facilityCode, string facilityName, bool deactivated)
		{
			this.DepartmentRef = departmentRef;
			this.Id = id;
			this.Name = name;
			this.FacilityCode = facilityCode;
			this.FacilityName = facilityName;
			this.Deactivated = deactivated;
		}

		[DataMember]
		public EntityRef DepartmentRef;

		[DataMember]
		public string Id;

		[DataMember]
		public string Name;

		[DataMember]
		public string FacilityCode;

		[DataMember]
		public string FacilityName;

		[DataMember]
		public bool Deactivated;

		public bool Equals(DepartmentSummary other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.DepartmentRef, DepartmentRef);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as DepartmentSummary);
		}

		public override int GetHashCode()
		{
			return (DepartmentRef != null ? DepartmentRef.GetHashCode() : 0);
		}
	}
}