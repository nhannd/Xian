using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class MrnDetail : DataContractBase
    {
        public MrnDetail(string id, string assigningAuthority)
        {
            this.Id = id;
            this.AssigningAuthority = assigningAuthority;
        }

        public MrnDetail()
        {
        }

        [DataMember]
        public string Id;

        [DataMember]
        public string AssigningAuthority;
    }
}
