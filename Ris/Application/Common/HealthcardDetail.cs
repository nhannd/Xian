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
        public HealthcardDetail(string id, string assigningAuthority, string versionCode, DateTime? expiryDate)
        {
            this.Id = id;
            this.AssigningAuthority = assigningAuthority;
            this.VersionCode = versionCode;
            this.ExpiryDate = expiryDate;
        }

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