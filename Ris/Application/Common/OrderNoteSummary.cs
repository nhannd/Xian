using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

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
        /// <param name="onBehalfOfGroup"></param>
        /// <param name="isAcknowledged"></param>
		/// <param name="urgent"></param>
		/// <param name="noteBody"></param>
        public OrderNoteSummary(EntityRef orderNoteRef, string category, DateTime? creationTime, DateTime? postTime,
			StaffSummary author, StaffGroupSummary onBehalfOfGroup, bool isAcknowledged, bool urgent, string noteBody)
        {
            OrderNoteRef = orderNoteRef;
            Category = category;
            CreationTime = creationTime;
            PostTime = postTime;
            Author = author;
        	OnBehalfOfGroup = onBehalfOfGroup;
            IsAcknowledged = isAcknowledged;
        	Urgent = urgent;
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

		[DataMember]
		public StaffGroupSummary OnBehalfOfGroup;

        /// <summary>
        /// Gets a value indicating whether the note has been acknowledged by all recipients.
        /// </summary>
        [DataMember]
        public bool IsAcknowledged;

        [DataMember]
        public bool Urgent;

        [DataMember]
        public string NoteBody;
    }
}
