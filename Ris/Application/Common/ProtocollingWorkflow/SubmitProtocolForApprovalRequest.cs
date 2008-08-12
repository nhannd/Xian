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
}