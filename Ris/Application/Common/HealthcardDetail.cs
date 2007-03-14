using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class HealthcardDetail : DataContractBase
    {
        [DataMember]
        public string Id;

        [DataMember]
        public string AssigningAuthority;

        [DataMember]
        public string VersionCode;

        [DataMember]
        public DateTime? ExpiryDate;
    }
}