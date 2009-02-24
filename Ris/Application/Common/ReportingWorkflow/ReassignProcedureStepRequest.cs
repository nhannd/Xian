using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class ReassignProcedureStepRequest : DataContractBase
	{
		public ReassignProcedureStepRequest(EntityRef procedureStepRef, EntityRef reassignedRadiologistRef)
		{
			this.ProcedureStepRef = procedureStepRef;
			this.ReassignedRadiologistRef = reassignedRadiologistRef;
		}

		[DataMember]
		public EntityRef ProcedureStepRef;

		[DataMember]
		public EntityRef ReassignedRadiologistRef;
	}
}
