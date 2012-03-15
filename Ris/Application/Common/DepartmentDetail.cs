#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class DepartmentDetail : DataContractBase
	{
		public DepartmentDetail()
		{
		}

		public DepartmentDetail(EntityRef departmentRef, string id, string name, string description, FacilitySummary facility, bool deactivated)
		{
			this.DepartmentRef = departmentRef;
			this.Id = id;
			this.Name = name;
			this.Description = description;
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
		public string Description;

		[DataMember]
		public FacilitySummary Facility;

		[DataMember]
		public bool Deactivated;
	}
}