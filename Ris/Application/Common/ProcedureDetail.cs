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
using ClearCanvas.Enterprise.Common;
using System;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ProcedureDetail : DataContractBase
	{
		[DataMember]
		public EntityRef ProcedureRef;

		[DataMember]
		public EnumValueInfo Status;

		[DataMember]
		public ProcedureTypeSummary Type;

		[DataMember]
		public DateTime? ScheduledStartTime;

		[DataMember]
		public EnumValueInfo SchedulingCode;

		[DataMember]
		public DateTime? StartTime;

		[DataMember]
		public DateTime? EndTime;

		[DataMember]
		public DateTime? CheckInTime;

		[DataMember]
		public DateTime? CheckOutTime;

		[DataMember]
		public FacilitySummary PerformingFacility;

		[DataMember]
		public DepartmentSummary PerformingDepartment;

		[DataMember]
		public EnumValueInfo Laterality;

		[DataMember]
		public EnumValueInfo ImageAvailability;

		[DataMember]
		public bool Portable;

		[DataMember]
		public List<ProcedureStepDetail> ProcedureSteps;

		[DataMember]
		public ProtocolDetail Protocol;
	}
}