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
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.ProtocollingWorkflow
{
	[DataContract]
	public class AcceptProtocolRequest : UpdateProtocolRequest
	{
		public AcceptProtocolRequest(EntityRef protocolAssignmentStepRef, ProtocolDetail protocol, List<OrderNoteDetail> orderNotes)
			: base(protocolAssignmentStepRef, protocol, orderNotes)
		{
		}

		public AcceptProtocolRequest(EntityRef protocolAssignmentStepRef, EntityRef supervisorRef)
			: base(protocolAssignmentStepRef, supervisorRef)
		{
		}
	}
}