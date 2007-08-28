using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class StaffDetail : DataContractBase
    {
        public StaffDetail(string staffId, EnumValueInfo staffType, PersonNameDetail personNameDetail)
        {
            this.StaffId = staffId;
            this.StaffType = staffType;
            this.Name = personNameDetail;
        }

        public StaffDetail()
        {
            this.Name = new PersonNameDetail();
        }

        [DataMember]
        public string StaffId;

        [DataMember]
        public EnumValueInfo StaffType;

        [DataMember]
        public PersonNameDetail Name;
    }
}
