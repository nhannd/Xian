#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
	[DataContract]
	public class LoadStaffEditorFormDataResponse : DataContractBase
	{
		public LoadStaffEditorFormDataResponse(
			List<EnumValueInfo> staffTypeChoices,
			List<EnumValueInfo> sexChoices,
			List<EnumValueInfo> phoneTypeChoices,
			List<EnumValueInfo> addressTypeChoices,
			List<StaffGroupSummary> groupChoices)
		{
			this.StaffTypeChoices = staffTypeChoices;
			this.SexChoices = sexChoices;
			this.PhoneTypeChoices = phoneTypeChoices;
			this.AddressTypeChoices = addressTypeChoices;
			this.StaffGroupChoices = groupChoices;
		}

		[DataMember]
		public List<EnumValueInfo> StaffTypeChoices;

		[DataMember]
		public List<EnumValueInfo> SexChoices;

		[DataMember]
		public List<EnumValueInfo> PhoneTypeChoices;

		[DataMember]
		public List<EnumValueInfo> AddressTypeChoices;

		[DataMember]
		public List<StaffGroupSummary> StaffGroupChoices;
	}
}
