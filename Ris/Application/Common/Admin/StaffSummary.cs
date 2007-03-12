using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class StaffSummary : DataContractBase
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

        // Member for Practitioner
        [DataMember]
        public string LicenseNumber;
    }
}
