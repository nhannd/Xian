using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
	/// <summary>
	/// Data contract for updating a protocol
	/// <remarks>
	/// The contract should only be populated via the constructor.  Specifically, the protocol's supervisor should only be set via either the 
	/// "protocol" parameter in the three argument constructor, or the "supervisorRef" parameter of the two argument constructor.  This criteria 
	/// is enforced by <see cref="IProtocollingWorkflowService"/> and will cause a <see cref="RequestValidationException"/> if not met.
	/// </remarks>
	/// </summary>
	[DataContract]
	public class UpdateProtocolRequest : DataContractBase
	{
		public UpdateProtocolRequest(EntityRef protocolAssignmentStepRef, ProtocolDetail protocol, List<OrderNoteDetail> orderNotes)
		{
			this.ProtocolAssignmentStepRef = protocolAssignmentStepRef;
			this.Protocol = protocol;
			this.OrderNotes = orderNotes;
		}

		public UpdateProtocolRequest(EntityRef protocolAssignmentStepRef, EntityRef supervisorRef)
		{
			this.ProtocolAssignmentStepRef = protocolAssignmentStepRef;
			this.SupervisorRef = supervisorRef;
		}

		[DataMember]
		public EntityRef ProtocolAssignmentStepRef;

		[DataMember]
		public ProtocolDetail Protocol;

		[DataMember]
		public List<OrderNoteDetail> OrderNotes;

		[DataMember]
		public EntityRef SupervisorRef;
	}
}