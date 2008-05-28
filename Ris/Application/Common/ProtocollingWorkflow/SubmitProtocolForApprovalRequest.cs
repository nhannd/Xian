using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
	[DataContract]
	public class SubmitProtocolForApprovalRequest : DataContractBase
	{
		public SubmitProtocolForApprovalRequest(EntityRef orderRef)
		{
			this.OrderRef = orderRef;
		}

		[DataMember]
		public EntityRef OrderRef;
	}
}