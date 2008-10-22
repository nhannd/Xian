using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
	[DataContract]
	public class GetLinkableProtocolsRequest : DataContractBase
	{
		public GetLinkableProtocolsRequest(EntityRef protocolAssignmentStepRef)
		{
			this.ProtocolAssignmentStepRef = protocolAssignmentStepRef;
		}

		[DataMember]
		public EntityRef ProtocolAssignmentStepRef;
	}
}