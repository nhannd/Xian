#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.BrowsePatientData
{
	[DataContract]
	public class OrderListItem : VisitListItem
	{
		public OrderListItem()
		{
		}

		#region Order

		[DataMember]
		public EntityRef OrderRef;

		[DataMember]
		public string PlacerNumber;

		[DataMember]
		public string AccessionNumber;

		[DataMember]
		public DiagnosticServiceSummary DiagnosticService;

		[DataMember]
		public DateTime? EnteredTime;

		[DataMember]
		public DateTime? SchedulingRequestTime;

		[DataMember]
		public DateTime? OrderScheduledStartTime;

		[DataMember]
		public ExternalPractitionerSummary OrderingPractitioner;

		[DataMember]
		public FacilitySummary OrderingFacility;

		[DataMember]
		public string ReasonForStudy;

		[DataMember]
		public EnumValueInfo OrderPriority;

		[DataMember]
		public EnumValueInfo OrderStatus;

		[DataMember]
		public EnumValueInfo CancelReason;

		#endregion

		#region Procedure

		[DataMember]
		public EntityRef ProcedureRef;

		[DataMember]
		public ProcedureTypeSummary ProcedureType;

		[DataMember]
		public DateTime? ProcedureScheduledStartTime;

		[DataMember]
		public EnumValueInfo ProcedureSchedulingCode;

		[DataMember]
		public DateTime? ProcedureCheckInTime;

		[DataMember]
		public DateTime? ProcedureCheckOutTime;

		[DataMember]
		public EnumValueInfo ProcedureStatus;

		[DataMember]
		public FacilitySummary ProcedurePerformingFacility;

		[DataMember]
		public bool ProcedurePortable;

		[DataMember]
		public EnumValueInfo ProcedureLaterality;

		#endregion
	}
}
