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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ProcedureSummary : DataContractBase
	{
		public ProcedureSummary(EntityRef rpRef, EntityRef orderRef, string index, ProcedureTypeSummary type, EnumValueInfo laterality, bool portable)
		{
			this.ProcedureRef = rpRef;
			this.OrderRef = orderRef;
			this.Index = index;
			this.Type = type;
			this.Laterality = laterality;
			this.Portable = portable;
		}

		public ProcedureSummary()
		{
		}

		[DataMember]
		public EntityRef ProcedureRef;

		[DataMember]
		public EntityRef OrderRef;

		[DataMember]
		public string Index;

		[DataMember]
		public DateTime? ScheduledStartTime;

		[DataMember]
		public EnumValueInfo SchedulingCode;

		[DataMember]
		public FacilitySummary PerformingFacility;

		[DataMember]
		public ProcedureTypeSummary Type;

		[DataMember]
		public EnumValueInfo Laterality;

		[DataMember]
		public bool Portable;
	}
}
