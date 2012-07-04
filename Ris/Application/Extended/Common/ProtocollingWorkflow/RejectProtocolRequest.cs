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
	public class RejectProtocolRequest : UpdateProtocolRequest
	{
		public RejectProtocolRequest(
			EntityRef protocolAssignmentStepRef,
			ProtocolDetail protocol,
			List<OrderNoteDetail> orderNotes,
			EnumValueInfo rejectReason,
			OrderNoteDetail additionalCommentsNote)
			: base(protocolAssignmentStepRef, protocol, orderNotes)
		{
			this.RejectReason = rejectReason;
			this.AdditionalCommentsNote = additionalCommentsNote;
		}

		public RejectProtocolRequest(
			EntityRef protocolAssignmentStepRef, 
			EntityRef supervisorRef, 
			EnumValueInfo rejectReason, 
			OrderNoteDetail additionalCommentsNote)
			: base(protocolAssignmentStepRef, supervisorRef)
		{
			this.RejectReason = rejectReason;
			this.AdditionalCommentsNote = additionalCommentsNote;
		}

		[DataMember]
		public EnumValueInfo RejectReason;

		[DataMember]
		public OrderNoteDetail AdditionalCommentsNote;
	}
}