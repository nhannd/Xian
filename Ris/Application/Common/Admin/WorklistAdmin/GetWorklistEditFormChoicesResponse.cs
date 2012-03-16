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

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
	[DataContract]
	public class GetWorklistEditFormChoicesResponse : DataContractBase
	{
		[DataMember]
		public List<WorklistClassSummary> WorklistClasses;

		[DataMember]
		public List<StaffSummary> StaffChoices;

		[DataMember]
		public List<StaffGroupSummary> GroupSubscriberChoices;

		[DataMember]
		public List<FacilitySummary> FacilityChoices;

		[DataMember]
		public List<DepartmentSummary> DepartmentChoices;

		[DataMember]
		public List<LocationSummary> PatientLocationChoices;

		[DataMember]
		public List<EnumValueInfo> PatientClassChoices;

		[DataMember]
		public List<EnumValueInfo> OrderPriorityChoices;

		[DataMember]
		public List<StaffGroupSummary> OwnerGroupChoices;

		[DataMember]
		public List<ProcedureTypeSummary> ProcedureTypeChoices;

		[DataMember]
		public bool CurrentServerConfigurationRequiresTimeFilter;

		[DataMember]
		public int CurrentServerConfigurationMaxSpanDays;
	}
}
