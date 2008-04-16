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
        public GetConversationResponse(List<OrderNoteDetail> orderNotes)
        {
            OrderNotes = orderNotes;
        }

        /// <summary>
        /// List of order notes in the conversation.
        /// </summary>
        [DataMember]
        public List<OrderNoteDetail> OrderNotes;
    }
}
