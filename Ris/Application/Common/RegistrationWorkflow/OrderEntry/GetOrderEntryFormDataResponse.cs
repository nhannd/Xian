#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using System.Collections.Generic;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
	[DataContract]
	public class GetOrderEntryFormDataResponse : DataContractBase
	{
		public GetOrderEntryFormDataResponse(
			List<FacilitySummary> facilityChoices,
			List<DepartmentSummary> departmentChoices,
			List<ModalitySummary> modalityChoices,
			List<EnumValueInfo> orderPriorityChoices,
			List<EnumValueInfo> cancelReasonChoices,
			List<EnumValueInfo> lateralityChoices,
			List<EnumValueInfo> schedulingCodeChoices)
		{
			this.FacilityChoices = facilityChoices;
			this.DepartmentChoices = departmentChoices;
			this.ModalityChoices = modalityChoices;
			this.OrderPriorityChoices = orderPriorityChoices;
			this.CancelReasonChoices = cancelReasonChoices;
			this.LateralityChoices = lateralityChoices;
			this.SchedulingCodeChoices = schedulingCodeChoices;
		}

		[DataMember]
		public List<FacilitySummary> FacilityChoices;

		[DataMember]
		public List<DepartmentSummary> DepartmentChoices;

		[DataMember]
		public List<EnumValueInfo> OrderPriorityChoices;

		[DataMember]
		public List<EnumValueInfo> CancelReasonChoices;

		[DataMember]
		public List<EnumValueInfo> LateralityChoices;

		[DataMember]
		public List<EnumValueInfo> SchedulingCodeChoices;

		[DataMember]
		public List<ModalitySummary> ModalityChoices;
	}
}
