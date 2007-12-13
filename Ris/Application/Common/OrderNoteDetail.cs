using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class OrderNoteDetail : DataContractBase, ICloneable
    {
        /// <summary>
        /// Constructor for use by client in creating a <see cref="OrderNoteDetail"/> for a new (never saved) note.
        /// It will not have a creation time or author.
        /// </summary>
        /// <param name="comment"></param>
        public OrderNoteDetail(string comment)
        {
            this.Comment = comment;
        }

        /// <summary>
        /// Constructor for use by server in creating a <see cref="OrderNoteDetail"/> for an existing order note.
        /// </summary>
        /// <param name="creationTime"></param>
        /// <param name="author"></param>
        /// <param name="comment"></param>
        public OrderNoteDetail(DateTime? creationTime, StaffSummary author, string comment)
        {
            this.CreationTime = creationTime;
            this.Author = author;
            this.Comment = comment;
        }

        [DataMember]
        public DateTime? CreationTime;

        [DataMember]
        public StaffSummary Author;

        [DataMember]
        public string Comment;

        #region ICloneable Members

        public object Clone()
        {
            return new OrderNoteDetail(
                this.CreationTime,
                this.Author == null ? null : (StaffSummary)this.Author.Clone(),
                this.Comment);
        }

        #endregion
    }
}
