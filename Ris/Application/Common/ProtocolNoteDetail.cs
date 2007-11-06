using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ProtocolNoteDetail : DataContractBase
    {
        public ProtocolNoteDetail(StaffSummary author, DateTime timeStamp, string text)
        {
            Author = author;
            TimeStamp = timeStamp;
            Text = text;
        }

        [DataMember]
        public StaffSummary Author;

        [DataMember] 
        public DateTime TimeStamp;

        [DataMember]
        public string Text;
    }
}