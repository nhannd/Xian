using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class ReassignProcedureStepResponse : DataContractBase
	{
		public ReassignProcedureStepResponse(EntityRef procedureStepRef)
		{
			this.ProcedureStepRef = procedureStepRef;
		}

		[DataMember]
		public EntityRef ProcedureStepRef;
	}
}
