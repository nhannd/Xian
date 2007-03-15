using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class NoteDetail : DataContractBase
    {
        public NoteDetail(string text, string severity, DateTime timeStamp)
        {
            this.Text = text;
            this.Severity = severity;
            this.TimeStamp = timeStamp;
        }

        [DataMember]
        public string Text;

        [DataMember]
        public string Severity;

        [DataMember]
        public DateTime TimeStamp;
    }
}
