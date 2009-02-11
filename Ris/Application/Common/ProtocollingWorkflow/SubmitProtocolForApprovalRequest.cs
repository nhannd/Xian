using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
	[DataContract]
	public class SubmitProtocolForApprovalRequest : UpdateProtocolRequest
	{
		public SubmitProtocolForApprovalRequest(EntityRef protocolAssignmentStepRef, ProtocolDetail protocol, List<OrderNoteDetail> orderNotes)
			: base(protocolAssignmentStepRef, protocol, orderNotes)
		{
		}

		public SubmitProtocolForApprovalRequest(EntityRef protocolAssignmentStepRef, EntityRef supervisorRef)
			: base(protocolAssignmentStepRef, supervisorRef)
		{
		}
	}
}