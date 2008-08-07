using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class ReassignProcedureStepRequest : DataContractBase
	{
		public ReassignProcedureStepRequest(EntityRef procedureStepRef, StaffSummary reassignedRadiologist)
		{
			this.ProcedureStepRef = procedureStepRef;
			this.ReassignedRadiologist = reassignedRadiologist;
		}

		[DataMember]
		public EntityRef ProcedureStepRef;

		[DataMember]
		public StaffSummary ReassignedRadiologist;
	}
}
