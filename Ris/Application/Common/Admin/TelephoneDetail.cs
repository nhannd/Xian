using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class TelephoneDetail : DataContractBase
    {
        [DataMember]
        public string CountryCode;

        [DataMember]
        public string AreaCode;

        [DataMember]
        public string Number;

        [DataMember]
        public string Extension;

        // TODO Enum fields
        //[DataMember]
        //public TelephoneUse Use;
        //[DataMember]
        //public TelephoneEquipment Equipment;

        [DataMember]
        public DateTime? ValidRangeFrom;

        [DataMember]
        public DateTime? ValidRangeUntil;
    }
}
