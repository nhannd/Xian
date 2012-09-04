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
	public class FacilityDetail : DataContractBase
	{
		public FacilityDetail(EntityRef facilityRef, string code, string name, string description, EnumValueInfo informationAuthority, bool deactivated)
		{
			this.FacilityRef = facilityRef;
			this.Code = code;
			this.Name = name;
			this.Description = description;
			this.InformationAuthority = informationAuthority;
			this.Deactivated = deactivated;
		}

		public FacilityDetail()
		{
		}

		[DataMember]
		public EntityRef FacilityRef;

		[DataMember]
		public string Code;

		[DataMember]
		public string Name;

		[DataMember]
		public string Description;

		[DataMember]
		public EnumValueInfo InformationAuthority;

		[DataMember]
		public bool Deactivated;
	}
}