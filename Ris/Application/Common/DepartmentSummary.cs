using System;
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

		public DepartmentSummary(EntityRef departmentRef, string id, string name, FacilitySummary facility, bool deactivated)
		{
			this.DepartmentRef = departmentRef;
			this.Id = id;
			this.Name = name;
			this.Facility = facility;
			this.Deactivated = deactivated;
		}

		[DataMember]
		public EntityRef DepartmentRef;

		[DataMember]
		public string Id;

		[DataMember]
		public string Name;

		[DataMember]
		public FacilitySummary Facility;

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