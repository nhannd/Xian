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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ProcedureStepSummary : DataContractBase
	{
		public ProcedureStepSummary(
			EntityRef procedureStepRef,
			string procedureStepName,
			EnumValueInfo state,
			DateTime? startTime,
			DateTime? endTime,
			ModalitySummary modality,
			ProcedureSummary procedure
			)
		{
			this.ProcedureStepRef = procedureStepRef;
			this.ProcedureStepName = procedureStepName;
			this.State = state;
			this.StartTime = startTime;
			this.EndTime = endTime;
			this.Modality = modality;
			this.Procedure = procedure;
		}

		[DataMember]
		public EntityRef ProcedureStepRef;

		[DataMember]
		public string ProcedureStepName;

		[DataMember]
		public EnumValueInfo State;

		[DataMember]
		public DateTime? StartTime;

		[DataMember]
		public DateTime? EndTime;

		[DataMember]
		public ProcedureSummary Procedure;

		/// <summary>
		/// Specifies the modality of a MPS.  This field is null for other types of procedure step.
		/// </summary>
		[DataMember]
		public ModalitySummary Modality;

	}
}
