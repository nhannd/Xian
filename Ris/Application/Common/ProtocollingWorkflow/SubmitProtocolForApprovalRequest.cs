using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
	[DataContract]
	public class SubmitProtocolForApprovalRequest : UpdateOrderProtocolRequest
	{
		public SubmitProtocolForApprovalRequest(EntityRef orderRef, List<ProtocolDetail> protocols, List<OrderNoteDetail> orderNotes)
			: base(orderRef, protocols, orderNotes)
		{
		}

		public SubmitProtocolForApprovalRequest(EntityRef orderRef)
			: this(orderRef, null, null)
		{
		}
	}

	[DataContract]
	public class SubmitProtocolForApprovalRequest2 : UpdateProtocolRequest
	{
		public SubmitProtocolForApprovalRequest2(EntityRef protocolAssignmentStepRef, ProtocolDetail protocol, List<OrderNoteDetail> orderNotes)
			: base(protocolAssignmentStepRef, protocol, orderNotes)
		{
		}

		public SubmitProtocolForApprovalRequest2(EntityRef protocolAssignmentStepRef)
			: this(protocolAssignmentStepRef, null, null)
		{
		}
	}
}