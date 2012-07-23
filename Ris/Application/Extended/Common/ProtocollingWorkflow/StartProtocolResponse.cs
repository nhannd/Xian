#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.ProtocollingWorkflow
{
	[DataContract]
	public class StartProtocolResponse : DataContractBase
	{
		public StartProtocolResponse(
			EntityRef protocolAssignmentStepRef,
			EntityRef assignedStaffRef,
			bool protocolClaimed,
			List<OrderNoteDetail> protocolNotes,
			OrderDetail order)
		{
			this.ProtocolAssignmentStepRef = protocolAssignmentStepRef;
			this.AssignedStaffRef = assignedStaffRef;
			this.ProtocolClaimed = protocolClaimed;
			this.ProtocolNotes = protocolNotes;
			this.Order = order;
		}

		[DataMember]
		public EntityRef ProtocolAssignmentStepRef;

		[DataMember]
		public EntityRef AssignedStaffRef;

		[DataMember]
		public bool ProtocolClaimed;

		[DataMember]
		public List<OrderNoteDetail> ProtocolNotes;

		[DataMember]
		public OrderDetail Order;
	}
}