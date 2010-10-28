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

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ModalityProcedureStepSummary : ProcedureStepSummary
	{
		public ModalityProcedureStepSummary(ProcedureStepSummary ps, string description)
			: base(ps.ProcedureStepRef, ps.ProcedureStepName, ps.State, ps.StartTime, ps.EndTime, ps.Modality, ps.Procedure)
		{
			this.Description = description;
		}

		public ModalityProcedureStepSummary(
			EntityRef procedureStepRef,
			string procedureStepName,
			EnumValueInfo state,
			DateTime? startTime,
			DateTime? endTime,
			ModalitySummary modality,
			ProcedureSummary procedure,
			String description)
			: base(procedureStepRef, procedureStepName, state, startTime, endTime, modality, procedure)
		{
			this.Description = description;
		}

		[DataMember]
		public string Description;
	}
}
