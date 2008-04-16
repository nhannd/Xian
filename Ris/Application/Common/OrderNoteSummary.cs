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
        public OrderNoteSummary(EntityRef orderNoteRef, string category, DateTime? creationTime, DateTime? sentTime, StaffSummary author, bool isAcknowledged, string noteBody)
        {
            OrderNoteRef = orderNoteRef;
            Category = category;
            CreationTime = creationTime;
            SentTime = sentTime;
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
        public DateTime? SentTime;

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