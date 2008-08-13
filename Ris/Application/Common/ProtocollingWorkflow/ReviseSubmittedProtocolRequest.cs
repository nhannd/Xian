using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
	[DataContract]
	public class ReviseSubmittedProtocolRequest : DataContractBase
	{
		public ReviseSubmittedProtocolRequest(EntityRef orderRef)
		{
			this.OrderRef = orderRef;
		}

		[DataMember]
		public EntityRef OrderRef;
	}
}