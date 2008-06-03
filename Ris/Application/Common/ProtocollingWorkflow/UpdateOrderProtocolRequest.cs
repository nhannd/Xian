using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
	[DataContract]
	public class UpdateOrderProtocolRequest : DataContractBase
	{
		public UpdateOrderProtocolRequest(EntityRef orderRef, List<ProtocolDetail> protocols,  List<OrderNoteDetail> orderNotes)
		{
			this.OrderRef = orderRef;
			this.Protocols = protocols;
			this.OrderNotes = orderNotes;
		}

		[DataMember]
		public EntityRef OrderRef;

		[DataMember]
		public List<ProtocolDetail> Protocols;

		[DataMember]
		public List<OrderNoteDetail> OrderNotes;
	}
}