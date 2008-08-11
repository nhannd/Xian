using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class ReassignProcedureStepRequest : DataContractBase
	{
		public ReassignProcedureStepRequest(EntityRef procedureStepRef, EntityRef reassignedRadiologistRef)
			: this(procedureStepRef, reassignedRadiologistRef, false)
		{
		}

		public ReassignProcedureStepRequest(EntityRef procedureStepRef, EntityRef reassignedRadiologistRef, bool keepReportPart)
		{
			this.ProcedureStepRef = procedureStepRef;
			this.ReassignedRadiologistRef = reassignedRadiologistRef;
			this.KeepReportPart = keepReportPart;
		}

		[DataMember]
		public EntityRef ProcedureStepRef;

		[DataMember]
		public EntityRef ReassignedRadiologistRef;

		[DataMember]
		public bool KeepReportPart;
	}
}
