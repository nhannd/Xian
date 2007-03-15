using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class EmailAddressDetail : DataContractBase
    {
        public EmailAddressDetail(string address, DateTime? validRangeFrom, DateTime? validRangeUntil)
        {
            this.Address = address;
            this.ValidRangeFrom = validRangeFrom;
            this.ValidRangeUntil = validRangeUntil;
        }
    
        [DataMember]
        public string Address;

        [DataMember]
        public DateTime? ValidRangeFrom;

        [DataMember]
        public DateTime? ValidRangeUntil;
    }
}
