using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class OrderNoteSummary : DataContractBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderNoteRef"></param>
        /// <param name="category"></param>
        /// <param name="creationTime"></param>
        /// <param name="postTime"></param>
        /// <param name="author"></param>
        /// <param name="isAcknowledged"></param>
        /// <param name="noteBody"></param>
        public OrderNoteSummary(EntityRef orderNoteRef, string category, DateTime? creationTime, DateTime? postTime, StaffSummary author, bool isAcknowledged, string noteBody)
        {
            OrderNoteRef = orderNoteRef;
            Category = category;
            CreationTime = creationTime;
            PostTime = postTime;
            Author = author;
            IsAcknowledged = isAcknowledged;
            NoteBody = noteBody;
        }

        [DataMember]
        public EntityRef OrderNoteRef;

        [DataMember]
        public string Category;

        [DataMember]
        public DateTime? CreationTime;

        [DataMember]
        public DateTime? PostTime;

        [DataMember]
        public StaffSummary Author;

        /// <summary>
        /// Gets a value indicating whether the note has been acknowledged by all recipients.
        /// </summary>
        [DataMember]
        public bool IsAcknowledged;

        [DataMember]
        public string NoteBody;
    }
}