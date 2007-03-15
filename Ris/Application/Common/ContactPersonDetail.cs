using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ContactPersonDetail : DataContractBase
    {
        public ContactPersonDetail(EnumValueInfo type, string name, string address, string homePhoneNumber, string businessPhoneNumber, EnumValueInfo relationship)
        {
            this.Type = type;
            this.Name = name;
            this.Address = address;
            this.HomePhoneNumber = homePhoneNumber;
            this.BusinessPhoneNumber = businessPhoneNumber;
            this.Relationship = relationship;
        }
        
        [DataMember]
        public EnumValueInfo Type;

        [DataMember]
        public string Name;

        [DataMember]
        public string Address;

        [DataMember]
        public string HomePhoneNumber;

        [DataMember]
        public string BusinessPhoneNumber;

        [DataMember]
        public EnumValueInfo Relationship;
    }
}
