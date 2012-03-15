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

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
	[DataContract]
	public class GetConversationResponse : DataContractBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="orderRef"></param>
		/// <param name="orderNotes"></param>
		/// <param name="count"></param>
		public GetConversationResponse(EntityRef orderRef, List<OrderNoteDetail> orderNotes, int count)
		{
			this.OrderRef = orderRef;
			this.OrderNotes = orderNotes;
			this.NoteCount = count;
		}

		/// <summary>
		/// Current OrderRef for the requested order.
		/// </summary>
		[DataMember]
		public EntityRef OrderRef;

		/// <summary>
		/// List of order notes in the conversation matching the specified filters,
		/// or null if <see cref="GetConversationRequest.CountOnly"/> was true.
		/// </summary>
		[DataMember]
		public List<OrderNoteDetail> OrderNotes;

		/// <summary>
		/// Count of notes in the conversation.
		/// </summary>
		[DataMember]
		public int NoteCount;
	}
}
