using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
	[DataContract]
	public class SubmitProtocolForApprovalRequest : UpdateOrderProtocolRequest
	{
		public SubmitProtocolForApprovalRequest(EntityRef orderRef, List<ProtocolDetail> protocols, List<OrderNoteDetail> orderNotes, EntityRef supervisorStaffRef)
			: base(orderRef, protocols, orderNotes)
		{
			this.Supervisor = supervisorStaffRef;
		}

		[DataMember]
		public EntityRef Supervisor;
	}
}