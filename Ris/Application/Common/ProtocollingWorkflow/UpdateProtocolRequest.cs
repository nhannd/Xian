using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
	[DataContract]
	public class UpdateProtocolRequest : DataContractBase
	{
		public UpdateProtocolRequest(EntityRef protocolAssignmentStepRef, ProtocolDetail protocol, List<OrderNoteDetail> orderNotes)
		{
			this.ProtocolAssignmentStepRef = protocolAssignmentStepRef;
			this.Protocol = protocol;
			this.OrderNotes = orderNotes;
		}

		[DataMember]
		public EntityRef ProtocolAssignmentStepRef;

		[DataMember]
		public ProtocolDetail Protocol;

		[DataMember]
		public List<OrderNoteDetail> OrderNotes;
	}
}