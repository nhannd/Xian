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
