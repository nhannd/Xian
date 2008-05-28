using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
	[DataContract]
	public class GetOperationEnablementRequest : DataContractBase
	{
		public GetOperationEnablementRequest(EntityRef orderRef, EntityRef procedureStepRef)
		{
			this.OrderRef = orderRef;
			this.ProcedureStepRef = procedureStepRef;
		}

		[DataMember]
		public EntityRef ProcedureStepRef;

		[DataMember]
		public EntityRef OrderRef;
	}
}