using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class NoteDetail : DataContractBase, ICloneable
    {
        public NoteDetail(string comment, 
            NoteCategorySummary category, 
            StaffSummary createdBy, 
            DateTime? timeStamp,
            DateTime? validRangeFrom,
            DateTime? validRangeUntil)
        {
            this.Comment = comment;
            this.Category = category;
            this.CreatedBy = createdBy;
            this.TimeStamp = timeStamp;
            this.ValidRangeFrom = validRangeFrom;
            this.ValidRangeUntil = validRangeUntil;
        }

        public NoteDetail()
        {
        }

        [DataMember]
        public string Comment;

        [DataMember]
        public NoteCategorySummary Category;

        [DataMember]
        public StaffSummary CreatedBy;

        [DataMember]
        public DateTime? TimeStamp;

        [DataMember]
        public DateTime? ValidRangeFrom;

        [DataMember]
        public DateTime? ValidRangeUntil;

        #region ICloneable Members

        public object Clone()
        {
            NoteDetail clone = new NoteDetail();
            clone.Comment = this.Comment;
            clone.Category = (NoteCategorySummary)this.Category.Clone();
            clone.CreatedBy = (StaffSummary)this.CreatedBy.Clone();
            clone.TimeStamp = this.TimeStamp;
            clone.ValidRangeFrom = this.ValidRangeFrom;
            clone.ValidRangeUntil = this.ValidRangeUntil;

            return clone;
        }

        #endregion    
    }
}
