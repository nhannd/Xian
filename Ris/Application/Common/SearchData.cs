using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class SearchData : DataContractBase
    {
        [DataMember]
        public string MrnID;

        [DataMember]
        public string MrnAssigningAuthority;

        [DataMember]
        public string HealthcardID;

        [DataMember]
        public string FamilyName;

        [DataMember]
        public string GivenName;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public bool ShowActiveOnly;
    }
}
