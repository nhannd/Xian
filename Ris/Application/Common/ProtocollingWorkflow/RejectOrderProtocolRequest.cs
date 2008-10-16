#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
	[DataContract]
	public class RejectOrderProtocolRequest : UpdateOrderProtocolRequest
	{
		public RejectOrderProtocolRequest(
			EntityRef orderRef, 
			List<ProtocolDetail> protocols, 
			List<OrderNoteDetail> orderNotes, 
			EnumValueInfo rejectReason, 
			OrderNoteDetail additionalCommentsNote)
			: base(orderRef, protocols, orderNotes)
		{
			this.RejectReason = rejectReason;
			this.AdditionalCommentsNote = additionalCommentsNote;
		}

		public RejectOrderProtocolRequest(EntityRef orderRef, EnumValueInfo rejectReason, OrderNoteDetail additionalCommentsNote)
			: this(orderRef, null, null, rejectReason, additionalCommentsNote)
		{
		}

		[DataMember]
		public EnumValueInfo RejectReason;

		[DataMember]
		public OrderNoteDetail AdditionalCommentsNote;
	}

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
			EnumValueInfo rejectReason, 
			OrderNoteDetail additionalCommentsNote)
			: this(protocolAssignmentStepRef, null, null, rejectReason, additionalCommentsNote)
		{
		}

		[DataMember]
		public EnumValueInfo RejectReason;

		[DataMember]
		public OrderNoteDetail AdditionalCommentsNote;
	}
}