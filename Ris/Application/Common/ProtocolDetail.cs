using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ProtocolDetail : DataContractBase
    {
        public ProtocolDetail()
        {
            Codes = new List<ProtocolCodeDetail>();
        }

        [DataMember] 
        public StaffSummary Author;

        [DataMember]
        public bool ApprovalRequired;

        [DataMember]
        public List<ProtocolCodeDetail> Codes;

        [DataMember]
        public List<ProtocolNoteDetail> Notes;
    }
}