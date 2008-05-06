using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
    [DataContract]
    public class GetConversationResponse : DataContractBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="orderNotes"></param>
        /// <param name="count"></param>
        public GetConversationResponse(List<OrderNoteDetail> orderNotes, int count)
        {
            OrderNotes = orderNotes;
        	NoteCount = count;
        }

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
