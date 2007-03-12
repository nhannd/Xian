using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class AddressDetail : DataContractBase
    {
        [DataMember]
        public string Street;

        [DataMember]
        public string Unit;

        [DataMember]
        public string City;

        [DataMember]
        public string Province;

        [DataMember]
        public string PostalCode;

        [DataMember]
        public string Country;

        //TODO Enum field
        //[DataMember]
        //public AddressType Type;

        [DataMember]
        public DateTime? ValidRangeFrom;

        [DataMember]
        public DateTime? ValidRangeUntil;
    }
}
