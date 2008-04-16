using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.OrderNotes
{
    [DataContract]
    public class QueryNoteboxResponse : DataContractBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="noteboxItems"></param>
        /// <param name="itemCount"></param>
        public QueryNoteboxResponse(List<OrderNoteboxItemSummary> noteboxItems, int itemCount)
        {
            NoteboxItems = noteboxItems;
            ItemCount = itemCount;
        }

        /// <summary>
        /// List of notebox items.
        /// </summary>
        [DataMember]
        public List<OrderNoteboxItemSummary> NoteboxItems;

        /// <summary>
        /// Count of total items in the notebox.
        /// </summary>
        [DataMember]
        public int ItemCount;
    }
}
