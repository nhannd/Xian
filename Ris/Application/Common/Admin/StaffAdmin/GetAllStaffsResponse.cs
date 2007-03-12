using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.StaffAdmin
{
    [DataContract]
    public class GetAllStaffsResponse : DataContractBase
    {
        [DataMember]
        public StaffListItem[] Staffs;
    }

    [DataContract]
    public class StaffListItem : DataContractBase
    {
        [DataMember]
        public EntityRef StaffRef;

        [DataMember]
        public string FamilyName;

        [DataMember]
        public string GivenName;

        [DataMember]
        public string MiddleName;

        [DataMember]
        public string Prefix;

        [DataMember]
        public string Suffix;

        [DataMember]
        public string Degree;

        [DataMember]
        public string LicenseNumber;
    }
}
